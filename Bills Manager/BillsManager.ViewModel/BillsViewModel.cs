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
        //IHandle<AvailableSuppliersMessage>,
        IHandle<BillsNeedRefreshMessage>,
        IHandle<BillsFilterMessage>,
        IHandle<SupplierNameChangedMessage>,
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
        }

        #endregion

        #region properties

        private SearchBillsViewModel searchBillsViewModel;
        public SearchBillsViewModel SearchBillsViewModel
        {
            get
            {
                if (this.searchBillsViewModel == null) this.searchBillsViewModel = new SearchBillsViewModel(this.eventAggregator);

                return this.searchBillsViewModel;
            }
            set
            {
                if (this.searchBillsViewModel != value)
                {
                    this.searchBillsViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SearchBillsViewModel);
                }
            }
        }

        private ObservableCollection<BillViewModel> billViewModels;
        public ObservableCollection<BillViewModel> BillViewModels
        {
            get
            {
                if (this.billViewModels == null)
                {
                    if (!Execute.InDesignMode) // move provider loading logic to ctor to easily separate from dt
                    {
                        this.RefreshBillsList(false);
                    }
                    else // IF IN DESGIN MODE
                    {
                        this.billViewModels = new ObservableCollection<BillViewModel>();
                    }
                }

                return this.billViewModels;
            }
            private set
            {
                if (this.billViewModels != value)
                {
                    this.billViewModels = value;
                    this.NotifyOfPropertyChange(() => this.BillViewModels);
                    this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
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

        protected void RefreshBillsList(bool notify = true) // TODO: review and optimize
        {
            this.billViewModels = new ObservableCollection<BillViewModel>(this.billsProvider.GetAll().Select(b => new BillViewModel(b, this.windowManager, /*this.dialogService,*/ this.eventAggregator)));

            IEnumerable<Supplier> availableSuppliers = null;

            this.eventAggregator.Publish(new AskForAvailableSuppliersMessage(supps => availableSuppliers = supps));

            availableSuppliers.Apply(s => this.eventAggregator.Publish(
                new BillsOfSupplierMessage(
                    s.Name,
                    this.BillViewModels.Where(bvm => bvm.Supplier == s.Name).Select(bvm => bvm.ExposedBill))));

            if (notify)
            {
                this.NotifyOfPropertyChange(() => this.BillViewModels);
                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);
            }
        }

        #region message handlers

        //public void Handle(AvailableSuppliersMessage message)
        //{
        //    uint count = 0;

        //    foreach (Supplier s in message.AvailableSuppliers)
        //    {
        //        count++;
        //        break;
        //    }

        //    this.CanAddNewBill = (count != 0);
        //}

        public void Handle(BillsNeedRefreshMessage message)
        {
            this.RefreshBillsList();
        }

        public void Handle(BillsFilterMessage message)
        {
            this.BillsFilters = message.Filters;
        }

        public void Handle(SupplierNameChangedMessage message)
        {
            foreach (BillViewModel bvm in this.BillViewModels)
            {
                if (bvm.Supplier == message.OldName)
                    bvm.Supplier = message.NewName;
            }

            IEnumerable<Bill> bills = this.BillViewModels.Where(bvm => bvm.Supplier == message.NewName).Select(bvm => bvm.ExposedBill);

            if (bills.Count() > 0)
                this.billsProvider.Edit(bills);
        }

        public void Handle(SupplierDeletedMessage message)
        {
            var bvmsToDelete = this.BillViewModels.Where(bvm => bvm.Supplier == message.DeletedSupplier.Name).ToList();
            bvmsToDelete.Select(bvm => bvm.ExposedBill).Apply(b => this.billsProvider.Delete(b));
            foreach (var bvm in bvmsToDelete)
            {
                this.BillViewModels.Remove(bvm);
            }
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
                if (this.addNewBillCommand == null) this.addNewBillCommand = new RelayCommand(
                    () =>
                    {
                        BillViewModel newBillVM = new BillViewModel(new Bill(this.billsProvider.GetLastID() + 1), this.windowManager, /*this.dialogService,*/ this.eventAggregator);

                        newBillVM.SetupForAdEdit();

                        if (this.windowManager.ShowDialog(newBillVM, settings: new Dictionary<String, object> { { "ResizeMode", ResizeMode.NoResize }, { "IsCloseButtonVisible", true } }) == true)
                        {
                            this.billsProvider.Add(newBillVM.ExposedBill);

                            this.BillViewModels.Add(newBillVM);
                            this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                            this.SelectedBillViewModel = newBillVM;

                            this.eventAggregator.Publish(new BillsOfSupplierMessage(
                                newBillVM.Supplier,
                                this.BillViewModels.Select(bvm => bvm.ExposedBill).Where(b => b.Supplier == newBillVM.Supplier)));
                        }
                    });

                return this.addNewBillCommand;
            }
        }

        //private bool canAddNewBill = false; // TODO: change enabling system with add supp button in new bill
        //public bool CanAddNewBill
        //{
        //    get { return this.canAddNewBill; }
        //    set
        //    {
        //        if (this.canAddNewBill != value)
        //        {
        //            this.canAddNewBill = value;
        //            this.NotifyOfPropertyChange(() => this.CanAddNewBill);
        //        }
        //    }
        //}

        private RelayCommand<BillViewModel> editBillCommand;
        public RelayCommand<BillViewModel> EditBillCommand
        {
            get
            {
                if (this.editBillCommand == null)
                    this.editBillCommand = new RelayCommand<BillViewModel>(
                        p =>
                        {
                            p.BeginEdit();

                            if (this.windowManager.ShowDialog(
                                p,
                                null,
                                settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
                            {
                                if (this.billsProvider.Edit(p.ExposedBill))
                                {
                                    this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                                    this.eventAggregator.Publish(new BillsOfSupplierMessage(
                                        p.Supplier,
                                        this.BillViewModels.Select(bvm => bvm.ExposedBill).Where(b => b.Supplier == p.Supplier)));
                                }
                                else
                                {
                                    this.windowManager.ShowDialog(new DialogViewModel("Error", "Error during editing"));
                                    //this.dialogService.ShowMessage("Error", "Error during editing");
                                }
                            }
                        },
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
                        p =>
                        {
                            //if (this.dialogService.ShowYesNoQuestion("Delete bill", "Do you really want to delete this bill?\n\n" +
                            //    "Supplier.Name" + ", " + p.Code + " " + string.Format("{0:c}", p.Amount)))

                            var question = new DialogViewModel(
                                "Delete bill",
                                "Do you really want to delete this bill?\n\n" + p.Supplier + ", " + p.Code + " " + string.Format("{0:c}", p.Amount),
                                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, "Yes", 5),
                                    new DialogResponse(ResponseType.No, "No")
                                });

                            this.windowManager.ShowDialog(question);

                            if (question.Response == ResponseType.Yes)
                            // TODO: language
                            // TODO: Supplier support
                            {
                                this.billsProvider.Delete(p.ExposedBill);

                                this.BillViewModels.Remove(p);
                                this.NotifyOfPropertyChange(() => this.FilteredBillViewModels);

                                this.SelectedBillViewModel = null;

                                this.eventAggregator.Publish(new BillsOfSupplierMessage(
                                    p.Supplier,
                                    this.BillViewModels.Select(bvm => bvm.ExposedBill).Where(b => b.Supplier == p.Supplier)));
                            }
                        },
                        p => p != null);

                return this.deleteBillCommand;
            }
        }

        #endregion
    }
}