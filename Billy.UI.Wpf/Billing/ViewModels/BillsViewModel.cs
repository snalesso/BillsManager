using Billy.Billing.Application.DTOs;
using Billy.Billing.Persistence;
using Billy.Billing.Services;
using Billy.UI.Wpf.Common.Services;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.PLinq;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Billing.ViewModels
{
    public class BillsViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IDialogService _dialogService;
        private readonly IBillingService _billingService;

        private readonly ISourceCache<BillDto, long> _billsSourceCache;

        //private readonly Func<AddBillViewModel> _addBillViewModelFactoryMethod;
        //private readonly Func<BillDto, EditBillViewModel> _editBillViewModelFactoryMethod;

        private readonly SerialDisposable _billsSubscription;

        #endregion

        #region ctors

        public BillsViewModel(
            IDialogService dialogService
            , IBillingService billingService
            //, Func<AddBillViewModel> addBillViewModelFactoryMethod
            //, Func<BillDto, EditBillViewModel> editBillViewModelFactoryMethod
            )
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._billingService = billingService ?? throw new ArgumentNullException(nameof(billingService));
            //this._addBillViewModelFactoryMethod = addBillViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(addBillViewModelFactoryMethod));
            //this._editBillViewModelFactoryMethod = editBillViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editBillViewModelFactoryMethod));

            this._billsSubscription = new SerialDisposable().DisposeWith(this._disposables);
            this._billsSourceCache = new SourceCache<BillDto, long>(x => x.Id).DisposeWith(this._disposables);

            this.WhenSelectionChanged = this.WhenAnyValue(x => x.SelectedBillViewModel).DistinctUntilChanged();
            this.WhenHasSelectionChanged = this.WhenSelectionChanged.Select(x => x != null).DistinctUntilChanged();

            this.LoadBills = ReactiveCommand.Create(
                () =>
                {
                    this._billsSourceCache.Edit(async updater =>
                    {
                        var items = await this._billingService.Bills.GetAsync().ConfigureAwait(false);
                        updater.AddOrUpdate(items);
                    });
                })
                .DisposeWith(this._disposables);
            _ = this.LoadBills.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            //this.ShowAddBillView = ReactiveCommand.CreateFromTask(
            //    () => this._dialogService.ShowDialogAsync(this._addBillViewModelFactoryMethod()))
            //    .DisposeWith(this._disposables);
            //_ = this.ShowAddBillView.ThrownExceptions
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(ex => Debug.WriteLine(ex))
            //    .DisposeWith(this._disposables);

            this.RemoveBill = ReactiveCommand.CreateFromTask(
                async (BillViewModel billViewModel) =>
                {
                    if (billViewModel == null)
                    {
                        // TODO: should throw?
                        return;
                    }

                    if (this.SelectedBillViewModel == billViewModel)
                    {
                        this.SelectedBillViewModel = null;
                    }

                    // TODO: handle if removed failed
                    var wasRemoved = await this._billingService.Bills.RemoveAsync(billViewModel.Id);
                }
                , this.WhenHasSelectionChanged)
                .DisposeWith(this._disposables);
            _ = this.RemoveBill.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            //this.ShowEditBillView = ReactiveCommand.CreateFromTask(
            //     async (BillViewModel billViewModel) =>
            //     {
            //         if (billViewModel == null)
            //         {
            //             // TODO: throw or log, cause this shouldnt happen
            //             return;
            //         }

            //         var editBillVM = this._editBillViewModelFactoryMethod.Invoke(billViewModel.BillDto);
            //         await this._dialogService.ShowDialogAsync(editBillVM);
            //     }
            //     , this.WhenHasSelectionChanged)
            //    .DisposeWith(this._disposables); ;
            //_ = this.ShowEditBillView.ThrownExceptions
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(ex => Debug.WriteLine(ex))
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public IObservable<BillViewModel> WhenSelectionChanged { get; }
        public IObservable<bool> WhenHasSelectionChanged { get; }

        private ReadOnlyObservableCollection<BillViewModel> _billViewModels;
        public ReadOnlyObservableCollection<BillViewModel> BillViewModels
        {
            get { return this._billViewModels; }
            private set => this.Set(ref this._billViewModels, value);
        }

        private BillViewModel _selectedBillViewModel;
        public BillViewModel SelectedBillViewModel
        {
            get => this._selectedBillViewModel;
            set => this.Set(ref this._selectedBillViewModel, value);
        }

        #endregion

        #region methods

        private void SubscribeToBills()
        {
            this._billsSubscription.Disposable =
                Observable
                .StartAsync(
                    async () => await this._billingService.Bills.GetAsync().ConfigureAwait(false),
                    RxApp.TaskpoolScheduler)
                .ToObservableChangeSet(x => x.Id)
                .Transform(
                    bill => new BillViewModel(bill),
                    new ParallelisationOptions(ParallelType.Parallelise))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var billVMs)
                .DisposeMany()
                .Subscribe();

            this.BillViewModels = billVMs;
        }

        public void UnsubscribeFromBills()
        {
            this._billsSubscription.Disposable = null;
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this.SubscribeToBills();

            await base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            this.UnsubscribeFromBills();

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> LoadBills { get; }

        public ReactiveCommand<Unit, Unit> ShowAddBillView { get; }
        public ReactiveCommand<BillViewModel, Unit> ShowEditBillView { get; }
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