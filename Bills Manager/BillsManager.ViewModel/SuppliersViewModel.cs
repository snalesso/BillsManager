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
        IHandle<AddNewSupplierRequestMessage>,
        IHandle<AskForSupplierMessage>,
        IHandle<EditSupplierRequestMessage>
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

        private ObservableCollection<SupplierDetailsViewModel> supplierViewModels;
        public ObservableCollection<SupplierDetailsViewModel> SupplierViewModels
        {
            get { return this.supplierViewModels; }
            protected set
            {
                if (this.supplierViewModels != value)
                {
                    this.supplierViewModels = value;
                    this.NotifyOfPropertyChange(() => this.SupplierViewModels);

                    if (!Execute.InDesignMode)
                        this.eventAggregator.Publish(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier)));
                }
            }
        }

        public ReadOnlyObservableCollection<SupplierDetailsViewModel> FilteredSupplierViewModels
        {
            get
            {
                if (this.Filters != null)

                    return new ReadOnlyObservableCollection<SupplierDetailsViewModel>(
                        new ObservableCollection<SupplierDetailsViewModel>(
                            this.SupplierViewModels.Where(this.Filters)));

                else
                    return new ReadOnlyObservableCollection<SupplierDetailsViewModel>(this.SupplierViewModels);
            }
        }

        private SupplierDetailsViewModel selectedSupplierViewModel;
        public SupplierDetailsViewModel SelectedSupplierViewModel
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

        private IEnumerable<Func<SupplierDetailsViewModel, bool>> filters;
        public IEnumerable<Func<SupplierDetailsViewModel, bool>> Filters
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
            this.SupplierViewModels = new ObservableCollection<SupplierDetailsViewModel>(
                this.suppliersProvider.GetAll()
                .Select(s => new SupplierDetailsViewModel(s, this.windowManager, this.eventAggregator)));
        }

        private string GetSupplierSummary(Supplier supplier)
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

        #region CRUD

        private void AddSupplier()
        {
            {
                var newVM = new SupplierAddEditViewModel(new Supplier(this.suppliersProvider.GetLastID() + 1), this.windowManager, this.eventAggregator);

                tryAdd:
                if (this.windowManager.ShowDialog(newVM, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
                {
                    if (this.suppliersProvider.Add(newVM.ExposedSupplier))
                    {
                        this.SupplierViewModels.Add(new SupplierDetailsViewModel(newVM.ExposedSupplier, this.windowManager, this.eventAggregator));

                        this.eventAggregator.Publish(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier)));
                    }
                    else // TODO: reshow the dialog with this suppVM
                    {
                        this.windowManager.ShowDialog(new DialogViewModel("Adding error", "Couldn't save the new supplier. Please try again."), settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });
                        goto tryAdd;
                    }
                }
            }
        }

        private void EditSupplier(Supplier supplier)
        {
            Supplier oldSupplier = (Supplier)supplier.Clone();
            var editVM = new SupplierAddEditViewModel(supplier, this.windowManager, this.eventAggregator);

            editVM.BeginEdit();

            if (this.windowManager.ShowDialog(editVM, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
            {
                if (this.suppliersProvider.Edit(editVM.ExposedSupplier))
                {
                    // TODO: move to this.EditSupplier
                    this.SupplierViewModels.Single(svm => svm.ExposedSupplier == supplier).Refresh();
                    this.eventAggregator.Publish(new SupplierEditedMessage(editVM.ExposedSupplier, oldSupplier));
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

        private void DeleteSupplier(Supplier supplier)
        {
            var question = new DialogViewModel(
                "Deleting supplier",
                "Do you really want to DELETE this supplier?" +
                Environment.NewLine +
                Environment.NewLine +
                this.GetSupplierSummary(supplier)
                ,
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"),
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

            if (question.Response == ResponseType.Yes)
            {
                this.suppliersProvider.Delete(supplier);
                this.SupplierViewModels.Remove(this.SupplierViewModels.Single(svm => svm.ExposedSupplier == supplier));

                this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                this.SelectedSupplierViewModel = null;
                this.eventAggregator.Publish(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier)));
                this.eventAggregator.Publish(new SupplierDeletedMessage(supplier));
            }
        }

        private void ShowSupplierDetails(SupplierDetailsViewModel supplierViewModel)
        {
            this.windowManager.ShowDialog(
                supplierViewModel,
                settings: new Dictionary<String, object> { { "ResizeMode", ResizeMode.NoResize }, { "IsCloseButtonVisible", false } });
        }

        #endregion

        private void ShowSupplierBills(Supplier supplier)
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

        public void Handle(AskForSupplierMessage message)
        {
            message.GiveSupplier(this.SupplierViewModels.Single(svm => svm.ID == message.SupplierID).ExposedSupplier);
        }

        public void Handle(EditSupplierRequestMessage message)
        {
            this.EditSupplier(message.Supplier);
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

        private RelayCommand<SupplierDetailsViewModel> editSupplierCommand;
        public RelayCommand<SupplierDetailsViewModel> EditSupplierCommand
        {
            get
            {
                if (this.editSupplierCommand == null)
                    this.editSupplierCommand = new RelayCommand<SupplierDetailsViewModel>(
                        p => this.EditSupplier(p.ExposedSupplier),
                        p => p != null);

                return this.editSupplierCommand;
            }
        }

        private RelayCommand<SupplierDetailsViewModel> deleteSupplierCommand;
        public RelayCommand<SupplierDetailsViewModel> DeleteSupplierCommand
        {
            get
            {
                if (this.deleteSupplierCommand == null)
                    this.deleteSupplierCommand = new RelayCommand<SupplierDetailsViewModel>(
                        p => this.DeleteSupplier(p.ExposedSupplier),
                        p => p != null);

                return this.deleteSupplierCommand;
            }
        }

        private RelayCommand<SupplierDetailsViewModel> showSuppliersBillsCommand;
        public RelayCommand<SupplierDetailsViewModel> ShowSuppliersBillsCommand
        {
            get
            {
                if (this.showSuppliersBillsCommand == null)
                    this.showSuppliersBillsCommand = new RelayCommand<SupplierDetailsViewModel>(
                        p => this.ShowSupplierBills(p.ExposedSupplier),
                        p => p != null);

                return this.showSuppliersBillsCommand;
            }
        }

        private RelayCommand<SupplierDetailsViewModel> showSupplierDetailsCommand;
        public RelayCommand<SupplierDetailsViewModel> ShowSupplierDetailsCommand
        {
            get
            {
                if (this.showSupplierDetailsCommand == null)
                    this.showSupplierDetailsCommand = new RelayCommand<SupplierDetailsViewModel>(
                        p => this.ShowSupplierDetails(p),
                        p => p != null);

                return this.showSupplierDetailsCommand;
            }
        }

        #endregion
    }
}