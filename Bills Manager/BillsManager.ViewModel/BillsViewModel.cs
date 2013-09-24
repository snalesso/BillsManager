using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BillsManager.Model;
using BillsManager.Service;
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
        IHandle<SupplierDeletedMessage>
    {
        #region fields

        private readonly IBillsProvider billsProvider;
        private readonly IWindowManager windowManager;
        //private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public BillsViewModel(
            IBillsProvider billsProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator)
        {
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.billsProvider = billsProvider;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);

            this.LoadBills();
        }

        #endregion

        #region properties

        private ObservableCollection<BillViewModel> billViewModels;
        public ObservableCollection<BillViewModel> BillViewModels
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

        public ReadOnlyObservableCollection<BillViewModel> FilteredBillViewModels
        {
            get
            {
                if (this.BillsFilters != null)

                    return new ReadOnlyObservableCollection<BillViewModel>( // TODO: check whether this is a good practice
                        new ObservableCollection<BillViewModel>(
                            this.BillViewModels
                            .Where(this.BillsFilters)));

                else
                    return new ReadOnlyObservableCollection<BillViewModel>(this.BillViewModels);
            }
        }

        private BillViewModel selectedBillViewModel;
        public BillViewModel SelectedBillViewModel
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

        private IEnumerable<Func<BillViewModel, bool>> billsFilters;
        public IEnumerable<Func<BillViewModel, bool>> BillsFilters
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

        protected string GetBillInfo(Bill bill)
        {
            return
                bill.SupplierID +
                Environment.NewLine +
                bill.Code +
                Environment.NewLine +
                string.Format("{0:c}", bill.Amount);
        }

        protected void AddBill()
        {
            BillViewModel newBillVM = new BillViewModel(new Bill(this.billsProvider.GetLastID() + 1), this.windowManager, /*this.dialogService,*/ this.eventAggregator);

            newBillVM.SetupForAddEdit();

            if (this.windowManager.ShowDialog(newBillVM, settings: new Dictionary<String, object> { { "ResizeMode", ResizeMode.NoResize }, { "IsCloseButtonVisible", true } })
                == true)
            {
                // TODO: make it possible to show the view through a dialogviewmodel (evaluate the idea)
                this.billsProvider.Add(newBillVM.ExposedBill);

                this.BillViewModels.Add(newBillVM);
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = newBillVM;

                this.eventAggregator.Publish(new BillAddedMessage(newBillVM.ExposedBill));
            }
        }

        protected void EditBill(BillViewModel billViewModel)
        {
            Bill oldVersion = (Bill)billViewModel.ExposedBill.Clone();

            billViewModel.BeginEdit();

            if (this.windowManager.ShowDialog(billViewModel, null, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
            {
                if (this.billsProvider.Edit(billViewModel.ExposedBill))
                {
                    this.eventAggregator.Publish(new BillEditedMessage(billViewModel.ExposedBill, oldVersion));
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

        protected void DeleteBill(BillViewModel billViewModel)
        {
            //if (this.dialogService.ShowYesNoQuestion("Delete bill", "Do you really want to delete this bill?\n\n" +
            //    "Supplier.Name" + ", " + p.Code + " " + string.Format("{0:c}", p.Amount)))

            var question = new DialogViewModel(
                "Deleting bill",
                "Do you really want to DELETE this bill?\n\n" + // TODO: language
                this.GetBillInfo(billViewModel.ExposedBill),
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"),
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

            if (question.Response == ResponseType.Yes)
            {
                this.billsProvider.Delete(billViewModel.ExposedBill);

                this.BillViewModels.Remove(billViewModel);
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                this.SelectedBillViewModel = null;

                this.eventAggregator.Publish(new BillDeletedMessage(billViewModel.ExposedBill));
            }
        }

        protected void PayBill(BillViewModel billViewModel)
        {
            Bill oldVersion = (Bill)billViewModel.ExposedBill.Clone();

            billViewModel.PaymentDate = DateTime.Today;

            this.billsProvider.Edit(billViewModel.ExposedBill);

            this.eventAggregator.Publish(new BillEditedMessage(billViewModel.ExposedBill, oldVersion));
        }

        protected void LoadBills()
        {
            this.BillViewModels = new ObservableCollection<BillViewModel>(
                this.billsProvider.GetAll()
                .Select(b => new BillViewModel(b, this.windowManager, this.eventAggregator)));
        }

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
                    throw new ApplicationException("Couldn't delete this bill: " + Environment.NewLine + Environment.NewLine + this.GetBillInfo(bvm.ExposedBill));
            }

            if (this.BillsFilters == null)
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
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

        private RelayCommand<BillViewModel> editBillCommand;
        public RelayCommand<BillViewModel> EditBillCommand
        {
            get
            {
                if (this.editBillCommand == null)
                    this.editBillCommand = new RelayCommand<BillViewModel>(
                        p => this.EditBill(p),
                        p => p != null);

                return this.editBillCommand;
            }
        }

        private RelayCommand<BillViewModel> deleteBillCommand;
        public RelayCommand<BillViewModel> DeleteBillCommand
        {
            get
            {
                if (this.deleteBillCommand == null)
                    this.deleteBillCommand = new RelayCommand<BillViewModel>(
                        p => this.DeleteBill(p),
                        p => p != null);

                return this.deleteBillCommand;
            }
        }

        private RelayCommand<BillViewModel> payBillCommand;
        public RelayCommand<BillViewModel> PayBillCommand
        {
            get
            {
                if (this.payBillCommand == null)
                    this.payBillCommand = new RelayCommand<BillViewModel>(
                        p => this.PayBill(p),
                        p => p != null && !p.IsPaid);

                return this.payBillCommand;
            }
        }

        #endregion
    }
}