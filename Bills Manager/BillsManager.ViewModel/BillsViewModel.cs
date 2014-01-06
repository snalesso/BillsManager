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
        //private readonly IEventAggregator globalEventAggregator;
        private readonly IEventAggregator dbEventAggregator;
        private readonly IBillsProvider billsProvider;

        private readonly Func<IEventAggregator, Bill, BillAddEditViewModel> billAddEditViewModelFactory;
        private readonly Func<IEventAggregator, Bill, BillDetailsViewModel> billDetailsViewModelFactory;

        #endregion

        #region ctor

        public BillsViewModel(
            IWindowManager windowManager,
            //IEventAggregator globalEventAggregator,
            IEventAggregator dbEventaAggregator,
            IBillsProvider billsProvider,
            Func<IEventAggregator, Bill, BillAddEditViewModel> billAddEditViewModelFactory,
            Func<IEventAggregator, Bill, BillDetailsViewModel> billDetailsViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            //this.globalEventAggregator = globalEventAggregator;
            this.dbEventAggregator = dbEventaAggregator;
            this.billsProvider = billsProvider;

            // FACTORIES
            this.billAddEditViewModelFactory = billAddEditViewModelFactory;
            this.billDetailsViewModelFactory = billDetailsViewModelFactory;

            // SUBSCRIPTIONS
            //this.globalEventAggregator.Subscribe(this); // TODO: move to activate?
            this.dbEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        //this.globalEventAggregator.Unsubscribe(this);
                        this.dbEventAggregator.Unsubscribe(this);
                    }
                };

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
                        this.dbEventAggregator.Publish(new BillsListChangedMessage(this.BillViewModels.Select(bvm => bvm.ExposedBill)));
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
                if (this.Filters == null) return "All bills";

                foreach (var filter in this.Filters)
                {
                    filtersDescription += ", " + filter.Description();
                }

                return "Bills" + filtersDescription.Substring(1);
            }
        }

        #endregion

        #region methods

        private void LoadBills()
        {
            var bills = this.billsProvider.GetAllBills();

            var billVMs = bills.Select(bill => this.billDetailsViewModelFactory.Invoke(this.dbEventAggregator, bill));

            var sortedBillVMs =
                billVMs.OrderBy(billVM => billVM.IsPaid)
                .ThenBy(billVM => billVM.DueDate)
                .ThenBy(billVM => billVM.Amount)
                .ThenBy(billVM => billVM.SupplierName);

            this.BillViewModels = new ObservableCollection<BillDetailsViewModel>(sortedBillVMs);
        }

        private string GetBillSummary(Bill bill)
        {
            var suppName = this.GetSupplierName(bill.SupplierID);

            return
                (suppName != null ? suppName : "Supplier ID: " + bill.SupplierID) + // TODO: language
                Environment.NewLine +
                bill.Code +
                Environment.NewLine +
                string.Format("{0:N2} €", bill.Amount);
        }

        private string GetSupplierName(uint supplierID)
        {
            string supp = null;
            this.dbEventAggregator.Publish(new SupplierNameRequestMessage(supplierID, s => supp = s));
            return supp;
        }

        #region CRUD

        private void AddBill(Supplier supplier = null)
        {
            var newBvm = this.billAddEditViewModelFactory.Invoke(this.dbEventAggregator, new Bill(this.billsProvider.GetLastBillID() + 1));

            if (supplier != null) // TODO: safe operation? if supplier is not known by addbvm?
                newBvm.SelectedSupplier = supplier;

            //newBvm.SetupForAddEdit();

            if (this.windowManager.ShowDialog(newBvm)
                == true)
            {
                // TODO: make it possible to show the view through a dialogviewmodel (evaluate the idea)
                this.billsProvider.Add(newBvm.ExposedBill);

                var newBvmDetails = this.billDetailsViewModelFactory.Invoke(this.dbEventAggregator, newBvm.ExposedBill);

                this.BillViewModels.Add(newBvmDetails); // TODO: use an event handler?
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = newBvmDetails;

                this.dbEventAggregator.Publish(new BillAddedMessage(newBvm.ExposedBill));
            }
        }

        private void EditBill(Bill bill)
        {
            var baeVM = this.billAddEditViewModelFactory.Invoke(this.dbEventAggregator, bill);

            Bill oldVersion = (Bill)bill.Clone();

            baeVM.BeginEdit();

            if (this.windowManager.ShowDialog(baeVM) == true)
            {
                if (this.billsProvider.Edit(baeVM.ExposedBill))
                {
                    this.BillViewModels.Single(bvm => bvm.ExposedBill == bill).Refresh();
                    this.dbEventAggregator.Publish(new BillEditedMessage(baeVM.ExposedBill, oldVersion)); // TODO: move to confirm add edit command in addeditbvm
                }
                else
                {
                    this.windowManager.ShowDialog(new DialogViewModel(
                        "Edit failed", // TODO: language
                        "An error has occurred during the edit process. Please try again."));
                }
            }
        }

        private void DeleteBill(Bill bill)
        {
            var question = new DialogViewModel(
                "Delete bill",
                "Do you really want to DELETE this bill?\n\n" + // TODO: language
                this.GetBillSummary(bill),
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"), // TODO: language
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question);

            if (question.FinalResponse == ResponseType.Yes)
            {
                this.billsProvider.Delete(bill);

                this.BillViewModels.Remove(this.BillViewModels.Single(bvm => bvm.ExposedBill == bill));
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = null;

                this.dbEventAggregator.Publish(new BillDeletedMessage(bill));
            }
        }

        private void PayBill(Bill bill)
        {
            var dialog = new DialogViewModel(
                "Pay bill",
                "Are you sure you want to pay this bill?" +
                Environment.NewLine +
                Environment.NewLine +
                this.GetBillSummary(bill),
                new[] { new DialogResponse(ResponseType.Yes), new DialogResponse(ResponseType.No) });

            if (this.windowManager.ShowDialog(dialog) == true)
            {
                Bill oldVersion = (Bill)bill.Clone();

                bill.PaymentDate = DateTime.Today;

                this.billsProvider.Edit(bill);

                this.BillViewModels.Single(bvm => bvm.ExposedBill == bill).Refresh();

                this.dbEventAggregator.Publish(new BillEditedMessage(bill, oldVersion));
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