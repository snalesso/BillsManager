using Billy.Billing.Application.DTOs;
using Billy.Billing.Services;
using Billy.UI.Wpf.Common.Services;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DynamicData;
using DynamicData.Alias;
using DynamicData.PLinq;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Billing.ViewModels
{
    public class SuppliersViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IDialogService _dialogService;
        private readonly IBillingService _billingService;

        private readonly ISourceCache<SupplierDto, long> _suppliersSourceCache;

        private readonly Func<AddSupplierViewModel> _addSupplierViewModelFactoryMethod;
        private readonly Func<SupplierDto, EditSupplierViewModel> _editSupplierViewModelFactoryMethod;

        private readonly SerialDisposable _suppliersSubscription;

        #endregion

        #region ctors

        public SuppliersViewModel(
            IDialogService dialogService
            , IBillingService billingService
            , Func<AddSupplierViewModel> addSupplierViewModelFactoryMethod
            , Func<SupplierDto, EditSupplierViewModel> editSupplierViewModelFactoryMethod
            )
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._billingService = billingService ?? throw new ArgumentNullException(nameof(billingService));
            this._addSupplierViewModelFactoryMethod = addSupplierViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(addSupplierViewModelFactoryMethod));
            this._editSupplierViewModelFactoryMethod = editSupplierViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editSupplierViewModelFactoryMethod));

            this._suppliersSubscription = new SerialDisposable().DisposeWith(this._disposables);
            //new SourceCache<SupplierDto, long>(x => x.Id).DisposeWith(this._disposables);

            this.WhenSelectionChanged = this.WhenAnyValue(x => x.SelectedSupplierViewModel).DistinctUntilChanged();
            this.WhenHasSelectionChanged = this.WhenSelectionChanged.Select(x => x != null).DistinctUntilChanged();

            this.LoadSuppliers = ReactiveCommand.Create(
                () =>
                {
                    this._suppliersSourceCache.Edit(async updater =>
                    {
                        var items = await this._billingService.Suppliers.GetAllAsync().ConfigureAwait(false);
                        updater.AddOrUpdate(items);
                    });
                })
                .DisposeWith(this._disposables);
            _ = this.LoadSuppliers.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            this.ShowAddSupplierView = ReactiveCommand.CreateFromTask(
                async () => await this._dialogService.ShowDialogAsync(this._addSupplierViewModelFactoryMethod()).ConfigureAwait(false))
                .DisposeWith(this._disposables);
            _ = this.ShowAddSupplierView.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            this.RemoveSupplier = ReactiveCommand.CreateFromTask(
                async (SupplierViewModel supplierViewModel) =>
                {
                    if (supplierViewModel == null)
                    {
                        // TODO: should throw?
                        return;
                    }

                    if (this.SelectedSupplierViewModel == supplierViewModel)
                    {
                        this.SelectedSupplierViewModel = null;
                    }

                    // TODO: handle if removed failed
                    var wasRemoved = await this._billingService.Suppliers.RemoveAsync(supplierViewModel.Id).ConfigureAwait(false);
                }
                , this.WhenHasSelectionChanged)
                .DisposeWith(this._disposables);
            _ = this.RemoveSupplier.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            this.ShowEditSupplierView = ReactiveCommand.CreateFromTask(
                 async (SupplierViewModel supplierViewModel) =>
                 {
                     if (supplierViewModel == null)
                     {
                         // TODO: throw or log, cause this shouldnt happen
                         return;
                     }

                     var editSupplierVM = this._editSupplierViewModelFactoryMethod.Invoke(supplierViewModel.SupplierDto);
                     await this._dialogService.ShowDialogAsync(editSupplierVM).ConfigureAwait(false);
                 }
                 , this.WhenHasSelectionChanged)
                .DisposeWith(this._disposables); ;
            _ = this.ShowEditSupplierView.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            //this.SortedFilteredSupplierViewModelsROOC
            //    .ObserveCollectionChanges()
            //    .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Replace)
            //    .Subscribe(x =>
            //    {
            //        if (this._needsReselection)
            //        {
            //            this._needsReselection = false;
            //        }
            //        this.SelectedSupplierViewModel = x.EventArgs.NewItems.Count >= 1 ? x.EventArgs.NewItems[0] as SupplierViewModel : default;
            //    })
            //    .DisposeWith(this._disposables);

            //this._isCollectionSet_OAPH = this
            //    .WhenAnyValue(x => x.SortedFilteredSupplierViewModelsROOC)
            //    .Select(x => x != null)
            //    //.ObserveOn(RxApp.MainThreadScheduler)
            //    .ToProperty(this, nameof(this.IsCollectionSet), deferSubscription: true)
            //    .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public IObservable<SupplierViewModel> WhenSelectionChanged { get; }
        public IObservable<bool> WhenHasSelectionChanged { get; }

        private ReadOnlyObservableCollection<SupplierViewModel> _supplierViewModels;
        public ReadOnlyObservableCollection<SupplierViewModel> SupplierViewModels
        {
            get { return this._supplierViewModels; }
            private set => this.Set(ref this._supplierViewModels, value);
        }

        private SupplierViewModel _selectedSupplierViewModel;
        public SupplierViewModel SelectedSupplierViewModel
        {
            get => this._selectedSupplierViewModel;
            set => this.Set(ref this._selectedSupplierViewModel, value);
        }

        #endregion

        #region methods

        private void SubscribeToSuppliers()
        {
            //this._suppliersSubscription.Disposable = this._billingService.Suppliers.SuppliersChanges
            this._suppliersSubscription.Disposable = this._billingService.Suppliers.Cache
                .Connect()
                //.RefCount()
                .Transform(
                    supplier => new SupplierViewModel(supplier),
                    new ParallelisationOptions(ParallelType.Parallelise))
                //.OnItemUpdated((current, previous) =>
                //{
                //    // TODO: (better?) alternative: create an observable (selection change, collection.update) => if (needs_handle_update) -> change selection
                //    if (this.SelectedSupplierViewModel != previous || previous == current)
                //        return;

                //    //this._needsReselection = true;
                //    this._previousSelection = this.SelectedSupplierViewModel;

                //    var plan = this.SortedFilteredSupplierViewModelsROOC
                //        .ObserveCollectionChanges()
                //        .Where(x =>
                //           {
                //               var replacedItem = x.EventArgs.OldItems[0];

                //               var z = x.EventArgs.Action == NotifyCollectionChangedAction.Replace
                //               && x.EventArgs.OldItems.Count >= 1
                //               && replacedItem == this._previousSelection;

                //               return z;
                //           })
                //            .Take(1)
                //        .Select(x => x.EventArgs.NewItems.Count >= 1 ? x.EventArgs.NewItems[0] as SupplierViewModel : default)
                //        .And(
                //            this.WhenPropertyChanged(x => x.SelectedSupplierViewModel)
                //            .Take(1)
                //            .Where(x => x.Value == null)
                //            .Select(_ => Unit.Default))
                //        .Then((x, y) =>
                //        {
                //            return x;
                //        });

                //    Observable.When(plan).Take(1).Subscribe(x =>
                //    {
                //        this._previousSelection = null;
                //        this.SelectedSupplierViewModel = x;
                //    });
                //})
                //.DeferUntilLoaded()
                //.Multicast(new ReplaySubject<IChangeSet<SupplierViewModel, int>>())
                //.AutoConnect(1, subscription => this._suppliersSubscription.Disposable = subscription)

                //.ObserveOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                //.SubscribeOn(RxApp.MainThreadScheduler)

                .Bind(out var supplierVMs)
                .DisposeMany()

                //.SubscribeOn(RxApp.TaskpoolScheduler)

                // TODO: .MonitorStatus
                // TODO: delay subscription to OnActivate
                .Subscribe();

            this.SupplierViewModels = supplierVMs;
        }

        public void UnsubscribeFromSuppliers()
        {
            this._suppliersSubscription.Disposable = null;
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this.SubscribeToSuppliers();

            await base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            this.UnsubscribeFromSuppliers();

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public override Task TryCloseAsync(bool? dialogResult = null)
        {
            return base.TryCloseAsync(dialogResult);
        }

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            return base.CanCloseAsync(cancellationToken);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> LoadSuppliers { get; }

        public ReactiveCommand<Unit, Unit> ShowAddSupplierView { get; }
        public ReactiveCommand<SupplierViewModel, Unit> ShowEditSupplierView { get; }
        public ReactiveCommand<SupplierViewModel, Unit> RemoveSupplier { get; }

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