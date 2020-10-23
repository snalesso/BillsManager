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

        public async Task<IReadOnlyCollection<BillDto>> GetAsync()
        {
            IReadOnlyCollection<Bill> bills;

            //using (var uow = this._billingUowFactoryMethod.Invoke())
            using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            {
                bills = await uow.Bills.GetMultipleAsync().ConfigureAwait(false);
            }

            var billDTOs = bills.Select(s => new BillDto(s)).ToList();

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

            var addedBillDto = new BillDto(addedBill);

            this._added_Subject.OnNext(addedBillDto);

            return addedBillDto;
        }

        public async Task UpdateAsync(long billId, IDictionary<string, object> changes)
        {

            Bill updatedBill = null;
            try
            {
                using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
                {
                    await uow.Bills.UpdateAsync(billId, changes).ConfigureAwait(false);
                    await uow.CommitAsync().ConfigureAwait(false);
                    updatedBill = await uow.Bills.GetByIdAsync(billId).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error updating bill from service.");
                Debug.WriteLine(ex);
                throw;
            }

            //try
            //{
            //    using (var uow = await this._billingUnitOfWorkFactory.CreateAsync().ConfigureAwait(false))
            //    {
            //        // TODO: read outside any transaction
            //        updatedBill = await uow.Bills.GetByIdAsync(billId).ConfigureAwait(false);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("Error reading updated bill from service.");
            //    Debug.WriteLine(ex);
            //    throw;
            //}

            var addedBillDto = new BillDto(updatedBill);

            this._updated_Subject.OnNext(new[] { addedBillDto });
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