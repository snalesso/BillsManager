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
    public partial class BillsViewModel :
        Screen,
        IHandle<BillsNeedRefreshMessage>,
        IHandle<BillsFilterMessage>,
        IHandle<SupplierDeletedMessage>,
        IHandle<EditBillRequestMessage>
    {
        #region fields

        private readonly IBillsProvider billsProvider;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public BillsViewModel(
            IBillsProvider billsProvider,
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            this.windowManager = windowManager;
            this.billsProvider = billsProvider;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);

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
                        this.eventAggregator.Publish(new BillsListChangedMessage(this.BillViewModels.Select(bvm => bvm.ExposedBill)));
                }
            }

        }

        public ReadOnlyObservableCollection<BillDetailsViewModel> FilteredBillViewModels
        {
            get
            {
                if (this.BillsFilters != null)

                    return new ReadOnlyObservableCollection<BillDetailsViewModel>( // TODO: check whether this is a good practice
                        new ObservableCollection<BillDetailsViewModel>(
                            this.BillViewModels
                            .Where(this.BillsFilters)));

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
                if (this.selectedBillViewModel != value)
                {
                    this.selectedBillViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SelectedBillViewModel);
                }
            }
        }

        private IEnumerable<Func<BillDetailsViewModel, bool>> billsFilters;
        public IEnumerable<Func<BillDetailsViewModel, bool>> BillsFilters
        {
            get { return this.billsFilters; }
            private set
            {
                if (this.billsFilters != value)
                {
                    this.billsFilters = value;
                    this.NotifyOfPropertyChange(() => this.BillsFilters);
                    this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
                }
            }
        }

        #endregion

        #region methods

        private void LoadBills()
        {
            this.BillViewModels = new ObservableCollection<BillDetailsViewModel>(
                this.billsProvider.GetAll()
                .Select(b => new BillDetailsViewModel(b, this.windowManager, this.eventAggregator)));
        }

        private string GetBillSummary(Bill bill)
        {
            var supp = this.GetSupplier(bill.SupplierID);

            return
                (supp != null ? supp.Name : "Supplier ID: " + bill.SupplierID) + // TODO: language
                Environment.NewLine +
                bill.Code +
                Environment.NewLine +
                string.Format("{0:C2}", bill.Amount);
        }

        private Supplier GetSupplier(uint supplierID)
        {
            // TODO: move supplier logic to BillsViewModel (same for supp's obligation amount)
            Supplier supp = null;
            this.eventAggregator.Publish(new AskForSupplierMessage(supplierID, s => supp = s));
            return supp;
        }

        #region CRUD

        private void AddBill()
        {
            var newBvm = new BillAddEditViewModel(new Bill(this.billsProvider.GetLastID() + 1), this.windowManager, this.eventAggregator);

            //newBvm.SetupForAddEdit();

            if (this.windowManager.ShowDialog(newBvm, settings: new Dictionary<String, object> { { "ResizeMode", ResizeMode.NoResize }, { "IsCloseButtonVisible", false } })
                == true)
            {
                // TODO: make it possible to show the view through a dialogviewmodel (evaluate the idea)
                this.billsProvider.Add(newBvm.ExposedBill);

                var newBvmDetails = new BillDetailsViewModel(newBvm.ExposedBill, this.windowManager, this.eventAggregator);

                this.BillViewModels.Add(newBvmDetails);
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = newBvmDetails;

                this.eventAggregator.Publish(new BillAddedMessage(newBvm.ExposedBill));
            }
        }

        private void EditBill(Bill bill)
        {
            var baeVM = new BillAddEditViewModel(bill, this.windowManager, this.eventAggregator);
            Bill oldVersion = (Bill)bill.Clone();

            baeVM.BeginEdit();

            if (this.windowManager.ShowDialog(baeVM,
                settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
            {
                if (this.billsProvider.Edit(baeVM.ExposedBill))
                {
                    this.BillViewModels.Single(bvm => bvm.ExposedBill == bill).Refresh();
                    this.eventAggregator.Publish(new BillEditedMessage(baeVM.ExposedBill, oldVersion)); // TODO: move to confirm add edit command in addeditbvm
                }
                else
                {
                    this.windowManager.ShowDialog(new DialogViewModel(
                        "Error", // TODO: language
                        "An error has been encountered during the editing process."),
                        settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });
                }
            }
        }

        private void DeleteBill(Bill bill)
        {
            var question = new DialogViewModel(
                "Deleting bill",
                "Do you really want to DELETE this bill?\n\n" + // TODO: language
                this.GetBillSummary(bill),
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"), // TODO: language
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

            if (question.Response == ResponseType.Yes)
            {
                this.billsProvider.Delete(bill);

                this.BillViewModels.Remove(this.BillViewModels.Single(bvm => bvm.ExposedBill == bill));
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = null;

                this.eventAggregator.Publish(new BillDeletedMessage(bill));
            }
        }

        private void PayBill(Bill bill)
        {
            var dialog = new DialogViewModel(
                "Paying bill",
                "Are you sure you want to pay this bill?" +
                Environment.NewLine +
                Environment.NewLine +
                this.GetBillSummary(bill),
                new[]
                {
                    new DialogResponse(ResponseType.Yes),
                    new DialogResponse(ResponseType.No)
                });

            if (this.windowManager.ShowDialog(dialog,
                settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
            {
                Bill oldVersion = (Bill)bill.Clone();

                bill.PaymentDate = DateTime.Today;

                this.billsProvider.Edit(bill);

                this.BillViewModels.Single(bvm => bvm.ExposedBill == bill).Refresh();

                this.eventAggregator.Publish(new BillEditedMessage(bill, oldVersion));
            }
        }

        private void ShowBillDetails(BillDetailsViewModel billDetailsViewModel)
        {
            this.windowManager.ShowDialog(
                billDetailsViewModel,
                settings: new Dictionary<String, object> { { "ResizeMode", ResizeMode.NoResize }, { "IsCloseButtonVisible", false } });
        }

        #endregion

        #region message handlers

        public void Handle(BillsNeedRefreshMessage message)
        {
            this.LoadBills();
        }

        public void Handle(BillsFilterMessage message)
        {
            this.BillsFilters = message.Filters;
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

            if (this.BillsFilters == null)
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
        }

        public void Handle(EditBillRequestMessage message)
        {
            this.EditBill(message.Bill); /* TODO: is it better to pass only the ID and let this VM to get the bill?
                                          * (this would check whether the bill is contained or is a lost one) */
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