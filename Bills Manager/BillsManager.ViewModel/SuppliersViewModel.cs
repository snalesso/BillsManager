using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BillsManager.Model;
using BillsManager.Service.Providers;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SuppliersViewModel :
        Screen,
        IHandle<AskForAvailableSuppliersMessage>,
        IHandle<SuppliersNeedRefreshMessage>,
        IHandle<SuppliersFilterMessage>,
        IHandle<AddNewSupplierRequestMessage>
    {
        #region fields

        private readonly ISuppliersProvider suppliersProvider;
        private readonly IWindowManager windowManager;
        //private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public SuppliersViewModel(
            ISuppliersProvider suppliersProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator)
        {
            this.suppliersProvider = suppliersProvider;
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);

            this.LoadSuppliers();
        }

        #endregion

        #region properties

        private ObservableCollection<SupplierViewModel> supplierViewModels;
        public ObservableCollection<SupplierViewModel> SupplierViewModels
        {
            get { return this.supplierViewModels; }
            protected set
            {
                if (this.supplierViewModels != value)
                {
                    this.supplierViewModels = value;
                    this.NotifyOfPropertyChange(() => this.SupplierViewModels);

                    if (!Execute.InDesignMode)
                        this.PublishAvailableSuppliersChanged();
                }
            }
        }

        public ReadOnlyObservableCollection<SupplierViewModel> FilteredSupplierViewModels
        {
            get
            {
                if (this.Filters != null)

                    return new ReadOnlyObservableCollection<SupplierViewModel>(
                        new ObservableCollection<SupplierViewModel>(
                            this.SupplierViewModels.Where(this.Filters)));

                else
                    return new ReadOnlyObservableCollection<SupplierViewModel>(this.SupplierViewModels);
            }
        }

        private SupplierViewModel selectedSupplierViewModel;
        public SupplierViewModel SelectedSupplierViewModel
        {
            get { return this.selectedSupplierViewModel; }
            set
            {
                if (this.selectedSupplierViewModel != value)
                {
                    this.selectedSupplierViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SelectedSupplierViewModel);
                }
            }
        }

        private IEnumerable<Func<SupplierViewModel, bool>> filters;
        public IEnumerable<Func<SupplierViewModel, bool>> Filters
        {
            get { return this.filters; }
            set
            {
                if (this.filters != value)
                {
                    this.filters = value;
                    this.NotifyOfPropertyChange(() => this.Filters);
                    this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);
                }
            }
        }

        #endregion

        #region methods

        private void LoadSuppliers()
        {
            this.SupplierViewModels = new ObservableCollection<SupplierViewModel>(
                this.suppliersProvider.GetAll()
                .Select(s => new SupplierViewModel(s, this.windowManager, this.eventAggregator)));
        }

        private void AddSupplier()
        {
            {
                SupplierViewModel newSvm = new SupplierViewModel(
                    new Supplier(this.suppliersProvider.GetLastID() + 1), this.windowManager, /*this.dialogService,*/ this.eventAggregator);
                
                if (this.windowManager.ShowDialog(newSvm, null, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
                {
                    if (this.suppliersProvider.Add(newSvm.ExposedSupplier))
                    {
                        this.SupplierViewModels.Add(newSvm);

                        this.PublishAvailableSuppliersChanged();
                    }
                    else // TODO: reshow the dialog with this suppVM
                    {
                        throw new OperationCanceledException("Couldn't save the new supplier. Please try again.");
                    }
                }
            }
        }

        private void EditSupplier(SupplierViewModel supplierViewModel)
        {
            Supplier oldSupplier = (Supplier)supplierViewModel.ExposedSupplier.Clone();

            supplierViewModel.BeginEdit();

            if (this.windowManager.ShowDialog(supplierViewModel, null, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
            {
                if (this.suppliersProvider.Edit(supplierViewModel.ExposedSupplier))
                {
                    // TODO: move to this.EditSupplier
                    this.eventAggregator.Publish(new SupplierEditedMessage(supplierViewModel.ExposedSupplier, oldSupplier));
                }
                else
                {
                    this.windowManager.ShowDialog(new DialogViewModel(
                        "Error", // TODO: language
                        "Error during editing."),
                        settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });
                }
            }
        }

        private void DeleteSupplier(SupplierViewModel supplierViewModel)
        {
            var question = new DialogViewModel(
                "Deleting supplier",
                "Do you really want to DELETE this supplier?" +
                Environment.NewLine +
                Environment.NewLine +
                this.GetSupplierInfo(supplierViewModel.ExposedSupplier)
                ,
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"),
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

            if (question.Response == ResponseType.Yes)
            {
                this.suppliersProvider.Delete(supplierViewModel.ExposedSupplier);
                this.SupplierViewModels.Remove(this.SupplierViewModels.Single(svm => svm.ExposedSupplier == supplierViewModel.ExposedSupplier));

                this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                this.SelectedSupplierViewModel = null;
                this.PublishAvailableSuppliersChanged();
                this.eventAggregator.Publish(new SupplierDeletedMessage(supplierViewModel.ExposedSupplier));
            }
        }

        private string GetSupplierInfo(Supplier supplier)
        {
            return
                supplier.Name +
                Environment.NewLine +
                supplier.Street +
                " " + supplier.Number +
                ", " + supplier.Zip +
                " " + supplier.City +
                " (" + supplier.Province + ")" +
                " - " + supplier.Country;
            // TODO: add bills count to supplier's info
        }

        private void PublishAvailableSuppliersChanged()
        {
            this.eventAggregator.Publish(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier)));
        }

        private void ShowSuppliersBills(Supplier supplier)
        {
            this.eventAggregator.Publish(new ActivateBillsSupplierFilterMessage(supplier));
        }

        #region message handlers

        public void Handle(AskForAvailableSuppliersMessage message)
        {
            message.AcquireSuppliersAction(this.SupplierViewModels.Select(svm => svm.ExposedSupplier));
        }

        public void Handle(SuppliersNeedRefreshMessage message)
        {
            this.LoadSuppliers();
        }

        public void Handle(SuppliersFilterMessage message)
        {
            this.Filters = message.Filters;
        }

        public void Handle(AddNewSupplierRequestMessage message)
        {
            this.AddSupplier();
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand addNewSupplierCommand;
        public RelayCommand AddNewSupplierCommand
        {
            get
            {
                if (this.addNewSupplierCommand == null) this.addNewSupplierCommand = new RelayCommand(
                    () => this.AddSupplier());

                return this.addNewSupplierCommand;
            }
        }

        private RelayCommand<SupplierViewModel> editSupplierCommand;
        public RelayCommand<SupplierViewModel> EditSupplierCommand
        {
            get
            {
                if (this.editSupplierCommand == null)
                    this.editSupplierCommand = new RelayCommand<SupplierViewModel>(
                        p => this.EditSupplier(p),
                        p => p != null);

                return this.editSupplierCommand;
            }
        }

        private RelayCommand<SupplierViewModel> deleteSupplierCommand;
        public RelayCommand<SupplierViewModel> DeleteSupplierCommand
        {
            get
            {
                if (this.deleteSupplierCommand == null)
                    this.deleteSupplierCommand = new RelayCommand<SupplierViewModel>(
                        p => this.DeleteSupplier(p),
                        p => p != null);

                return this.deleteSupplierCommand;
            }
        }

        private RelayCommand<SupplierViewModel> showSuppliersBillsCommand;
        public RelayCommand<SupplierViewModel> ShowSuppliersBillsCommand
        {
            get
            {
                if (this.showSuppliersBillsCommand == null)
                    this.showSuppliersBillsCommand = new RelayCommand<SupplierViewModel>(
                        p => this.ShowSuppliersBills(p.ExposedSupplier),
                        p => p != null);

                return this.showSuppliersBillsCommand;
            }
        }

        #endregion
    }
}