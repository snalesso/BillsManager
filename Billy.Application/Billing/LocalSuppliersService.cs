using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Billy.Core.Domain.Billing.Application.DTOs;
using Billy.Core.Domain.Billing.Persistence.SQL.MSSQLServer;
using Billy.Domain.Billing.Models;
using Billy.Domain.Billing.Persistence;
using DynamicData;
using DynamicData.Kernel;

namespace Billy.Billing.Application
{
    public class LocalSuppliersService : ISuppliersService, IDisposable
    {
        private readonly IBillingUnitOfWorkFactory _billingUnitOfWorkFactory;
        //private readonly Func<IBillingUnitOfWork> _billingUowFactoryMethod;
        private readonly SerialDisposable _suppliersChangesSubscription;

        public LocalSuppliersService(
            IBillingUnitOfWorkFactory _billingUnitOfWorkFactory
            //, Func<IBillingUnitOfWork> billingUowFactoryMethod
            )
        {
            this._billingUnitOfWorkFactory = _billingUnitOfWorkFactory ?? throw new ArgumentNullException(nameof(_billingUnitOfWorkFactory));
            //this._billingUowFactoryMethod = billingUowFactoryMethod ?? throw new ArgumentNullException(nameof(billingUowFactoryMethod));

            this._added_Subject = new Subject<SupplierDTO>().DisposeWith(this._disposables);
            this._updated_Subject = new Subject<IReadOnlyCollection<SupplierDTO>>().DisposeWith(this._disposables);
            this._removed_Subject = new Subject<IReadOnlyCollection<int>>().DisposeWith(this._disposables);

            //this._isBusy_BehaveiorSubject = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
            //this.IsBusyChanged = this._isBusy_BehaveiorSubject.DistinctUntilChanged();

            this._suppliersChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);
            this.SuppliersChanges = ObservableChangeSet.Create<SupplierDTO, int>(
                async cache =>
                {
                    var supplierDTOs = await this.GetAllAsync();

                    cache.AddOrUpdate(supplierDTOs);

                    var crudSubscriptions = new CompositeDisposable();

                    this.Added
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(e)))
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

                                            return new Change<SupplierDTO, int>(
                                                ChangeReason.Update,
                                                updatedSupplierDto.Id,
                                                updatedSupplierDto,
                                                oldVersion);
                                        })
                                    .ToArray();

                                var changes = new ChangeSet<SupplierDTO, int>(updates);

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
                // TODO: add synchronization to handle multiple subscriptions?
                .RefCount()
                //.Transform(supplier => this._supplierViewModelFactoryMethod.Invoke(supplier), new ParallelisationOptions(ParallelType.Parallelise))
                .DisposeMany()
                .Multicast(new ReplaySubject<IChangeSet<SupplierDTO, int>>())
                .AutoConnect(1, subscription => this._suppliersChangesSubscription.Disposable = subscription);
        }

        // READ

        // TODO: instance members of subjects are not thread safe, consider using some kind of locking
        private readonly ISubject<SupplierDTO> _added_Subject;
        public IObservable<SupplierDTO> Added => this._added_Subject;
        private readonly ISubject<IReadOnlyCollection<SupplierDTO>> _updated_Subject;
        public IObservable<IReadOnlyCollection<SupplierDTO>> Updated => this._updated_Subject;
        private readonly ISubject<IReadOnlyCollection<int>> _removed_Subject;
        public IObservable<IReadOnlyCollection<int>> Removed => this._removed_Subject;

        public async Task<IReadOnlyCollection<SupplierDTO>> GetAllAsync()
        {
            IReadOnlyCollection<Supplier> suppliers;

            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync())
            {
                suppliers = await uow.Suppliers.GetMultipleAsync();
            }

            var supplierDTOs = suppliers.Select(s => new SupplierDTO(s)).ToList();

            return supplierDTOs;
        }

        public IObservable<IChangeSet<SupplierDTO, int>> SuppliersChanges { get; }

        //private readonly BehaviorSubject<bool> _isBusy_BehaveiorSubject;
        //public IObservable<bool> IsBusyChanged { get; }

        //public IQbservable<Supplier> Suppliers => throw new NotImplementedException();

        // WRITE

        public async Task<SupplierDTO> CreateAndAddAsync(IDictionary<string, object> data)
        {
            //using (var uow = this._billingUowFactoryMethod.Invoke())
            Supplier addedSupplier = null;

            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync())
            {

                try
                {
                    addedSupplier = await uow.Suppliers.CreateAndAddAsync(data);
                    await uow.CommitAsync();
                }
                catch (Exception)
                {
                    Debug.WriteLine("error creating supplier from service");
                    throw;
                }
            }

            if (addedSupplier == null)
                return null;

            var addedSupplierDto = new SupplierDTO(addedSupplier);

            this._added_Subject.OnNext(addedSupplierDto);

            return addedSupplierDto;
        }

        public async Task UpdateAsync(int supplierId, IDictionary<string, object> changes)
        {
            Supplier updatedSupplier = null;

            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync())
            {
                try
                {
                    await uow.Suppliers.UpdateAsync(supplierId, changes);
                    await uow.CommitAsync();

                    try
                    {
                        // TODO: read outside any transaction
                        updatedSupplier = await uow.Suppliers.GetByIdAsync(supplierId);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("error reading updated supplier from service");
                        throw;
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("error updating supplier from service");
                    throw;
                }
            }

            var addedSupplierDto = new SupplierDTO(updatedSupplier);

            this._updated_Subject.OnNext(new[] { addedSupplierDto });
        }

        //private readonly ReaderWriterLockSlim _readerWriterLockSlim;
        //private async Task Aquire()
        //{
        //    var x = new Nito.AsyncEx.AsyncReaderWriterLock();
        //    await x.
        //}

        public async Task<bool> RemoveAsync(int id)
        {
            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync())
            {
                try
                {
                    await uow.Suppliers.RemoveAsync(id);
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