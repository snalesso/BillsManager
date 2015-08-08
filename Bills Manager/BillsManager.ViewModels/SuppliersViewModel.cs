using BillsManager.Localization;
using BillsManager.Models;
using BillsManager.Services.Data;
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
        IHandle<AvailableSuppliersRequest>,
        IHandle<FilterMessage<SupplierDetailsViewModel>>,
        IHandle<AddNewSupplierOrder>,
        IHandle<SupplierNameRequest>,
        IHandle<EditSupplierOrder>,
        IHandle<FilterMessage<BillDetailsViewModel>>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly ISuppliersProvider suppliersProvider;

        private readonly Func<Supplier, SupplierAddEditViewModel> supplierAddEditViewModelFactory;
        private readonly Func<Supplier, SupplierDetailsViewModel> supplierDetailsViewModelFactory;

        private readonly IComparer<SupplierDetailsViewModel> supplierDetailsVMComparer = new SupplierDetailsViewModelComparer();

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
            this.globalEventAggregator = globalEventAggregator;

            // FACTORIES
            this.supplierAddEditViewModelFactory = supplierAddEditViewModelFactory;
            this.supplierDetailsViewModelFactory = supplierDetailsViewModelFactory;

            // SUBSCRIPTIONS
            this.globalEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
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
            set
            {
                if (this.supplierViewModels != value)
                {
                    this.supplierViewModels = value;
                    this.NotifyOfPropertyChange(() => this.SupplierViewModels);
                    this.FilteredSupplierViewModels =
                        new ReadOnlyObservableCollectionEx<SupplierDetailsViewModel>(
                            this.SupplierViewModels,
                            this.Filters != null ? this.Filters.Select(f => f.Execute) : null,
                            this.supplierDetailsVMComparer);

                    // IDEA: #IF !DEBUG ?
                    if (!Execute.InDesignMode)
                        this.globalEventAggregator.PublishOnUIThread(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier).ToList()));
                }
            }
        }

        private ReadOnlyObservableCollectionEx<SupplierDetailsViewModel> filteredSupplierViewModels;
        public ReadOnlyObservableCollectionEx<SupplierDetailsViewModel> FilteredSupplierViewModels
        {
            get { return this.filteredSupplierViewModels; }
            private set
            {
                // TODO: remove set / make lazy get?

                if (this.filteredSupplierViewModels == value) return;

                this.filteredSupplierViewModels = value;
                this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);
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
                    this.NotifyOfPropertyChange();

                    var newSelSupp = value != null ? value.ExposedSupplier : null;
                    this.globalEventAggregator.PublishOnUIThread(new SelectedSupplierChangedMessage(newSelSupp));
                }
            }
        }

        private IEnumerable<Filter<SupplierDetailsViewModel>> filters;
        public IEnumerable<Filter<SupplierDetailsViewModel>> Filters
        {
            get { return this.filters; }
            private set
            {
                if (this.filters != value)
                {
                    this.filters = value;
                    this.NotifyOfPropertyChange();
                    this.UpdateFilters();
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
        }

        #region CRUD

        private void AddSupplier()
        {
            {
                var addSupplierVM = this.supplierAddEditViewModelFactory.Invoke(new Supplier(this.suppliersProvider.GetLastSupplierID() + 1));

            tryAdd: // TODO: optimize
                if (this.windowManager.ShowDialog(addSupplierVM) == true)
                {
                    if (this.suppliersProvider.Add(addSupplierVM.ExposedSupplier))
                    {
                        var newSupplierDetailsVM = this.supplierDetailsViewModelFactory.Invoke(addSupplierVM.ExposedSupplier);

                        this.SupplierViewModels.Add(newSupplierDetailsVM);

                        this.SelectedSupplierViewModel = newSupplierDetailsVM;

                        this.globalEventAggregator.PublishOnUIThread(new AddedMessage<Supplier>(addSupplierVM.ExposedSupplier));
                    }
                    else // TODO: reshow the dialog with this suppVM
                    {
                        var dialog =
                            DialogViewModel
                            .Show(
                                DialogType.Error,
                                TranslationManager.Instance.Translate("AddSupplierFailed").ToString(),
                                TranslationManager.Instance.Translate("AddSupplierFailedMessage").ToString() +
                                Environment.NewLine +
                                TranslationManager.Instance.Translate("TryAgain").ToString())
                            .Ok();

                        this.windowManager.ShowDialog(dialog);

                        goto tryAdd;
                    }
                }
            }
        }

        private void EditSupplier(Supplier supplier)
        {
            Supplier oldSupplier = (Supplier)supplier.Clone();
            var editSupplierVM = this.supplierAddEditViewModelFactory.Invoke(supplier);

            editSupplierVM.BeginEdit();

            if (this.windowManager.ShowDialog(editSupplierVM) == true)
            {
                if (this.suppliersProvider.Edit(editSupplierVM.ExposedSupplier))
                {
                    // TODO: move to this.EditSupplier
                    var editedSupplierDetailsVM = this.SupplierViewModels.FirstOrDefault(svm => svm.ExposedSupplier == supplier);
                    editedSupplierDetailsVM.Refresh();

                    this.SelectedSupplierViewModel = null;

                    this.globalEventAggregator.PublishOnUIThread(new EditedMessage<Supplier>(oldSupplier, editSupplierVM.ExposedSupplier));
                }
                else
                {
                    // URGENT: dont exit if changes are not saved
                    var dialog =
                        DialogViewModel.Show(
                            DialogType.Error,
                            TranslationManager.Instance.Translate("EditFailed").ToString(),
                            TranslationManager.Instance.Translate("EditFailedMessage").ToString())
                        .Ok();

                    this.windowManager.ShowDialog(dialog);
                }
            }
        }

        private void DeleteSupplier(Supplier supplier)
        {
            DialogViewModel deleteSupplierQuestion =
                DialogViewModel.Show(
                    DialogType.Question,
                    TranslationManager.Instance.Translate("DeleteSupplier").ToString(),
                    TranslationManager.Instance.Translate("DeleteSupplierQuestion").ToString() +
                    Environment.NewLine +
                    Environment.NewLine +
                    this.GetSupplierSummary(supplier))
                .YesNo();

            this.windowManager.ShowDialog(deleteSupplierQuestion);

            if (deleteSupplierQuestion.FinalResponse == ResponseType.Yes)
            {
                if (this.suppliersProvider.Delete(supplier))
                {
                    if (this.SelectedSupplierViewModel.ExposedSupplier == supplier)
                        this.SelectedSupplierViewModel = null;

                    this.SupplierViewModels.Remove(this.SupplierViewModels.FirstOrDefault(svm => svm.ExposedSupplier == supplier));

                    this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);
                    this.globalEventAggregator.PublishOnUIThread(new DeletedMessage<Supplier>(supplier));

                    //this.globalEventAggregator.PublishOnUIThread(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier).ToList()));
                }
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
            this.globalEventAggregator.PublishOnUIThread(new ShowSuppliersBillsOrder(supplier));
        }

        private void CopyEMail(SupplierDetailsViewModel supplierVM)
        {
            Clipboard.SetText(supplierVM.eMail);
        }

        #endregion

        #region message handlers

        public void Handle(AvailableSuppliersRequest message)
        {
            message.AcquireSuppliersAction(this.SupplierViewModels.Select(svm => svm.ExposedSupplier));
        }

        public void Handle(FilterMessage<SupplierDetailsViewModel> message)
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

        public void Handle(SupplierNameRequest message)
        {
            message.GiveSupplier(this.SupplierViewModels.FirstOrDefault(svm => svm.ID == message.SupplierID).ExposedSupplier.Name);
        }

        public void Handle(EditSupplierOrder message)
        {
            this.EditSupplier(message.Supplier);
        }

        public void Handle(FilterMessage<BillDetailsViewModel> message)
        {
            // TODO: when bills are filtered by a supplier, select that supplier
        }

        #endregion

        #region support
        private void UpdateFilters()
        {
            this.FilteredSupplierViewModels.Filters = (this.Filters != null ? this.Filters.Select(f => f.Execute) : null);
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand addNewSupplierCommand;
        public RelayCommand AddNewSupplierCommand
        {
            get
            {
                return this.addNewSupplierCommand ?? (this.addNewSupplierCommand =
                    new RelayCommand(
                        () => this.AddSupplier()));
            }
        }

        private RelayCommand<SupplierDetailsViewModel> editSupplierCommand;
        public RelayCommand<SupplierDetailsViewModel> EditSupplierCommand
        {
            get
            {
                return this.editSupplierCommand ?? (this.editSupplierCommand =
                    new RelayCommand<SupplierDetailsViewModel>(
                        p => this.EditSupplier(p.ExposedSupplier),
                        p => p != null));
            }
        }

        private RelayCommand<SupplierDetailsViewModel> deleteSupplierCommand;
        public RelayCommand<SupplierDetailsViewModel> DeleteSupplierCommand
        {
            get
            {
                return this.deleteSupplierCommand ?? (this.deleteSupplierCommand =
                    new RelayCommand<SupplierDetailsViewModel>(
                        p => this.DeleteSupplier(p.ExposedSupplier),
                        p => p != null));
            }
        }

        private RelayCommand<SupplierDetailsViewModel> showSuppliersBillsCommand;
        public RelayCommand<SupplierDetailsViewModel> ShowSuppliersBillsCommand
        {
            get
            {
                return this.showSuppliersBillsCommand ?? (this.showSuppliersBillsCommand =
                    new RelayCommand<SupplierDetailsViewModel>(
                        p => this.ShowSupplierBills(p.ExposedSupplier),
                        p => p != null));
            }
        }

        private RelayCommand<SupplierDetailsViewModel> showSupplierDetailsCommand;
        public RelayCommand<SupplierDetailsViewModel> ShowSupplierDetailsCommand
        {
            get
            {
                return this.showSupplierDetailsCommand ?? (this.showSupplierDetailsCommand =
                    new RelayCommand<SupplierDetailsViewModel>(
                        p => this.ShowSupplierDetails(p),
                        p => p != null));
            }
        }

        private RelayCommand<SupplierDetailsViewModel> copyEMailCommand;
        public RelayCommand<SupplierDetailsViewModel> CopyEMailCommand
        {
            get
            {
                return this.copyEMailCommand ?? (this.copyEMailCommand =
                    new RelayCommand<SupplierDetailsViewModel>(
                        p => this.CopyEMail(p),
                        p => p != null && !string.IsNullOrWhiteSpace(p.eMail)));
            }
        }

        #endregion
    }
}