using Billy.Billing.Application.DTOs;
using Billy.Billing.Models;
using Billy.Billing.Persistence;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Cache.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public class LocalBillsService : IBillsService, IDisposable
    {
        private readonly IBillingUnitOfWorkFactory _billingUnitOfWorkFactory;
        private readonly SerialDisposable _billsChangesSubscription;

        public LocalBillsService(
            IBillingUnitOfWorkFactory _billingUnitOfWorkFactory
            )
        {
            this._billingUnitOfWorkFactory = _billingUnitOfWorkFactory ?? throw new ArgumentNullException(nameof(_billingUnitOfWorkFactory));

            this._added_Subject = new Subject<BillDto>().DisposeWith(this._disposables);
            this._updated_Subject = new Subject<IReadOnlyCollection<BillDto>>().DisposeWith(this._disposables);
            this._removed_Subject = new Subject<IReadOnlyCollection<long>>().DisposeWith(this._disposables);

            //this._isBusy_BehaveiorSubject = new BehaviorSubject<bool>(false).DisposeWith(this._disposables);
            //this.IsBusyChanged = this._isBusy_BehaveiorSubject.DistinctUntilChanged();

            this._billsChangesSubscription = new SerialDisposable().DisposeWith(this._disposables);
        }

        // READ

        // TODO: instance members of subjects are not thread safe, consider using some kind of locking
        private readonly ISubject<BillDto> _added_Subject;
        public IObservable<BillDto> Added => this._added_Subject;
        private readonly ISubject<IReadOnlyCollection<BillDto>> _updated_Subject;
        public IObservable<IReadOnlyCollection<BillDto>> Updated => this._updated_Subject;
        private readonly ISubject<IReadOnlyCollection<long>> _removed_Subject;
        public IObservable<IReadOnlyCollection<long>> Removed => this._removed_Subject;

        public Task<IReadOnlyCollection<BillDto>> GetAsync()
        {
            return this.GetAsync(null);
        }

        public async Task<IReadOnlyCollection<BillDto>> GetAsync(BillCriteria billCriteria)
        {
            IReadOnlyCollection<Bill> bills = null;

            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                bills = await uow.Bills.GetMultipleAsync(billCriteria).ConfigureAwait(false);
            }

            var billDTOs = bills.Select(s => BillDto.From(s)).ToList().AsReadOnly();

            return billDTOs;
        }

        public async Task<BillDto> CreateAndAddAsync(IDictionary<string, object> data)
        {
            //using (var uow = this._billingUowFactoryMethod.Invoke())
            Bill addedBill = null;

            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                try
                {
                    addedBill = await uow.Bills.CreateAndAddAsync(data).ConfigureAwait(false);
                    await uow.CommitAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Debug.WriteLine("error creating bill from service");
                    throw;
                }
            }

            if (addedBill == null)
                return null;

            var addedBillDto = BillDto.From(addedBill);

            this._added_Subject.OnNext(addedBillDto);

            return addedBillDto;
        }

        public async Task UpdateAsync(long billId, IDictionary<string, object> changes)
        {
            try
            {
                Bill updatedBill = null;

                using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
                {
                    try
                    {
                        await uow.Bills.UpdateAsync(billId, changes).ConfigureAwait(false);
                        await uow.CommitAsync().ConfigureAwait(false);
                        updatedBill = await uow.Bills.GetByIdAsync(billId).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        await uow.RollbackAsync();
                        throw;
                    }
                }

                this._updated_Subject.OnNext(new[] { BillDto.From(updatedBill) });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task<bool> RemoveAsync(long id)
        {
            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                try
                {
                    await uow.Bills.RemoveAsync(id).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    Debug.WriteLine("error removing bill from serive");
                    //throw;
                    return false;
                }
            }

            this._removed_Subject.OnNext(new[] { id });

            return true;
        }

        public IObservable<IChangeSet<BillDto>> GetBillsChanges(IObservable<BillCriteria> whenCriteriaChanged)
        {
            IObservable<IChangeSet<BillDto>> result = null;

            var changes = ObservableChangeSet
                .Create<BillDto, long>(cache =>
                {
                    var crudSubscriptions = new CompositeDisposable();

                    whenCriteriaChanged
                        .Subscribe(criteria =>
                        {
                            cache.Edit(async updater =>
                            {
                                var billDtos = await this.GetAsync(criteria).ConfigureAwait(false);
                                updater.Clear();
                                updater.AddOrUpdate(billDtos);
                            });

                        })
                        .DisposeWith(crudSubscriptions);

                    Observable
                        .CombineLatest(this.Added, whenCriteriaChanged, (added, criteria) => (added, criteria))
                        .Subscribe(async addInfo =>
                        {
                            var (added, criteria) = addInfo;

                            Bill addedBill = null;

                            using (var billingUow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
                            {
                                var criteriaWithId = criteria with { Ids = new[] { added.Id } };
                                addedBill = await billingUow.Bills.GetSingleAsync(criteriaWithId).ConfigureAwait(false);

                                if (addedBill is not null)
                                    return;

                                cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(BillDto.From(addedBill)));
                            }
                        })
                            .DisposeWith(crudSubscriptions);

                    Observable
                        .CombineLatest(this.Updated, whenCriteriaChanged, (updated, criteria) => (updated, criteria))
                        .Subscribe(async updateInfo =>
                        {
                            var (updated, criteria) = updateInfo;
                            IReadOnlyCollection<Bill> updatedBills = null;

                            using (var billingUow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
                            {
                                var criteriaWithIds = criteria with { Ids = updated.Select(x => x.Id) };
                                updatedBills = await billingUow.Bills.GetMultipleAsync(criteriaWithIds).ConfigureAwait(false);

                                cache.Edit(cacheUpdater =>
                                {
                                    var billsChanges = updatedBills.Select(bill =>
                                    {
                                        var oldVersion = cacheUpdater.Lookup(bill.Id);
                                        var newVersion = BillDto.From(bill);

                                        return new Change<BillDto, long>(ChangeReason.Update, bill.Id, newVersion, oldVersion);
                                    });

                                    cacheUpdater.Clone(new ChangeSet<BillDto, long>(billsChanges));
                                });
                            }
                        })
                        .DisposeWith(crudSubscriptions);

                    Observable
                        .CombineLatest(this.Removed, whenCriteriaChanged, (removedIds, criteria) => (removedIds, criteria))
                        .Subscribe(removedInfo =>
                        {
                            var (removedIds, criteria) = removedInfo;
                            cache.Edit(cacheUpdater => cacheUpdater.RemoveKeys(removedIds));
                        })
                        .DisposeWith(crudSubscriptions);

                    return crudSubscriptions;
                },
                x => x.Id)
                .RefCount();

            return result;
        }

        #region IDisposable

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