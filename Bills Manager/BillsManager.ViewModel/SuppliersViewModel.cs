using BillsManager.Localization;
using BillsManager.Models;
using BillsManager.Services.Providers;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BillsManager.ViewModels
{
    public sealed partial class SuppliersViewModel :
        Screen,
        IHandle<AvailableSuppliersRequestMessage>,
        IHandle<SuppliersFilterMessage>,
        IHandle<AddNewSupplierOrder>,
        IHandle<SupplierNameRequestMessage>,
        IHandle<EditSupplierRequestMessage>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator dbEventAggregator;
        private readonly ISuppliersProvider suppliersProvider;

        private readonly Func<Supplier, SupplierAddEditViewModel> supplierAddEditViewModelFactory;
        private readonly Func<Supplier, SupplierDetailsViewModel> supplierDetailsViewModelFactory;

        #endregion

        #region ctor

        public SuppliersViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            ISuppliersProvider suppliersProvider,
            Func<Supplier, SupplierAddEditViewModel> supplierAddEditViewModelFactory,
            Func<Supplier, SupplierDetailsViewModel> supplierDetailsViewModelFactory)
        {
            // SERVICES
            this.suppliersProvider = suppliersProvider;
            this.windowManager = windowManager;
            this.dbEventAggregator = globalEventAggregator;

            // FACTORIES
            this.supplierAddEditViewModelFactory = supplierAddEditViewModelFactory;
            this.supplierDetailsViewModelFactory = supplierDetailsViewModelFactory;

            // SUBSCRIPTIONS
            this.dbEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.dbEventAggregator.Unsubscribe(this);
                };

            // UI
            this.DisplayName = @"Suppliers";

            // START
            this.LoadSuppliers();
        }

        #endregion

        #region properties

        private ObservableCollection<SupplierDetailsViewModel> supplierViewModels;
        public ObservableCollection<SupplierDetailsViewModel> SupplierViewModels
        {
            get { return this.supplierViewModels; }
            private set
            {
                if (this.supplierViewModels != value)
                {
                    this.supplierViewModels = value;
                    this.NotifyOfPropertyChange(() => this.SupplierViewModels);
                    this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                    if (!Execute.InDesignMode)
                        this.dbEventAggregator.Publish(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier)));
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
                            this.SupplierViewModels.Where(this.Filters.Select(filter => filter.Execute))));

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

                    var newSelSupp = value != null ? value.ExposedSupplier : null;
                    this.dbEventAggregator.Publish(new SelectedSupplierChagedMessage(newSelSupp));
                }
            }
        }

        private IEnumerable<Filter<SupplierDetailsViewModel>> filters;
        public IEnumerable<Filter<SupplierDetailsViewModel>> Filters
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

        public void LoadSuppliers()
        {
            var supps = this.suppliersProvider.GetAllSuppliers();

            var suppVMs = supps.Select(supplier => this.supplierDetailsViewModelFactory.Invoke(supplier)).ToList();

            suppVMs.Sort();

            this.SupplierViewModels = new ObservableCollection<SupplierDetailsViewModel>(suppVMs);
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
                //var newVM = new SupplierAddEditViewModel(this.windowManager, this.dbEventAggregator, new Supplier(this.suppliersProvider.GetLastSupplierID() + 1));
                var newSvm = this.supplierAddEditViewModelFactory.Invoke(new Supplier(this.suppliersProvider.GetLastSupplierID() + 1));

            tryAdd: // TODO: optimize
                if (this.windowManager.ShowDialog(newSvm) == true)
                {
                    if (this.suppliersProvider.Add(newSvm.ExposedSupplier))
                    {
                        var newSuppDetVM = this.supplierDetailsViewModelFactory.Invoke(newSvm.ExposedSupplier);

                        this.SupplierViewModels.AddSorted(newSuppDetVM);
                        this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                        this.SelectedSupplierViewModel = newSuppDetVM;

                        this.dbEventAggregator.Publish(new SupplierAddedMessage(newSvm.ExposedSupplier));
                    }
                    else // TODO: reshow the dialog with this suppVM
                    {
                        this.windowManager.ShowDialog(new DialogViewModel(
                            "Add failed", // TODO: language
                            "Couldn't save the new supplier. Please try again."));
                        goto tryAdd;
                    }
                }
            }
        }

        private void EditSupplier(Supplier supplier)
        {
            Supplier oldSupplier = (Supplier)supplier.Clone();
            //var editVM = new SupplierAddEditViewModel(this.windowManager, this.dbEventAggregator, supplier);
            var editVM = this.supplierAddEditViewModelFactory.Invoke(supplier);

            editVM.BeginEdit();

            if (this.windowManager.ShowDialog(editVM) == true)
            {
                if (this.suppliersProvider.Edit(editVM.ExposedSupplier))
                {
                    // TODO: move to this.EditSupplier
                    var editedSuppVM = this.SupplierViewModels.Single(svm => svm.ExposedSupplier == supplier);
                    var wasSelected = this.SelectedSupplierViewModel == editedSuppVM;
                    this.SupplierViewModels.SortEdited(editedSuppVM);

                    if (wasSelected) // TODO: not working
                        this.SelectedSupplierViewModel = editedSuppVM;

                    this.dbEventAggregator.Publish(new SupplierEditedMessage(editVM.ExposedSupplier, oldSupplier));
                }
                else
                {
                    // URGENT: dont exit if changes are not saved
                    this.windowManager.ShowDialog(new DialogViewModel(
                        TranslationManager.Instance.Translate("EditFailed").ToString(),
                        TranslationManager.Instance.Translate("EditFailedMessage").ToString()));
                }
            }
        }

        private void DeleteSupplier(Supplier supplier)
        {
            var question = new DialogViewModel(
                TranslationManager.Instance.Translate("DeleteSupplier").ToString(),
                TranslationManager.Instance.Translate("DeleteSupplierQuestion").ToString() +
                Environment.NewLine +
                Environment.NewLine +
                this.GetSupplierSummary(supplier),
                new[]
                {
                    new DialogResponse(
                        ResponseType.Yes,
                        TranslationManager.Instance.Translate("Delete").ToString(),
                        TranslationManager.Instance.Translate("Yes").ToString()),
                    new DialogResponse(
                        ResponseType.No, 
                        TranslationManager.Instance.Translate("No").ToString())
                });

            this.windowManager.ShowDialog(question);

            if (question.FinalResponse == ResponseType.Yes)
            {
                this.suppliersProvider.Delete(supplier);
                this.SupplierViewModels.Remove(this.SupplierViewModels.Single(svm => svm.ExposedSupplier == supplier));

                this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                this.SelectedSupplierViewModel = null;
                this.dbEventAggregator.Publish(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier)));
                this.dbEventAggregator.Publish(new SupplierDeletedMessage(supplier));
            }
        }

        private void ShowSupplierDetails(SupplierDetailsViewModel supplierViewModel)
        {
            // TODO: make paramter use only supplier
            // TODO: decide if CanClose of window is enabled on edit views
            this.windowManager.ShowDialog(supplierViewModel, settings: new Dictionary<string, object> { { "CanClose", true } });
        }

        #endregion

        #region extras

        private void ShowSupplierBills(Supplier supplier)
        {
            this.dbEventAggregator.Publish(new ShowSuppliersBillsOrder(supplier));
        }

        private void CopyEMail(SupplierDetailsViewModel supplierVM)
        {
            Clipboard.SetText(supplierVM.eMail);
        }

        #endregion

        #region message handlers

        public void Handle(AvailableSuppliersRequestMessage message)
        {
            message.AcquireSuppliersAction(this.SupplierViewModels.Select(svm => svm.ExposedSupplier));
        }

        public void Handle(SuppliersFilterMessage message)
        {
            this.Filters = message.Filters;

            if (!this.FilteredSupplierViewModels.Contains(this.SelectedSupplierViewModel))
                this.SelectedSupplierViewModel = null;
        }

        public void Handle(AddNewSupplierOrder message)
        {
            //if (this.suppliersProvider.Name == message.DBName)
            //{
            this.AddSupplier();
            //}
        }

        public void Handle(SupplierNameRequestMessage message)
        {
            message.GiveSupplier(this.SupplierViewModels.Single(svm => svm.ID == message.SupplierID).ExposedSupplier.Name);
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

        private RelayCommand<SupplierDetailsViewModel> copyEMailCommand;
        public RelayCommand<SupplierDetailsViewModel> CopyEMailCommand
        {
            get
            {
                if (this.copyEMailCommand == null)
                    this.copyEMailCommand = new RelayCommand<SupplierDetailsViewModel>(
                        p => this.CopyEMail(p),
                        p => p != null && !string.IsNullOrWhiteSpace(p.eMail));

                return this.copyEMailCommand;
            }
        }

        #endregion
    }
}