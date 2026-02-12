using Billy.Billing.Application.DTOs;
using Billy.Billing.Models;
using Billy.Billing.Persistence;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Cache.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public class LocalSuppliersService : ISuppliersService, IDisposable
    {
        private readonly IBillingUnitOfWorkFactory _billingUnitOfWorkFactory;
        private readonly SerialDisposable _suppliersChangesSubscription;

        public LocalSuppliersService(
            IBillingUnitOfWorkFactory _billingUnitOfWorkFactory
            )
        {
            this._billingUnitOfWorkFactory = _billingUnitOfWorkFactory ?? throw new ArgumentNullException(nameof(_billingUnitOfWorkFactory));

            this._added_Subject = new Subject<SupplierDto>().DisposeWith(this._disposables);
            this._updated_Subject = new Subject<IReadOnlyCollection<SupplierDto>>().DisposeWith(this._disposables);
            this._removed_Subject = new Subject<IReadOnlyCollection<long>>().DisposeWith(this._disposables);

            //this._isBusy_BehaveiorSubject = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
            //this.IsBusyChanged = this._isBusy_BehaveiorSubject.DistinctUntilChanged();

            this._suppliersChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);
            var supplierDtoChangeSets = ObservableChangeSet.Create<SupplierDto, long>(
                async cache =>
                {
                    //await Task.Delay(3_000);
                    var supplierDTOs = await this.GetAllAsync().ConfigureAwait(false);

                    cache.AddOrUpdate(supplierDTOs);

                    var crudSubscriptions = new CompositeDisposable();

                    this.Added
                        .Subscribe(supplierDto => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(supplierDto)))
                        .DisposeWith(crudSubscriptions);

                    this.Updated
                        .Subscribe(e =>
                        {
                            cache.Edit(cacheUpdater =>
                            {
                                var updates = e
                                    .Select(
                                        updatedSupplierDto =>
                                        {
                                            var oldVersion = cacheUpdater.Lookup(updatedSupplierDto.Id);

                                            return new Change<SupplierDto, long>(
                                                ChangeReason.Update,
                                                updatedSupplierDto.Id,
                                                updatedSupplierDto,
                                                oldVersion);
                                        })
                                    .ToArray();

                                var changes = new ChangeSet<SupplierDto, long>(updates);

                                cacheUpdater.Clone(changes);
                            });
                        })
                        .DisposeWith(crudSubscriptions);

                    this.Removed
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.Remove(e)))
                        .DisposeWith(crudSubscriptions);

                    return crudSubscriptions;
                },
                x => x.Id)
                //this.SuppliersChanges = supplierDtoChangeSets
                //    //.RefCount()
                //    //.Transform(supplier => this._supplierViewModelFactoryMethod.Invoke(supplier), new ParallelisationOptions(ParallelType.Parallelise))
                //    //.DisposeMany()
                //    .Sort(SortExpressionComparer<SupplierDto>.Ascending(vm => vm.Name))
                //.Multicast(new ReplaySubject<IChangeSet<SupplierDto, int>>())
                //.AutoConnect(1, subscription => this._suppliersChangesSubscription.Disposable = subscription)
                ;
            this.Suppliers = supplierDtoChangeSets
                //.Sort(SortExpressionComparer<SupplierDto>.Ascending(vm => vm.Name))
                .RefCount()
                .AsObservableCache();// applyLocking: false);
        }

        // READ

        // TODO: instance members of subjects are not thread safe, consider using some kind of locking
        private readonly ISubject<SupplierDto> _added_Subject;
        public IObservable<SupplierDto> Added => this._added_Subject;
        private readonly ISubject<IReadOnlyCollection<SupplierDto>> _updated_Subject;
        public IObservable<IReadOnlyCollection<SupplierDto>> Updated => this._updated_Subject;
        private readonly ISubject<IReadOnlyCollection<long>> _removed_Subject;
        public IObservable<IReadOnlyCollection<long>> Removed => this._removed_Subject;

        public async Task<IReadOnlyCollection<SupplierDto>> GetAllAsync()
        {
            IReadOnlyCollection<Supplier> suppliers;

            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                suppliers = await uow.Suppliers.GetMultipleAsync().ConfigureAwait(false);
            }

            var supplierDTOs = suppliers.Select(s => new SupplierDto(s)).ToList();

            return supplierDTOs;
        }

        public IObservable<IChangeSet<SupplierDto, long>> SuppliersChanges { get; }

        public IObservableCache<SupplierDto, long> Suppliers { get; }

        //private readonly BehaviorSubject<bool> _isBusy_BehaveiorSubject;
        //public IObservable<bool> IsBusyChanged { get; }

        //public IQbservable<Supplier> Suppliers => throw new NotImplementedException();

        // WRITE

        public async Task<SupplierDto> CreateAndAddAsync(IDictionary<string, object> data)
        {
            //using (var uow = this._billingUowFactoryMethod.Invoke())
            Supplier addedSupplier = null;

            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                try
                {
                    addedSupplier = await uow.Suppliers.CreateAndAddAsync(data).ConfigureAwait(false);
                    await uow.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Debug.WriteLine("error creating supplier from service");
                    throw;
                }
            }

            if (addedSupplier == null)
                return null;

            var addedSupplierDto = new SupplierDto(addedSupplier);

            this._added_Subject.OnNext(addedSupplierDto);

            return addedSupplierDto;
        }

        public async Task UpdateAsync(long supplierId, IDictionary<string, object> changes)
        {

            Supplier updatedSupplier = null;
            try
            {
                using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
                {
                    await uow.Suppliers.UpdateAsync(supplierId, changes).ConfigureAwait(false);
                    await uow.CommitAsync().ConfigureAwait(false);
                    updatedSupplier = await uow.Suppliers.GetByIdAsync(supplierId).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating supplier from service.");
                Debug.WriteLine(ex);
                throw;
            }

            var addedSupplierDto = new SupplierDto(updatedSupplier);

            this._updated_Subject.OnNext(new[] { addedSupplierDto });
        }

        //private readonly ReaderWriterLockSlim _readerWriterLockSlim;
        //private async Task Aquire()
        //{
        //    var x = new Nito.AsyncEx.AsyncReaderWriterLock();
        //    await x.
        //}

        public async Task<bool> RemoveAsync(long id)
        {
            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                try
                {
                    await uow.Suppliers.RemoveAsync(id).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Debug.WriteLine("error removing supplier from serive");
                    //throw;
                    return false;
                }
            }

            this._removed_Subject.OnNext(new[] { id });

            return true;
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            //base.Dispose(isDisposing);
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}