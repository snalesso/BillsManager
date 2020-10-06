using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Billy.Domain.Billing.Models;
using Billy.Domain.Billing.Persistence;
using Billy.UI.Wpf.Services;
using DynamicData;
using DynamicData.PLinq;

namespace Billy.UI.Wpf.Presentation.Billing
{
    // TODO: add IsLoading to disable views
    public class BillingViewModelsProxy : IDisposable
    {
        [Obsolete("Should not be accessed directly,")]
        private readonly ISuppliersRepository _suppliersRepository;
        [Obsolete("Should not be accessed directly")]
        private readonly IBillsRepository _billsRepository;
        private readonly Func<Supplier, SupplierViewModel> _supplierViewModelFactoryMethod;
        private readonly Func<Bill, BillViewModel> _billViewModelFactoryMethod;
        private readonly SerialDisposable _suppliersSubscription;
        private readonly SerialDisposable _billsSubscription;

        public BillingViewModelsProxy(
            ISuppliersRepository suppliersRepository,
            IBillsRepository billsRepository,
            Func<Supplier, SupplierViewModel> supplierViewModelFactoryMethod,
            Func<Bill, BillViewModel> billViewModelFactoryMethod)
        {
            this._suppliersRepository = suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository));
            this._billsRepository = billsRepository ?? throw new ArgumentNullException(nameof(billsRepository));
            this._supplierViewModelFactoryMethod = supplierViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(supplierViewModelFactoryMethod));
            this._billViewModelFactoryMethod = billViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(billViewModelFactoryMethod));

            this._suppliersSubscription = new SerialDisposable().DisposeWith(this._disposables);
            this._billsSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this.SupplierViewModelsChanges = ObservableChangeSet.Create<Supplier, int>(
                async cache =>
                {
                    var items = await this._suppliersRepository.GetMultipleAsync();
                    cache.AddOrUpdate(items);

                    var eventsDisposables = new CompositeDisposable();

                    Observable
                        .FromEventPattern<EventHandler<IReadOnlyCollection<Supplier>>, IReadOnlyCollection<Supplier>>(
                            h => this._suppliersRepository.SuppliersAddeded += h,
                            h => this._suppliersRepository.SuppliersAddeded -= h)
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(e.EventArgs)))
                        .DisposeWith(eventsDisposables);

                    Observable
                        .FromEventPattern<EventHandler<IReadOnlyCollection<Supplier>>, IReadOnlyCollection<Supplier>>(
                            h => this._suppliersRepository.SuppliersUpdated += h,
                            h => this._suppliersRepository.SuppliersUpdated -= h)
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(e.EventArgs)))
                        .DisposeWith(eventsDisposables);

                    Observable
                        .FromEventPattern<EventHandler<IReadOnlyCollection<Supplier>>, IReadOnlyCollection<Supplier>>(
                            h => this._suppliersRepository.SuppliersRemoved += h,
                            h => this._suppliersRepository.SuppliersRemoved -= h)
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.RemoveKeys(e.EventArgs.Select(x => x.Id))))
                        .DisposeWith(eventsDisposables);

                    return eventsDisposables;
                },
                x => x.Id)
                // TODO: add synchronization to handle multiple subscriptions?
                .RefCount()
                .Transform(supplier => this._supplierViewModelFactoryMethod.Invoke(supplier), new ParallelisationOptions(ParallelType.Parallelise))
                .DisposeMany()
                .Multicast(new ReplaySubject<IChangeSet<SupplierViewModel, int>>())
                .AutoConnect(1, subscription => this._suppliersSubscription.Disposable = subscription);
            //.RefCount();

            this.BillViewModelsChanges = ObservableChangeSet.Create<Bill, int>(
                async cache =>
                {
                    var items = await this._billsRepository.GetBillsAsync();
                    cache.AddOrUpdate(items);

                    var eventsDisposables = new CompositeDisposable();

                    Observable
                        .FromEventPattern<EventHandler<IReadOnlyCollection<Bill>>, IReadOnlyCollection<Bill>>(
                            h => this._billsRepository.BillsAddeded += h,
                            h => this._billsRepository.BillsAddeded -= h)
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(e.EventArgs)))
                        .DisposeWith(eventsDisposables);

                    Observable
                        .FromEventPattern<EventHandler<IReadOnlyCollection<Bill>>, IReadOnlyCollection<Bill>>(
                            h => this._billsRepository.BillsUpdated += h,
                            h => this._billsRepository.BillsUpdated -= h)
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.AddOrUpdate(e.EventArgs)))
                        .DisposeWith(eventsDisposables);

                    Observable
                        .FromEventPattern<EventHandler<IReadOnlyCollection<Bill>>, IReadOnlyCollection<Bill>>(
                            h => this._billsRepository.BillsRemoved += h,
                            h => this._billsRepository.BillsRemoved -= h)
                        .Subscribe(e => cache.Edit(cacheUpdater => cacheUpdater.RemoveKeys(e.EventArgs.Select(x => x.Id))))
                        .DisposeWith(eventsDisposables);

                    return eventsDisposables;
                },
                x => x.Id)
                // TODO: add synchronization to handle multiple subscriptions?
                .RefCount()
                .Transform(bill => this._billViewModelFactoryMethod(bill), new ParallelisationOptions(ParallelType.Parallelise))
                .DisposeMany()
                .Multicast(new ReplaySubject<IChangeSet<BillViewModel, int>>())
                .AutoConnect(1, subscription => this._billsSubscription.Disposable = subscription);
            //.RefCount();
        }

        #region properties

        public IObservable<IChangeSet<SupplierViewModel, int>> SupplierViewModelsChanges { get; }
        public IObservable<IChangeSet<BillViewModel, int>> BillViewModelsChanges { get; }

        #endregion

        #region methods
        #endregion

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