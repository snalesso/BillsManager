using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Factories;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class ShellViewModel : Screen, IShell
    {
        #region fields

        //private readonly IDialogService dialogService;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IFactory<BillsViewModel> billsViewModelFactory;
        private readonly IFactory<SuppliersViewModel> suppliersViewModelFactory;
        private readonly IFactory<BackupsViewModel> backupsViewModelFactory;

        #endregion

        #region ctor

        public ShellViewModel(
            IFactory<BillsViewModel> billsViewModelFactory,
            IFactory<SuppliersViewModel> suppliersViewModelFactory,
            IFactory<BackupsViewModel> backupsViewModelFactory,
            //IDialogService dialogService,
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            this.billsViewModelFactory = billsViewModelFactory;
            this.suppliersViewModelFactory = suppliersViewModelFactory;
            this.backupsViewModelFactory = backupsViewModelFactory;

            //this.dialogService = dialogService;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;



            this.DisplayName = "Bills Manager";

            this.SuppliersViewModel = this.suppliersViewModelFactory.Create();
            this.BillsViewModel = this.billsViewModelFactory.Create();

            this.SearchBillsViewModel = new SearchBillsViewModel(this.SuppliersViewModel.SupplierViewModels.Select(svm => svm.ExposedSupplier), this.eventAggregator);
            this.SearchSuppliersViewModel = new SearchSuppliersViewModel(this.eventAggregator);
        }

        #endregion

        #region properties

        private BillsViewModel billsViewModel;
        public BillsViewModel BillsViewModel
        {
            get { return this.billsViewModel; }
            private set
            {
                if (this.billsViewModel != value)
                {
                    this.billsViewModel = value;
                    this.NotifyOfPropertyChange(() => this.BillsViewModel);
                }
            }
        }

        private SuppliersViewModel suppliersViewModel;
        public SuppliersViewModel SuppliersViewModel
        {
            get { return this.suppliersViewModel; }
            private set
            {
                if (this.suppliersViewModel != value)
                {
                    this.suppliersViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SuppliersViewModel);
                }
            }
        }

        private SearchBillsViewModel searchBillsViewModel;
        public SearchBillsViewModel SearchBillsViewModel
        {
            get { return this.searchBillsViewModel; }
            private set
            {
                if (this.searchBillsViewModel != value)
                {
                    this.searchBillsViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SearchBillsViewModel);
                }
            }
        }

        private SearchSuppliersViewModel searchSuppliersViewModel;
        public SearchSuppliersViewModel SearchSuppliersViewModel
        {
            get { return this.searchSuppliersViewModel; }
            set
            {
                if (this.searchSuppliersViewModel != value)
                {
                    this.searchSuppliersViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SearchSuppliersViewModel);
                }
            }
        }

        #endregion

        #region methods
        #endregion

        #region commands

        private RelayCommand showBackupCenterCommand;
        public RelayCommand ShowBackupCenterCommand
        {
            get
            {
                if (this.showBackupCenterCommand == null) this.showBackupCenterCommand = new RelayCommand(
                    () =>
                    {
                        this.windowManager.ShowDialog(this.backupsViewModelFactory.Create(), settings: new Dictionary<string, object>
                        {
                            {"ResizeMode", ResizeMode.NoResize}
                        });
                    });

                return this.showBackupCenterCommand;
            }
        }

        #endregion
    }
}