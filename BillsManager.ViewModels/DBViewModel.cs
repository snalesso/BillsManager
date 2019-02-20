using BillsManager.Localization;
using BillsManager.Models;
using BillsManager.Services.DB;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BillsManager.ViewModels
{
    public partial class DBViewModel : Conductor<Screen>.Collection.AllActive,
        IHandle<CRUDMessage>,
        IHandle<ShowSuppliersBillsOrder>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly IDBService dbConnector;

        private readonly Func<SuppliersViewModel> suppliersViewModelFactory;
        private readonly Func<BillsViewModel> billsViewModelFactory;
        //private readonly Func<TagsViewModel> tagsViewModelFactory;

        private readonly Func<SearchSuppliersViewModel> searchSuppliersViewModelFactory;
        private readonly Func<SearchBillsViewModel> searchBillsViewModelFactory;

        private readonly Func<IEnumerable<BillReportViewModel>, string, string, PrintReportViewModel> reportCenterViewModelFactory;

        #endregion

        #region ctor

        public DBViewModel(
            IWindowManager windowManager,
            IEventAggregator dbEventAggregator,
            IDBService dbConnector,
            Func<SuppliersViewModel> suppliersViewModelFactory,
            Func<BillsViewModel> billsViewModelFactory,
            //Func<TagsViewModel> tagsViewModelFactory,
            Func<SearchSuppliersViewModel> searchSuppliersViewModelFactory,
            Func<SearchBillsViewModel> searchBillsViewModelFactory,
            // IDEA: move report to tools?
            Func<IEnumerable<BillReportViewModel>, string, string, PrintReportViewModel> reportCenterViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAggregator = dbEventAggregator;
            this.dbConnector = dbConnector;

            // FACTORIES
            this.suppliersViewModelFactory = suppliersViewModelFactory;
            this.billsViewModelFactory = billsViewModelFactory;
            //this.tagsViewModelFactory = tagsViewModelFactory;

            this.searchSuppliersViewModelFactory = searchSuppliersViewModelFactory;
            this.searchBillsViewModelFactory = searchBillsViewModelFactory;

            this.reportCenterViewModelFactory = reportCenterViewModelFactory;

            // SUBSCRIPTIONS
            this.globalEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
                };

            //UI
            this.DisplayName = @"Bills Manager Database";

            // START
            //if (this.settingsProvider.Settings.StartupDBLoad)
            //{
            //    this.SuppliersViewModel.LoadSuppliers();
            //    this.BillsViewModel.LoadBills();
            //}
        }

        #endregion

        #region properties

        private BillsViewModel billsViewModel;
        public BillsViewModel BillsViewModel
        {
            get { return this.billsViewModel; }
            private set
            {
                if (this.billsViewModel == value) return;

                this.billsViewModel = value;
                this.NotifyOfPropertyChange(() => this.BillsViewModel);
            }
        }

        private SuppliersViewModel suppliersViewModel;
        public SuppliersViewModel SuppliersViewModel
        {
            get { return this.suppliersViewModel; }
            private set
            {
                if (this.suppliersViewModel == value) return;

                this.suppliersViewModel = value;
                this.NotifyOfPropertyChange(() => this.SuppliersViewModel);
            }
        }

        //private TagsViewModel tagsViewModel;
        //public TagsViewModel TagsViewModel
        //{
        //    get { return this.tagsViewModel; }
        //    private set
        //    {
        //        if (this.tagsViewModel == value) return;

        //        this.tagsViewModel = value;
        //        this.NotifyOfPropertyChange(() => this.TagsViewModel);
        //    }
        //}

        private SearchSuppliersViewModel searchSuppliersViewModel;
        public SearchSuppliersViewModel SearchSuppliersViewModel
        {
            get
            {
                //if (this.searchSuppliersViewModel == null)
                //    this.searchSuppliersViewModel = this.searchSuppliersViewModelFactory.Invoke(this.dbEventAggregator.Value);

                return this.searchSuppliersViewModel;
            }
            private set
            {
                if (this.searchSuppliersViewModel == value) return;

                this.searchSuppliersViewModel = value;
                this.NotifyOfPropertyChange(() => this.SearchSuppliersViewModel);
            }
        }

        private SearchBillsViewModel searchBillsViewModel;
        public SearchBillsViewModel SearchBillsViewModel
        {
            get
            {
                //if (this.searchBillsViewModel == null)
                //    this.searchBillsViewModel = this.searchBillsViewModelFactory.Invoke(this.dbEventAggregator.Value);

                return this.searchBillsViewModel;
            }
            private set
            {
                if (this.searchBillsViewModel == value) return;

                this.searchBillsViewModel = value;
                this.NotifyOfPropertyChange(() => this.SearchBillsViewModel);
            }
        }

        private DBConnectionState dbConnectionState = DBConnectionState.Disconnected;
        public DBConnectionState ConnectionState
        {
            get { return this.dbConnectionState; }
            set
            {
                if (this.dbConnectionState != value)
                {
                    this.dbConnectionState = value;
                    this.NotifyOfPropertyChange(() => this.ConnectionState);
                    this.globalEventAggregator.PublishOnUIThread(new DBConnectionStateChangedMessage(this.ConnectionState));

                    this.NotifyOfPropertyChange(() => this.IsConnectionActive);
                }
            }
        }

        public bool IsConnectionActive
        {
            get
            {
                return this.ConnectionState != DBConnectionState.Disconnected;
            }
        }

        private bool showFilters;
        public bool ShowFilters
        {
            get { return this.showFilters; }
            set
            {
                if (this.showFilters == value) return;

                this.showFilters = value;
                this.NotifyOfPropertyChange(() => this.ShowFilters);
            }
        }

        #region overrides

        public override void CanClose(System.Action<bool> callback)
        {
            if (this.ConnectionState != DBConnectionState.Disconnected)
                callback(this.Disconnect());
            else
                callback(true);
        }

        #endregion

        #endregion

        #region methods

        public bool Connect()
        {
            // TODO: give control to UI thread

            //var progressDialog = new ProgressViewModel("Loading ...");
            //this.windowManager.ShowWindow(progressDialog);

            if (this.dbConnector.Connect())
            {
                this.ConnectionState = DBConnectionState.Connected;

                this.SuppliersViewModel = this.suppliersViewModelFactory.Invoke();
                this.BillsViewModel = this.billsViewModelFactory.Invoke();

                this.SearchSuppliersViewModel = this.searchSuppliersViewModelFactory.Invoke();
                this.SearchBillsViewModel = this.searchBillsViewModelFactory.Invoke();

                this.ActivateItem(this.SearchSuppliersViewModel);
                this.ActivateItem(this.SearchBillsViewModel);

                this.ActivateItem(this.SuppliersViewModel);
                this.ActivateItem(this.BillsViewModel);

                //progressDialog.TryClose();

                return true;
            }

            //progressDialog.TryClose();

            DialogViewModel dbConnectionErrorDialog =
                DialogViewModel.Show(
                    DialogType.Error,
                    TranslationManager.Instance.Translate("DatabaseConnectionFailed"),
                    TranslationManager.Instance.Translate("DatabaseConnectionFailedMessage") +
                    Environment.NewLine +
                    TranslationManager.Instance.Translate("TryAgain"))
                .Ok();

            this.windowManager.ShowDialog(dbConnectionErrorDialog);

            return false;
        }

        public bool Disconnect()
        {
            if (this.ConnectionState == DBConnectionState.Unsaved)
            {
                DialogViewModel saveBeforeDisconnectDialog =
                    DialogViewModel.Show(
                        DialogType.Question,
                        TranslationManager.Instance.Translate("SaveQuestion"),
                        TranslationManager.Instance.Translate("ChangesNotSavedMessage") +
                        Environment.NewLine +
                        Environment.NewLine +
                        TranslationManager.Instance.Translate("SaveBeforeClosingQuestion"))
                    .YesNoCancel(
                        TranslationManager.Instance.Translate("SaveAndExit"),
                        TranslationManager.Instance.Translate("DontSave"),
                        TranslationManager.Instance.Translate("CancelExit"));

                this.windowManager.ShowDialog(saveBeforeDisconnectDialog);

                switch (saveBeforeDisconnectDialog.FinalResponse)
                {
                    case ResponseType.Yes:
                        if (!this.Save())
                        {
                            DialogViewModel errorDialog =
                                DialogViewModel.Show(
                                    DialogType.Error,
                                    TranslationManager.Instance.Translate("DatabaseSaveFailed"),
                                    TranslationManager.Instance.Translate("DatabaseSaveFailedMessage") +
                                    Environment.NewLine +
                                    TranslationManager.Instance.Translate("TryAgain"))
                                .Ok();

                            this.windowManager.ShowDialog(errorDialog);

                            return false;
                        }
                        break;

                    case ResponseType.No:
                        this.ConnectionState = DBConnectionState.Connected;
                        break;

                    case ResponseType.Cancel:
                        return false;
                }
            }

            this.dbConnector.Disconnect();

            this.ConnectionState = DBConnectionState.Disconnected;

            this.SearchSuppliersViewModel.TryClose();
            this.SearchBillsViewModel.TryClose();

            this.SearchSuppliersViewModel = null;
            this.SearchBillsViewModel = null;

            //this.TagsViewModel.TryClose();
            this.SuppliersViewModel.TryClose();
            this.BillsViewModel.TryClose(); /* TODO: review closing order, since this is an AllActive
                                             * a check is needed on whether the canclose automatically closes
                                             * activeitems or these 2 lines are required anyhow */
            this.SuppliersViewModel = null;
            this.BillsViewModel = null;
            //this.TagsViewModel = null;

            return true;
        }

        private bool Save()
        {
            if (this.dbConnector.Save())
            {
                this.ConnectionState = DBConnectionState.Connected;
                return true;
            }

            return false;
        }

        private void ShowReportCenter()
        {
            var billReports = this.BillsViewModel.FilteredBillViewModels.Select(bvm => new BillReportViewModel(bvm));

            var header = @"Bills Manager";
            var comment = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.BillsViewModel.FiltersDescription))
                comment += this.BillsViewModel.FiltersDescription;

            this.windowManager.ShowDialog(
                this.reportCenterViewModelFactory.Invoke(billReports, header, comment),
                settings: new Dictionary<string, object>()
                {
                    { "CanClose", true },
                    { "WindowState", WindowState.Maximized},
                    { "SizeToContent", SizeToContent.Manual},
                    { "ResizeMode", ResizeMode.CanResize },
                    { "AllowsTransparency", false },
                    { "WindowStyle", WindowStyle.SingleBorderWindow }
                });
        }

        private void ToggleShowFilters()
        {
            this.ShowFilters = !this.ShowFilters;
        }

        #region message handlers

        //public void Handle(SuppliersListChangedMessage message)
        //{
        //    this.IsDirty = true; // TODO: why is it called 3 times?
        //}

        //public void Handle(BillsListChangedMessage message)
        //{
        //    this.IsDirty = true; // TODO: why is it called 3 times?
        //}

        public void Handle(CRUDMessage message)
        {
            this.ConnectionState = DBConnectionState.Unsaved;
        }

        public void Handle(ShowSuppliersBillsOrder message)
        {
            if (!this.ShowFilters)
                this.ShowFilters = true;
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                return this.connectCommand ?? (this.connectCommand = 
                    new RelayCommand(
                        () => this.Connect(),
                        () => this.ConnectionState == DBConnectionState.Disconnected));
            }
        }

        private RelayCommand disconnectCommand;
        public RelayCommand DisconnectCommand
        {
            get
            {
                return this.disconnectCommand ?? (this.disconnectCommand =
                    new RelayCommand(
                        () => this.Disconnect(),
                        () => this.ConnectionState != DBConnectionState.Disconnected));
            }
        }

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return this.saveCommand ?? (this.saveCommand = 
                    new RelayCommand(
                        () => this.Save(),
                        () => this.ConnectionState == DBConnectionState.Unsaved));
            }
        }

        private RelayCommand showReportCenterCommand;
        public RelayCommand ShowReportCenterCommand
        {
            get
            {
                return this.showReportCenterCommand ?? (this.showReportCenterCommand = 
                    new RelayCommand(
                        () => this.ShowReportCenter(),
                        () => this.IsConnectionActive));
            }
        }

        private RelayCommand toggleShowFiltersCommand;
        public RelayCommand ToggleShowFiltersCommand
        {
            get
            {
                return this.toggleShowFiltersCommand ?? (this.toggleShowFiltersCommand =
                    new RelayCommand(
                        () => this.ToggleShowFilters()));
            }
        }

        #endregion
    }
}