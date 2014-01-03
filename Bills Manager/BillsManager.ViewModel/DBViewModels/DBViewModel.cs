using Autofac.Features.OwnedInstances;
using BillsManager.Services.Providers;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using BillsManager.ViewModels.Reporting;
using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace BillsManager.ViewModels
{
    public partial class DBViewModel : Conductor<Screen>.Collection.AllActive,
        IHandle<SupplierCRUDEvent>,
        IHandle<BillCRUDEvent>,
        IHandle<ShowSuppliersBillsOrder>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly Owned<IEventAggregator> dbEventAggregator; // TODO: review owned<> mechanism
        private readonly IDBConnector dbConnector;

        private readonly Func<IEventAggregator, IBillsProvider, BillsViewModel> billsViewModelFactory;
        private readonly Func<IEventAggregator, ISuppliersProvider, SuppliersViewModel> suppliersViewModelFactory;

        private readonly Func<IEventAggregator, SearchSuppliersViewModel> searchSuppliersViewModelFactory;
        private readonly Func<IEventAggregator, SearchBillsViewModel> searchBillsViewModelFactory;

        private readonly Func<IEnumerable<BillReportViewModel>, string, string, ReportCenterViewModel> reportCenterViewModelFactory;

        #endregion

        #region ctor

        public DBViewModel(
            IWindowManager windowManager,
            Owned<IEventAggregator> dbEventAggregator,
            IDBConnector dbConnector,
            Func<IEventAggregator, ISuppliersProvider, SuppliersViewModel> suppliersViewModelFactory,
            Func<IEventAggregator, IBillsProvider, BillsViewModel> billsViewModelFactory,
            Func<IEventAggregator, SearchSuppliersViewModel> searchSuppliersViewModelFactory,
            Func<IEventAggregator, SearchBillsViewModel> searchBillsViewModelFactory,
            Func<IEnumerable<BillReportViewModel>, string, string, ReportCenterViewModel> reportCenterViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.dbEventAggregator = dbEventAggregator;
            this.dbConnector = dbConnector;

            // FACTORIES
            this.suppliersViewModelFactory = suppliersViewModelFactory;
            this.billsViewModelFactory = billsViewModelFactory;

            this.searchSuppliersViewModelFactory = searchSuppliersViewModelFactory;
            this.searchBillsViewModelFactory = searchBillsViewModelFactory;

            this.reportCenterViewModelFactory = reportCenterViewModelFactory;

            // SUBSCRIPTIONS
            this.dbEventAggregator.Value.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.dbEventAggregator.Value.Unsubscribe(this);
                };
        }

        #endregion

        #region properties

        public new string DisplayName
        {
            get { return this.DBName; }
        }

        public string Path
        {
            get { return this.dbConnector.Path; }
        }

        public string DBName
        {
            get { return this.dbConnector.DBName; }
        }

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

        private bool isConnected;
        public bool IsConnected
        {
            get { return this.isConnected; }
            set
            {
                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    this.NotifyOfPropertyChange(() => this.IsConnected);
                }
            }
        }

        private bool isDirty;
        public bool IsDirty
        {
            get { return this.isDirty; }
            private set
            {
                if (this.isDirty != value)
                {
                    this.isDirty = value;
                    this.NotifyOfPropertyChange(() => this.IsDirty);
                    this.NotifyOfPropertyChange(() => this.IsNotDirty);
                }
            }
        }

        public bool IsNotDirty
        {
            get { return !this.IsDirty; }
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
            /* TODO: review trydisconnect order: 
             * it should check if it is connected before trying to disconnect,
             * so if you trydisconnect a non connected db, it just returns true and gg */

            if (this.IsConnected)
                callback(this.TryDisconnectDB());
            else
                callback(true);
        }

        #endregion

        #endregion

        #region methods

        public bool TryConnectDB()
        {
            //var progressDialog = new ProgressViewModel("Loading '" + this.DBName + "' ...");

            // TODO: give control to UI thread
            //this.windowManager.ShowWindow(progressDialog);

            if (this.dbConnector.Open())
            {
                //progressDialog.TryClose();

                this.IsConnected = true;

                this.SuppliersViewModel = this.suppliersViewModelFactory.Invoke(this.dbEventAggregator.Value, this.dbConnector);
                this.BillsViewModel = this.billsViewModelFactory.Invoke(this.dbEventAggregator.Value, this.dbConnector);

                this.SearchSuppliersViewModel = this.searchSuppliersViewModelFactory.Invoke(this.dbEventAggregator.Value);
                this.SearchBillsViewModel = this.searchBillsViewModelFactory.Invoke(this.dbEventAggregator.Value);

                this.ActivateItem(this.SearchSuppliersViewModel);
                this.ActivateItem(this.SearchBillsViewModel);

                this.ActivateItem(this.SuppliersViewModel);
                this.ActivateItem(this.BillsViewModel);

                return true;
            }

            //progressDialog.TryClose();

            var openErrorDialog =
                new DialogViewModel(
                    "Open " + this.DBName + "' failed", // TODO: language
                    "Database '" + this.DBName + "' couldn't be opened." +
                    Environment.NewLine +
                    "Please try again later.");

            this.windowManager.ShowDialog(openErrorDialog);

            return false;
        }

        public bool TryDisconnectDB()
        {
            if (this.IsDirty)
            {
                var saveRequest =
                    new DialogViewModel(
                        "Do you wish to save '" + this.DBName + "' ?", // TODO: language
                        "Database '" + this.DBName + "' has some changes that haven't been saved yet." +
                        Environment.NewLine +
                        Environment.NewLine +
                        "Do you wish to save before closing this database?",
                        new[] { new DialogResponse(ResponseType.Yes), new DialogResponse(ResponseType.No), new DialogResponse(ResponseType.Cancel) });

                this.windowManager.ShowDialog(saveRequest);

                switch (saveRequest.FinalResponse)
                {
                    case ResponseType.Yes:
                        if (!this.Save())
                        {
                            var errorDialog =
                                new DialogViewModel(
                                    "Save '" + this.DBName + "' failed", // TODO: language
                                    "Database '" + this.DBName + "' couldn't be saved." +
                                    Environment.NewLine +
                                    "Please try again later.");

                            this.windowManager.ShowDialog(errorDialog);

                            return false;
                        }
                        break;

                    case ResponseType.No:
                        this.IsDirty = false;
                        break;

                    case ResponseType.Cancel:
                        return false;
                }
            }

            this.dbConnector.Close();

            this.IsConnected = false;

            this.SearchSuppliersViewModel.TryClose();
            this.SearchBillsViewModel.TryClose();

            this.SearchSuppliersViewModel = null;
            this.SearchBillsViewModel = null;

            this.SuppliersViewModel.TryClose();
            this.BillsViewModel.TryClose(); /* TODO: review closing order, since this is an AllActive
                                             * a check is needed on whether the canclose automatically closes
                                             * activeitems or these 2 lines are required anyhow */
            this.SuppliersViewModel = null;
            this.BillsViewModel = null;

            return true;
        }

        private bool Save()
        {
            if (this.dbConnector.Save())
            {
                this.IsDirty = false;
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

            var header = this.DBName;
            var comment = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.BillsViewModel.FiltersDescription))
                comment += this.BillsViewModel.FiltersDescription;

            this.windowManager.ShowDialog(
                this.reportCenterViewModelFactory.Invoke(billReports, header, comment),
                settings: new Dictionary<string, object>()
                {
                    { "CanClose", true }/*,
                    { "ResizeMode", ResizeMode.CanResize },
                    { "AllowsTransparency", false },
                    { "WindowStyle", WindowStyle.SingleBorderWindow }*/
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
            this.IsDirty = true;
        }

        public void Handle(SupplierCRUDEvent message)
        {
            this.IsDirty = true;
        }

        public void Handle(ShowSuppliersBillsOrder message)
        {
            if (!this.ShowFilters)
                this.ShowFilters = true;
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (this.saveCommand == null)
                    this.saveCommand = new RelayCommand(
                        () => this.Save(),
                        () => this.IsDirty);

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
                        () => this.IsConnected);

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