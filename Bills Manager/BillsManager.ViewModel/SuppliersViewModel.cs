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
        IHandle<AvailableSuppliersRequest>,
        IHandle<FilterMessage<SupplierDetailsViewModel>>,
        IHandle<AddNewSupplierOrder>,
        IHandle<SupplierNameRequest>,
        IHandle<EditSupplierOrder>
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
                    this.NotifyOfPropertyChange();
                    this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                    if (!Execute.InDesignMode)
                        this.dbEventAggregator.PublishOnUIThread(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier).ToList()));
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
                    this.NotifyOfPropertyChange();

                    var newSelSupp = value != null ? value.ExposedSupplier : null;
                    this.dbEventAggregator.PublishOnUIThread(new SelectedSupplierChagedMessage(newSelSupp));
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
                    this.NotifyOfPropertyChange();
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

                        this.dbEventAggregator.PublishOnUIThread(new AddedMessage<Supplier>(newSvm.ExposedSupplier));
                    }
                    else // TODO: reshow the dialog with this suppVM
                    {
                        DialogViewModel dialog =
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
            //var editVM = new SupplierAddEditViewModel(this.windowManager, this.dbEventAggregator, supplier);
            var editVM = this.supplierAddEditViewModelFactory.Invoke(supplier);

            editVM.BeginEdit();

            if (this.windowManager.ShowDialog(editVM) == true)
            {
                if (this.suppliersProvider.Edit(editVM.ExposedSupplier))
                {
                    // TODO: move to this.EditSupplier
                    var editedSuppVM = this.SupplierViewModels.FirstOrDefault(svm => svm.ExposedSupplier == supplier);
                    var wasSelected = this.SelectedSupplierViewModel == editedSuppVM;
                    this.SupplierViewModels.SortEdited(editedSuppVM);

                    if (wasSelected) // TODO: not working
                        this.SelectedSupplierViewModel = editedSuppVM;

                    this.dbEventAggregator.PublishOnUIThread(new EditedMessage<Supplier>(editVM.ExposedSupplier, oldSupplier));
                }
                else
                {
                    // URGENT: dont exit if changes are not saved
                    DialogViewModel dialog =
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
                this.suppliersProvider.Delete(supplier);
                this.SupplierViewModels.Remove(this.SupplierViewModels.FirstOrDefault(svm => svm.ExposedSupplier == supplier));

                this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                this.SelectedSupplierViewModel = null;
                this.dbEventAggregator.PublishOnUIThread(new SuppliersListChangedMessage(this.SupplierViewModels.Select(svm => svm.ExposedSupplier).ToList()));
                this.dbEventAggregator.PublishOnUIThread(new DeletedMessage<Supplier>(supplier));
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
            this.dbEventAggregator.PublishOnUIThread(new ShowSuppliersBillsOrder(supplier));
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