using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Billy.Billing.Application;
using Billy.Core.Domain.Billing.Application.DTOs;
//using Billy.Core.Domain.Billing.Application.DTOs;
using Billy.Domain.Billing.Models;
using Billy.Domain.Billing.Persistence;
using Billy.UI.Wpf.Services;
using Caliburn.Micro;
using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Alias;
using DynamicData.Annotations;
using DynamicData.Binding;
using DynamicData.Cache;
using DynamicData.Cache.Internal;
using DynamicData.Diagnostics;
using DynamicData.Experimental;
using DynamicData.Kernel;
using DynamicData.List;
using DynamicData.Operators;
using DynamicData.PLinq;
using ReactiveUI;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class SuppliersViewModel : Screen, IDisposable
    {
        #region constants & fields

        private readonly IDialogService _dialogService;
        private readonly IBillingService _billingService;

        //private readonly ISuppliersRepository _suppliersRepository;
        //private readonly Func<IBillingUnitOfWork> _billingUowFactoryMethod;
        private readonly Func<AddSupplierViewModel> _addSupplierViewModelFactoryMethod;
        private readonly Func<SupplierDTO, EditSupplierViewModel> _editSupplierViewModelFactoryMethod;

        private readonly SerialDisposable _suppliersSubscription;

        #endregion

        #region ctors

        public SuppliersViewModel(
            IDialogService dialogService
            //, ISuppliersRepository suppliersRepository
            //, Func<IBillingUnitOfWork> billingUowFactoryMethod
            , IBillingService billingService
            , Func<AddSupplierViewModel> addSupplierViewModelFactoryMethod
            , Func<SupplierDTO, EditSupplierViewModel> editSupplierViewModelFactoryMethod
            )
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._billingService = billingService ?? throw new ArgumentNullException(nameof(billingService));
            this._addSupplierViewModelFactoryMethod = addSupplierViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(addSupplierViewModelFactoryMethod));
            this._editSupplierViewModelFactoryMethod = editSupplierViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editSupplierViewModelFactoryMethod));

            this._suppliersSubscription = new SerialDisposable().DisposeWith(this._disposables);

            this.WhenSelectionChanged = Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => this.PropertyChanged += h,
                    h => this.PropertyChanged -= h)
                .Where(x => x.EventArgs.PropertyName == nameof(this.SelectedSupplierViewModel))
                .Select(_ => this.SelectedSupplierViewModel)
                .StartWith(this.SelectedSupplierViewModel);

            this.ShowAddSupplierView = ReactiveCommand.CreateFromTask(
                () => this._dialogService.ShowDialogAsync(this._addSupplierViewModelFactoryMethod()));
            this.ShowAddSupplierView.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.ShowAddSupplierView.DisposeWith(this._disposables);

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
                    var wasRemoved = await this._billingService.Suppliers.RemoveAsync(supplierViewModel.Id);
                }
                , this.WhenSelectionChanged.Select(x => x != null));
            this.RemoveSupplier.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.RemoveSupplier.DisposeWith(this._disposables);

            this.ShowEditSupplierView = ReactiveCommand.CreateFromTask(
                 async (SupplierViewModel supplierViewModel) =>
                 {
                     if (supplierViewModel == null)
                     {
                         // TODO: throw or log, cause this shouldnt happen
                         return;
                     }

                     await this._dialogService.ShowDialogAsync(this._editSupplierViewModelFactoryMethod.Invoke(supplierViewModel.SupplierDto));
                 }
                 , this.WhenSelectionChanged.Select(x => x != null));
            this.ShowEditSupplierView.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.ShowEditSupplierView.DisposeWith(this._disposables);

            this._billingService.Suppliers.SuppliersChanges
                .RefCount()
                .Transform(supplier => new SupplierViewModel(supplier), new ParallelisationOptions(ParallelType.Parallelise))
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
                .DisposeMany()
                .Multicast(new ReplaySubject<IChangeSet<SupplierViewModel, int>>())
                .AutoConnect(1, subscription => this._suppliersSubscription.Disposable = subscription)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this._sortedFilteredSupplierViewModelsROOC)
                // TODO: delay subscription to OnActivate
                .Subscribe();

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
        }

        #endregion

        #region properties

        public IObservable<SupplierViewModel> WhenSelectionChanged { get; }

        private readonly ReadOnlyObservableCollection<SupplierViewModel> _sortedFilteredSupplierViewModelsROOC;
        public ReadOnlyObservableCollection<SupplierViewModel> SortedFilteredSupplierViewModelsROOC
        {
            get { return this._sortedFilteredSupplierViewModelsROOC; }
            //private set => this.Set(ref this._sortedFilteredSupplierViewModelsROOC, value);
        }

        private SupplierViewModel _previousSelection = null;
        private bool _needsReselection = false;
        private SupplierViewModel _selectedSupplierViewModel;
        public SupplierViewModel SelectedSupplierViewModel
        {
            get => this._selectedSupplierViewModel;
            set
            {
                this.Set(ref this._selectedSupplierViewModel, value);
            }

        }

        #endregion

        #region methods

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