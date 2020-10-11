using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Billy.Billing.Persistence;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class BillsViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IBillsRepository _billsRepository;
        //private readonly BillingViewModelsProxy _billingViewModelsProxy;

        #endregion

        #region ctors

        public BillsViewModel(
            IBillsRepository billsRepository
            //, BillingViewModelsProxy billingViewModelsProxy
            )
        {
            this._billsRepository = billsRepository ?? throw new ArgumentNullException(nameof(billsRepository));
            //this._billingViewModelsProxy = billingViewModelsProxy ?? throw new ArgumentNullException(nameof(billingViewModelsProxy));

            this.AddNewBill = ReactiveCommand.Create(() => { });
            this.AddNewBill.ThrownExceptions
                // TODO: learn how to use ObserveOn + SubscribeOn in the best way
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.AddNewBill.DisposeWith(this._disposables);

            this.RemoveBill = ReactiveCommand.Create(
                (BillViewModel billViewModel) =>
                {
                    if (this.SelectedBillViewModel == billViewModel)
                    {
                        this.SelectedBillViewModel = null;
                    }

                    //await this._billsRepository.RemoveAsync(billViewModel.Id);
                });
            this.RemoveBill.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.RemoveBill.DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        private ReadOnlyObservableCollection<BillViewModel> _sortedFilteredBillViewModelsROOC;
        public ReadOnlyObservableCollection<BillViewModel> SortedFilteredBillViewModelsROOC
        {
            get { return this._sortedFilteredBillViewModelsROOC; }
            private set => this.Set(ref this._sortedFilteredBillViewModelsROOC, value);
        }

        private BillViewModel _selectedBillViewModel;
        public BillViewModel SelectedBillViewModel
        {
            get => this._selectedBillViewModel;
            set => this.Set(ref this._selectedBillViewModel, value);
        }

        #endregion

        #region methods
        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> AddNewBill { get; }
        public ReactiveCommand<BillViewModel, Unit> RemoveBill { get; }

        #endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}