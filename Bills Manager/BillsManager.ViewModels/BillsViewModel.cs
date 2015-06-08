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

namespace BillsManager.ViewModels
{
    public sealed partial class BillsViewModel :
        Screen,
        IHandle<FilterMessage<BillDetailsViewModel>>,
        IHandle<DeletedMessage<Supplier>>,
        IHandle<EditBillOrder>,
        IHandle<AddBillToSupplierOrder>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly IBillsProvider billsProvider;

        private readonly Func<IEnumerable<Supplier>, Bill, BillAddEditViewModel> billAddEditViewModelFactory;
        private readonly Func<Bill, BillDetailsViewModel> billDetailsViewModelFactory;

        private readonly IComparer<BillDetailsViewModel> billDetailsVMComparer = new BillDetailsViewModelComparer();

        #endregion

        #region ctor

        public BillsViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            IBillsProvider billsProvider,
            Func<IEnumerable<Supplier>, Bill, BillAddEditViewModel> billAddEditViewModelFactory,
            Func<Bill, BillDetailsViewModel> billDetailsViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;
            this.billsProvider = billsProvider;

            // FACTORIES
            this.billAddEditViewModelFactory = billAddEditViewModelFactory;
            this.billDetailsViewModelFactory = billDetailsViewModelFactory;

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
            this.DisplayName = TranslationManager.Instance.Translate("Bills").ToString();

