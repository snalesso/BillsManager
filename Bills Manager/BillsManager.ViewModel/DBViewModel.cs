using BillsManager.Localization;
using BillsManager.Services.Providers;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using BillsManager.ViewModels.Reporting;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BillsManager.ViewModels
{
    public partial class DBViewModel : Conductor<Screen>.Collection.AllActive,
        IHandle<SupplierCRUDEvent>,
        IHandle<BillCRUDEvent>,
        IHandle<ShowSuppliersBillsOrder>,
        IHandle<RollbackAuthorizationRequestMessage>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly IDBConnector dbConnector;

        private readonly Func<SuppliersViewModel> suppliersViewModelFactory;
        private readonly Func<BillsViewModel> billsViewModelFactory;
        private readonly Func<TagsViewModel> tagsViewModelFactory;

        private readonly Func<SearchSuppliersViewModel> searchSuppliersViewModelFactory;
        private readonly Func<SearchBillsViewModel> searchBillsViewModelFactory;

        private readonly Func<IEnumerable<BillReportViewModel>, string, string, PrintReportViewModel> reportCenterViewModelFactory;

        #endregion

        #region ctor

        public DBViewModel(
            IWindowManager windowManager,
            IEventAggregator dbEventAggregator,
            IDBConnector dbConnector,
            Func<SuppliersViewModel> suppliersViewModelFactory,
            Func<BillsViewModel> billsViewModelFactory,
            Func<TagsViewModel> tagsViewModelFactory,
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
            this.tagsViewModelFactory = tagsViewModelFactory;

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

        private TagsViewModel tagsViewModel;
        public TagsViewModel TagsViewModel
        {
            get { return this.tagsViewModel; }
            private set
            {
                if (this.tagsViewModel == value) return;

                this.tagsViewModel = value;
                this.NotifyOfPropertyChange(() => this.TagsViewModel);
            }
        }

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
                    this.globalEventAggregator.Publish(new DBConnectionStateChangedMessage(this.ConnectionState));

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
            //var progressDialog = new ProgressViewModel("Loading '" + this.DBName + "' ...");

            // TODO: give control to UI thread
            //this.windowManager.ShowWindow(progressDialog);

            if (this.dbConnector.Open())
            {
                //progressDialog.TryClose();

                this.ConnectionState = DBConnectionState.Connected;

                this.SuppliersViewModel = this.suppliersViewModelFactory.Invoke();
                this.BillsViewModel = this.billsViewModelFactory.Invoke();
                this.TagsViewModel = this.tagsViewModelFactory.Invoke();

                this.SearchSuppliersViewModel = this.searchSuppliersViewModelFactory.Invoke();
                this.SearchBillsViewModel = this.searchBillsViewModelFactory.Invoke();

                this.ActivateItem(this.SearchSuppliersViewModel);
                this.ActivateItem(this.SearchBillsViewModel);

                this.ActivateItem(this.SuppliersViewModel);
                this.ActivateItem(this.BillsViewModel);

                return true;
            }

            //progressDialog.TryClose();

            var dbConnectionErrorDialog =
                new DialogViewModel(
                    "Database load failed", // TODO: language
                    "Database couldn't be opened." +
                    Environment.NewLine +
                    TranslationManager.Instance.Translate("TryAgain").ToString());

            this.windowManager.ShowDialog(dbConnectionErrorDialog);

            return false;
        }

        public bool Disconnect()
        {
            if (this.ConnectionState == DBConnectionState.Unsaved)
            {
                var saveRequest =
                    new DialogViewModel(
                        TranslationManager.Instance.Translate("SaveQuestion").ToString(),
                        TranslationManager.Instance.Translate("ChangesNotSavedMessage").ToString() +
                        Environment.NewLine +
                        Environment.NewLine +
                        TranslationManager.Instance.Translate("SaveBeforeClosingQuestion").ToString(),
                        new[]
                        {
                            new DialogResponse(
                                ResponseType.Yes,
                                TranslationManager.Instance.Translate("Yes").ToString()),
                            new DialogResponse(
                                ResponseType.No,
                                TranslationManager.Instance.Translate("No").ToString()),
                            new DialogResponse(ResponseType.Cancel,
                                TranslationManager.Instance.Translate("CancelExit").ToString())
                        });

                this.windowManager.ShowDialog(saveRequest);

                switch (saveRequest.FinalResponse)
                {
                    case ResponseType.Yes:
                        if (!this.Save())
                        {
                            var errorDialog =
                                new DialogViewModel(
                                    "Database save failed", // TODO: language
                                    "Database couldn't be saved." +
                                    Environment.NewLine +
                                    TranslationManager.Instance.Translate("TryAgain").ToString());

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

            this.dbConnector.Close();

            this.ConnectionState = DBConnectionState.Disconnected;

            this.SearchSuppliersViewModel.TryClose();
            this.SearchBillsViewModel.TryClose();

            this.SearchSuppliersViewModel = null;
            this.SearchBillsViewModel = null;

            this.TagsViewModel.TryClose();
            this.SuppliersViewModel.TryClose();
            this.BillsViewModel.TryClose(); /* TODO: review closing order, since this is an AllActive
                                             * a check is needed on whether the canclose automatically closes
                                             * activeitems or these 2 lines are required anyhow */
            this.SuppliersViewModel = null;
            this.BillsViewModel = null;
            this.TagsViewModel = null;

            return true;
        }

        private void Reload()
        {
            this.Disconnect();
            this.Connect();
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
            List<BillReportViewModel> billReports = new List<BillReportViewModel>(); ;

            foreach (var bdvm in this.BillsViewModel.FilteredBillViewModels)
            {
                billReports.Add(new BillReportViewModel(bdvm));
            }

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

        public void Handle(BillCRUDEvent message)
        {
            this.ConnectionState = DBConnectionState.Unsaved;
        }

        public void Handle(SupplierCRUDEvent message)
        {
            this.ConnectionState = DBConnectionState.Unsaved;
        }

        public void Handle(ShowSuppliersBillsOrder message)
        {
            if (!this.ShowFilters)
                this.ShowFilters = true;
        }

        public void Handle(RollbackAuthorizationRequestMessage message)
        {
            var canRollback = true;
            var wasConnected = this.ConnectionState == DBConnectionState.Connected;

            if (wasConnected)
                canRollback = this.Disconnect();

            if (canRollback) // TODO: check if authorization can be granted
            {
                message.ConfirmAuthorization();
                if (wasConnected)
                    this.Connect();
            }
            else
                message.NegateAuthorization();
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                if (this.connectCommand == null)
                    this.connectCommand = new RelayCommand(
                        () => this.Connect(),
                        () => this.ConnectionState == DBConnectionState.Disconnected);

                return this.connectCommand;
            }
        }

        private RelayCommand disconnectCommand;
        public RelayCommand DisconnectCommand
        {
            get
            {
                if (this.disconnectCommand == null)
                    this.disconnectCommand = new RelayCommand(
                        () => this.Disconnect(),
                        () => this.ConnectionState != DBConnectionState.Disconnected);

                return this.disconnectCommand;
            }
        }

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (this.saveCommand == null)
                    this.saveCommand = new RelayCommand(
                        () => this.Save(),
                        () => this.ConnectionState == DBConnectionState.Unsaved);

                return this.saveCommand;
            }
        }

        private RelayCommand showReportCenterCommand;
        public RelayCommand ShowReportCenterCommand
        {
            get
            {
                if (this.showReportCenterCommand == null)
                    this.showReportCenterCommand = new RelayCommand(
                        () => this.ShowReportCenter(),
                        () => this.IsConnectionActive);

                return this.showReportCenterCommand;
            }
        }

        private RelayCommand toggleShowFiltersCommand;
        public RelayCommand ToggleShowFiltersCommand
        {
            get
            {
                if (this.toggleShowFiltersCommand == null)
                    this.toggleShowFiltersCommand = new RelayCommand(
                        () => this.ToggleShowFilters());

                return this.toggleShowFiltersCommand;
            }
        }

        #endregion
    }
}