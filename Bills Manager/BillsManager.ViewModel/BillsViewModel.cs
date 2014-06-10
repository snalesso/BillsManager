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
        IHandle<BillsFilterMessage>,
        IHandle<SupplierDeletedMessage>,
        IHandle<EditBillRequestMessage>,
        IHandle<AddBillToSupplierOrder>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly IBillsProvider billsProvider;

        private readonly Func<IEnumerable<Supplier>, Bill, BillAddEditViewModel> billAddEditViewModelFactory;
        private readonly Func<Bill, BillDetailsViewModel> billDetailsViewModelFactory;

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
            this.globalEventAggregator.Subscribe(this); // TODO: move to activate?

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
        public ObservableCollection<BillDetailsViewModel> BillViewModels
        {
            get { return this.billViewModels; }
            private set
            {
                if (this.billViewModels != value)
                {
                    this.billViewModels = value;
                    this.NotifyOfPropertyChange(() => this.BillViewModels);
                    this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                    if (!Execute.InDesignMode)
                        this.globalEventAggregator.Publish(new BillsListChangedMessage(this.BillViewModels.Select(bvm => bvm.ExposedBill)));
                }
            }

        }

        public ReadOnlyObservableCollection<BillDetailsViewModel> FilteredBillViewModels
        {
            get
            {
                if (this.Filters != null)

                    return new ReadOnlyObservableCollection<BillDetailsViewModel>( // TODO: check whether this is a good practice
                        new ObservableCollection<BillDetailsViewModel>(
                            this.BillViewModels
                            .Where(this.Filters.Select(filter => filter.Execute))));

                else
                    return new ReadOnlyObservableCollection<BillDetailsViewModel>(this.BillViewModels);
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

        private IEnumerable<Filter<BillDetailsViewModel>> filters;
        public IEnumerable<Filter<BillDetailsViewModel>> Filters
        {
            get { return this.filters; }
            private set
            {
                if (this.filters == value) return;

                this.filters = value;
                this.NotifyOfPropertyChange(() => this.Filters);
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
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
            this.globalEventAggregator.Publish(new SupplierNameRequestMessage(supplierID, s => supp = s));
            return supp;
        }

        #region CRUD

        private void AddBill(Supplier supplier = null)
        {
            var supps = this.GetAvailableSuppliers();
            var newBvm = this.billAddEditViewModelFactory.Invoke(supps, new Bill(this.billsProvider.GetLastBillID() + 1));

            if (supplier != null) // TODO: safe operation? if supplier is not known by addbvm?
                newBvm.SelectedSupplier = supplier;

            //newBvm.SetupForAddEdit();
        tryAdd:
            if (this.windowManager.ShowDialog(newBvm) == true)
            {
                // TODO: make it possible to show the view through a dialogviewmodel (evaluate the idea)
                if (this.billsProvider.Add(newBvm.ExposedBill))
                {
                    var newBillDetVM = this.billDetailsViewModelFactory.Invoke(newBvm.ExposedBill);

                    this.BillViewModels.AddSorted(newBillDetVM);
                    this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                    this.SelectedBillViewModel = newBillDetVM;

                    this.globalEventAggregator.Publish(new BillAddedMessage(newBvm.ExposedBill));
                }
                else
                {
                    this.windowManager.ShowDialog(new DialogViewModel(
                        TranslationManager.Instance.Translate("AddBillFailed").ToString(),
                            TranslationManager.Instance.Translate("AddBillFailedMessage").ToString() +
                            Environment.NewLine +
                            TranslationManager.Instance.Translate("TryAgain").ToString()));
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
                if (this.billsProvider.Edit(baeVM.ExposedBill))
                {
                    var editedBillVM = this.BillViewModels.Single(bvm => bvm.ExposedBill == bill);
                    var wasSelected = this.SelectedBillViewModel == editedBillVM;
                    this.BillViewModels.SortEdited(editedBillVM);

                    if (wasSelected) // TODO: not working
                        this.SelectedBillViewModel = editedBillVM;

                    this.globalEventAggregator.Publish(new BillEditedMessage(baeVM.ExposedBill, oldVersion)); // TODO: move to confirm add edit command in addeditbvm
                }
                else
                {
                    this.windowManager.ShowDialog(
                        new DialogViewModel(
                            TranslationManager.Instance.Translate("EditFailed").ToString(),
                            TranslationManager.Instance.Translate("EditFailedMessage").ToString() +
                            Environment.NewLine +
                            TranslationManager.Instance.Translate("TryAgain").ToString()));
                }
            }
        }

        private void DeleteBill(Bill bill)
        {
            var question = new DialogViewModel(
                TranslationManager.Instance.Translate("DeleteBill").ToString(),
                TranslationManager.Instance.Translate("DeleteBillQuestion").ToString() +
                Environment.NewLine +
                Environment.NewLine +
                this.GetBillSummary(bill),
                new[]
                {
                    new DialogResponse(
                        ResponseType.Yes,
                        TranslationManager.Instance.Translate("Delete").ToString(),
                        TranslationManager.Instance.Translate("Yes").ToString()),
                    new DialogResponse(ResponseType.No,TranslationManager.Instance.Translate("No").ToString())
                });

            this.windowManager.ShowDialog(question);

            if (question.FinalResponse == ResponseType.Yes)
            {
                this.billsProvider.Delete(bill);

                this.BillViewModels.Remove(this.BillViewModels.Single(bvm => bvm.ExposedBill == bill));
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = null;

                this.globalEventAggregator.Publish(new BillDeletedMessage(bill));
            }
        }

        private void PayBill(Bill bill)
        {
            var dialog = new DialogViewModel(
                TranslationManager.Instance.Translate("PayBill").ToString(),
                TranslationManager.Instance.Translate("PayBillQuestion").ToString() +
                Environment.NewLine +
                Environment.NewLine +
                this.GetBillSummary(bill),
                new[] 
                { 
                    new DialogResponse(
                        ResponseType.Yes,
                        TranslationManager.Instance.Translate("PayBill").ToString()),
                    new DialogResponse(
                        ResponseType.No,
                        TranslationManager.Instance.Translate("No").ToString()) 
                });

            if (this.windowManager.ShowDialog(dialog) == true)
            {
                Bill oldVersion = (Bill)bill.Clone();

                bill.PaymentDate = DateTime.Today;

                this.billsProvider.Edit(bill);

                var editedVM = this.BillViewModels.Single(bvm => bvm.ExposedBill == bill);
                var wasSelected = this.SelectedBillViewModel == editedVM;
                this.BillViewModels.SortEdited(editedVM);

                if (wasSelected)
                    this.SelectedBillViewModel = editedVM;

                this.globalEventAggregator.Publish(new BillEditedMessage(bill, oldVersion));
            }
        }

        private void ShowBillDetails(BillDetailsViewModel billDetailsViewModel)
        {
            this.windowManager.ShowDialog(billDetailsViewModel, settings: new Dictionary<string, object>() { { "CanClose", true } });
        }

        #endregion

        #region message handlers

        public void Handle(BillsFilterMessage message)
        {
            this.Filters = message.Filters;

            if (!this.FilteredBillViewModels.Contains(this.SelectedBillViewModel))
                this.SelectedBillViewModel = null;
        }

        public void Handle(SupplierDeletedMessage message)
        {
            var bvmsToDelete = this.BillViewModels.Where(bvm => bvm.SupplierID == message.DeletedSupplier.ID).ToList();

            foreach (var bvm in bvmsToDelete)
            {
                if (this.billsProvider.Delete(bvm.ExposedBill))
                    this.BillViewModels.Remove(bvm);
                else
                    throw new ApplicationException("Couldn't delete this bill: " + Environment.NewLine + Environment.NewLine + this.GetBillSummary(bvm.ExposedBill));
            }

            if (this.Filters == null)
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
        }

        public void Handle(EditBillRequestMessage message)
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

        private IEnumerable<Supplier> GetAvailableSuppliers()
        {
            IEnumerable<Supplier> supps = Enumerable.Empty<Supplier>();
            this.globalEventAggregator.Publish(new AvailableSuppliersRequestMessage((s) => supps = s));
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
                if (this.addNewBillCommand == null)
                    this.addNewBillCommand = new RelayCommand(() => this.AddBill());

                return this.addNewBillCommand;
            }
        }

        private RelayCommand<BillDetailsViewModel> editBillCommand;
        public RelayCommand<BillDetailsViewModel> EditBillCommand
        {
            get
            {
                if (this.editBillCommand == null)
                    this.editBillCommand = new RelayCommand<BillDetailsViewModel>(
                        p => this.EditBill(p.ExposedBill),
                        p => p != null);

                return this.editBillCommand;
            }
        }

        private RelayCommand<BillDetailsViewModel> deleteBillCommand;
        public RelayCommand<BillDetailsViewModel> DeleteBillCommand
        {
            get
            {
                if (this.deleteBillCommand == null)
                    this.deleteBillCommand = new RelayCommand<BillDetailsViewModel>(
                        p => this.DeleteBill(p.ExposedBill),
                        p => p != null);

                return this.deleteBillCommand;
            }
        }

        private RelayCommand<BillDetailsViewModel> payBillCommand;
        public RelayCommand<BillDetailsViewModel> PayBillCommand
        {
            get
            {
                if (this.payBillCommand == null)
                    this.payBillCommand = new RelayCommand<BillDetailsViewModel>(
                        p => this.PayBill(p.ExposedBill),
                        p => p != null && !p.IsPaid);

                return this.payBillCommand;
            }
        }

        private RelayCommand<BillDetailsViewModel> showBillDetailsCommand;
        public RelayCommand<BillDetailsViewModel> ShowBillDetailsCommand
        {
            get
            {
                if (this.showBillDetailsCommand == null)
                    this.showBillDetailsCommand = new RelayCommand<BillDetailsViewModel>(
                        p => this.ShowBillDetails(p),
                        p => p != null);

                return this.showBillDetailsCommand;
            }
        }

        #endregion
    }
}