            // START
            this.LoadBills();
        }

        #endregion

        #region properties

        private ObservableCollection<BillDetailsViewModel> billViewModels;
        private ObservableCollection<BillDetailsViewModel> BillViewModels
        {
            get { return this.billViewModels; }
            set
            {
                if (this.billViewModels != value)
                {
                    this.billViewModels = value;
                    this.NotifyOfPropertyChange(() => this.BillViewModels);
                    this.FilteredBillViewModels =
                        new ReadOnlyObservableCollectionEx<BillDetailsViewModel>(
                            this.BillViewModels,
                            this.Filters != null ? this.Filters.Select(f => f.Execute) : null,
                            this.billDetailsVMComparer);

                    // IDEA: #IF !DEBUG ?
                    if (!Execute.InDesignMode)
                        this.globalEventAggregator.PublishOnUIThread(new BillsListChangedMessage(this.BillViewModels.Select(bvm => bvm.ExposedBill).ToList())); // TODO: move to filtered?
                }
            }

        }

        private ReadOnlyObservableCollectionEx<BillDetailsViewModel> filteredBillViewModels;
        public ReadOnlyObservableCollectionEx<BillDetailsViewModel> FilteredBillViewModels
        {
            get { return this.filteredBillViewModels; }
            private set
            {
                // TODO: remove set / make lazy get?

                if (this.filteredBillViewModels == value) return;

                this.filteredBillViewModels = value;
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
            }
        }

        private BillDetailsViewModel selectedBillViewModel;
        public BillDetailsViewModel SelectedBillViewModel
        {
            get { return this.selectedBillViewModel; }
            set
            {
                if (this.selectedBillViewModel == value) return;

                this.selectedBillViewModel = value;
                this.NotifyOfPropertyChange(() => this.SelectedBillViewModel);

            }
        }

        // IDEA: remove filter class and make it possibile to add or not a comment at print time?
        private IEnumerable<Filter<BillDetailsViewModel>> filters;
        public IEnumerable<Filter<BillDetailsViewModel>> Filters
        {
            get { return this.filters; }
            private set
            {
                if (this.filters == value) return;

                this.filters = value;
                this.NotifyOfPropertyChange(() => this.Filters);
                this.UpdateFilters();
                this.NotifyOfPropertyChange(() => this.FiltersDescription);
            }
        }

        public string FiltersDescription
        {
            get
            {
                var filtersDescription = string.Empty;
                if (this.Filters == null) return TranslationManager.Instance.Translate("AllTheBills").ToString();

                foreach (var filter in this.Filters)
                {
                    filtersDescription += ", " + filter.Description().ToLower(TranslationManager.Instance.CurrentLanguage);
                }

                return TranslationManager.Instance.Translate("Bills").ToString() + filtersDescription.Substring(1);
            }
        }

        #endregion

        #region methods

        public void LoadBills()
        {
            var bills = this.billsProvider.GetAllBills();

            var billVMs = bills.Select(bill => this.billDetailsViewModelFactory.Invoke(bill)).ToList();

            billVMs.Sort();

            this.BillViewModels = new ObservableCollection<BillDetailsViewModel>(billVMs);
        }

        private string GetBillSummary(Bill bill)
        {
            return
                this.GetSupplierName(bill.SupplierID) +
                Environment.NewLine +
                bill.Code +
                Environment.NewLine +
                string.Format(TranslationManager.Instance.Translate("_currency_format").ToString(), bill.Amount);
        }

        private string GetSupplierName(uint supplierID)
        {
            string supp = null;
            this.globalEventAggregator.PublishOnUIThread(new SupplierNameRequest(supplierID, s => supp = s));
            return supp;
        }

        #region CRUD

        private void AddBill(Supplier supplier = null)
        {
            var supps = this.GetAvailableSuppliers();
            var addBillVm = this.billAddEditViewModelFactory.Invoke(supps, new Bill(this.billsProvider.GetLastBillID() + 1));

            if (supplier != null) // TODO: safe operation? if supplier is not known by addbvm?
                addBillVm.SelectedSupplier = supplier;
tryAdd:
            if (this.windowManager.ShowDialog(addBillVm) == true)
            {
                // TODO: make it possible to show the view through a dialogviewmodel (evaluate the idea)
                if (this.billsProvider.Add(addBillVm.ExposedBill))
                {
                    var newBillDetailsVm = this.billDetailsViewModelFactory.Invoke(addBillVm.ExposedBill);

                    this.BillViewModels.Add(newBillDetailsVm);

                    this.SelectedBillViewModel = newBillDetailsVm;

                    this.globalEventAggregator.PublishOnUIThread(new AddedMessage<Bill>(newBillDetailsVm.ExposedBill));
                }
                else
                {
                    this.windowManager.ShowDialog(
                        DialogViewModel.Show(
                            DialogType.Error,
                            TranslationManager.Instance.Translate("AddBillFailed"),
                            TranslationManager.Instance.Translate("AddBillFailedMessage") +
                            Environment.NewLine +
                            TranslationManager.Instance.Translate("TryAgain"))
                        .Ok());

                    goto tryAdd;
                }
            }
        }

        private void EditBill(Bill bill)
        {
            var supps = this.GetAvailableSuppliers();
            var baeVM = this.billAddEditViewModelFactory.Invoke(supps, bill);

            Bill oldVersion = (Bill)bill.Clone();

            baeVM.BeginEdit();

            if (this.windowManager.ShowDialog(baeVM) == true)
            {
                if (this.billsProvider.Edit(baeVM.ExposedBill)) // URGENT: if the DB action fails, changes have to be rolled back!!
                {
                    var editedBillVM = this.BillViewModels.FirstOrDefault(bvm => bvm.ExposedBill == bill);
                    editedBillVM.Refresh();

                    this.SelectedBillViewModel = null;

                    this.globalEventAggregator.PublishOnUIThread(new EditedMessage<Bill>(oldVersion, baeVM.ExposedBill)); // TODO: move to confirm add edit command in addeditbvm
                }
                else
                {
                    this.windowManager.ShowDialog(
                        DialogViewModel.Show(
                            DialogType.Error,
                            TranslationManager.Instance.Translate("EditFailed"),
                            TranslationManager.Instance.Translate("EditFailedMessage") +
                            Environment.NewLine +
                            TranslationManager.Instance.Translate("TryAgain"))
                        .Ok());
                }
            }
        }

        private void DeleteBill(Bill bill)
        {
            DialogViewModel deleteBillConfirmationDialog =
                DialogViewModel.Show(
                    DialogType.Question,
                    TranslationManager.Instance.Translate("DeleteBill"),
                    TranslationManager.Instance.Translate("DeleteBillQuestion") +
                    Environment.NewLine +
                    Environment.NewLine +
                    this.GetBillSummary(bill))
                .YesNo(TranslationManager.Instance.Translate("Delete"));

            this.windowManager.ShowDialog(deleteBillConfirmationDialog);

            if (deleteBillConfirmationDialog.FinalResponse == ResponseType.Yes)
            {
                if (this.billsProvider.Delete(bill))
                {
                    this.BillViewModels.Remove(this.BillViewModels.FirstOrDefault(bvm => bvm.ExposedBill == bill));
                    //this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                    this.SelectedBillViewModel = null;

                    this.globalEventAggregator.PublishOnUIThread(new DeletedMessage<Bill>(bill));
                }
            }
        }

        private void PayBill(Bill bill)
        {
            DialogViewModel payBillConfirmationDialog =
                DialogViewModel.Show(
                    DialogType.Question,
                    TranslationManager.Instance.Translate("PayBill"),
                    TranslationManager.Instance.Translate("PayBillQuestion") +
                    Environment.NewLine +
                    Environment.NewLine +
                    this.GetBillSummary(bill))
                .YesNo(
                    TranslationManager.Instance.Translate("PayBill"));

            if (this.windowManager.ShowDialog(payBillConfirmationDialog) == true)
            {
                Bill oldVersion = (Bill)bill.Clone();

                bill.PaymentDate = DateTime.Today;

                if (this.billsProvider.Edit(bill))
                {
                    var editedVM = this.BillViewModels.FirstOrDefault(bvm => bvm.ExposedBill == bill);
                    editedVM.Refresh();

                    this.SelectedBillViewModel = null;

                    this.globalEventAggregator.PublishOnUIThread(new EditedMessage<Bill>(oldVersion, bill));
                }
            }
        }

        private void ShowBillDetails(BillDetailsViewModel billDetailsViewModel)
        {
            this.windowManager.ShowDialog(billDetailsViewModel, settings: new Dictionary<string, object>() { { "CanClose", true } });
        }

        #endregion

        #region message handlers

        public void Handle(FilterMessage<BillDetailsViewModel> message)
        {
            this.Filters = message.Filters;

            if (!this.FilteredBillViewModels.Contains(this.SelectedBillViewModel))
                this.SelectedBillViewModel = null;
        }

        public void Handle(DeletedMessage<Supplier> message)
        {
            var bvmsToDelete = this.BillViewModels.Where(bvm => bvm.SupplierID == message.DeletedItem.ID).ToArray();

            foreach (var bvm in bvmsToDelete)
            {
                if (this.billsProvider.Delete(bvm.ExposedBill))
                    this.BillViewModels.Remove(bvm);
                else
                    throw new ApplicationException("Couldn't delete this bill: " + Environment.NewLine + Environment.NewLine + this.GetBillSummary(bvm.ExposedBill));
            }

            // if there was an error during the romve operation on the db
            // this point wouldn't be reached
            //if (this.Filters == null)
            //    this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
        }

        public void Handle(EditBillOrder message)
        {
            this.EditBill(message.Bill); /* TODO: is it better to pass only the ID and let this VM to get the bill?
                                          * (this would check whether the bill is contained or is a lost one) */
        }

        public void Handle(AddBillToSupplierOrder message)
        {
            this.AddBill(message.Supplier);
        }

        #endregion

        #region overrides

        protected override void OnActivate()
        {
            base.OnActivate();
            //this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region support

        private void UpdateFilters()
        {
            this.FilteredBillViewModels.Filters = (this.Filters != null ? this.Filters.Select(f => f.Execute) : null);
        }

        private IEnumerable<Supplier> GetAvailableSuppliers()
        {
            IEnumerable<Supplier> supps = Enumerable.Empty<Supplier>();
            this.globalEventAggregator.PublishOnUIThread(new AvailableSuppliersRequest((s) => supps = s));
            return supps;
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand addNewBillCommand;
        public RelayCommand AddNewBillCommand
        {
            get
            {
                return this.addNewBillCommand ?? (this.addNewBillCommand = 
                    new RelayCommand(
                        () => this.AddBill()));
            }
        }

        private RelayCommand<BillDetailsViewModel> editBillCommand;
        public RelayCommand<BillDetailsViewModel> EditBillCommand
        {
            get
            {
                return this.editBillCommand ?? (this.editBillCommand = 
                    new RelayCommand<BillDetailsViewModel>(
                        p => this.EditBill(p.ExposedBill),
                        p => p != null));
            }
        }

        private RelayCommand<BillDetailsViewModel> deleteBillCommand;
        public RelayCommand<BillDetailsViewModel> DeleteBillCommand
        {
            get
            {
                return this.deleteBillCommand ?? (this.deleteBillCommand = 
                    new RelayCommand<BillDetailsViewModel>(
                        p => this.DeleteBill(p.ExposedBill),
                        p => p != null));
            }
        }

        private RelayCommand<BillDetailsViewModel> payBillCommand;
        public RelayCommand<BillDetailsViewModel> PayBillCommand
        {
            get
            {
                return this.payBillCommand ?? (this.payBillCommand = 
                    new RelayCommand<BillDetailsViewModel>(
                        p => this.PayBill(p.ExposedBill),
                        p => p != null && !p.IsPaid));
            }
        }

        private RelayCommand<BillDetailsViewModel> showBillDetailsCommand;
        public RelayCommand<BillDetailsViewModel> ShowBillDetailsCommand
        {
            get
            {
                return this.showBillDetailsCommand ?? (this.showBillDetailsCommand = 
                    new RelayCommand<BillDetailsViewModel>(
                        p => this.ShowBillDetails(p),
                        p => p != null));
            }
        }

        #endregion
    }
}