using System.Collections.Generic;
using System.Windows;
using BillsManager.Service;
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
        private readonly IEventAggregator eventAggregator; // TODO: review who should inherit this specific instance
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
        }

        #endregion

        #region properties

        private BillsViewModel billsViewModel;
        public BillsViewModel BillsViewModel
        {
            get
            {

                if (this.billsViewModel == null)
                    this.billsViewModel = this.billsViewModelFactory.Create();

                return this.billsViewModel;
            }
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
            get
            {
                if (this.suppliersViewModel == null)
                    this.suppliersViewModel = this.suppliersViewModelFactory.Create();

                return this.suppliersViewModel;
            }
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
            get
            {
                if (this.searchBillsViewModel == null)
                    this.searchBillsViewModel = new SearchBillsViewModel(this.eventAggregator);

                return this.searchBillsViewModel;
            }
            private set
            {
                if (this.searchBillsViewModel != value)
                {
                    this.searchBillsViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SearchBillsViewModel);
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
                        this.windowManager.ShowDialog(this.backupsViewModelFactory.Create(), null, settings: new Dictionary<string, object>
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