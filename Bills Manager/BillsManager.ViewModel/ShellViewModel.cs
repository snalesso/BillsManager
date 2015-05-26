using BillsManager.Localization;
using BillsManager.Services.Settings;
using BillsManager.ViewModels.Commanding;
//using BillsManager.ViewModels.Search;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BillsManager.ViewModels
{
    public partial class ShellViewModel : Conductor<Screen>
    //,IHandle<ActiveDBChangedMessage>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly ISettingsProvider settingsProvider;
        // main region
        private readonly Func<DBViewModel> dbViewModelFactory;
        // tools
        private readonly Func<BackupCenterViewModel> backupCenterViewModelFactory;
        private readonly Func<SendFeedbackViewModel> sendFeedbackViewModelFactory;
        private readonly Func<SettingsViewModel> settingsViewModelFactory;
        // other UI regions
        //private readonly Func<SearchViewModel<BillDetailsViewModel>> searchBillsViewModelFactory;
        private readonly Func<StatusBarViewModel> statusBarViewModelFactory;

        #endregion

        #region ctor

        public ShellViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            ISettingsProvider settingsProvider,
            Func<DBViewModel> dbViewModelFactory,
            //Func<SearchViewModel<BillDetailsViewModel>> searchBillsViewModelFactory,
            Func<StatusBarViewModel> statusBarViewModelFactory,
            Func<BackupCenterViewModel> backupCenterViewModelFactory,
            Func<SendFeedbackViewModel> sendFeedbackViewModelFactory,
            Func<SettingsViewModel> settingsViewModelFactory)
        {

            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;
            this.settingsProvider = settingsProvider;

            // FACTORIES
            this.dbViewModelFactory = dbViewModelFactory;
            //this.searchBillsViewModelFactory = searchBillsViewModelFactory;
            this.statusBarViewModelFactory = statusBarViewModelFactory;
            this.backupCenterViewModelFactory = backupCenterViewModelFactory;
            this.sendFeedbackViewModelFactory = sendFeedbackViewModelFactory;
            this.settingsViewModelFactory = settingsViewModelFactory;

            // SUBSCRIPTIONS
            this.globalEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
                };

            // UI
            var sb = this.StatusBarViewModel; // initialize the status bar in order to receive db first load notifications
            this.DisplayName = TranslationManager.Instance.Translate("BillsManager");

            // START
            this.ActivateItem(this.DBViewModel);

            if (this.settingsProvider.Settings.StartupDBLoad)
                this.DBViewModel.Connect();
        }

        #endregion

        #region properties

        private DBViewModel dbViewModel;
        public DBViewModel DBViewModel
        {
            get
            {
                return this.dbViewModel ?? (this.dbViewModel = this.dbViewModelFactory.Invoke());
            }
        }


        //private SearchViewModel<BillDetailsViewModel> searchViewModel;
        //public SearchViewModel<BillDetailsViewModel> SearchViewModel
        //{
        //    get
        //    {
        //        if (this.searchViewModel == null)
        //            this.searchViewModel = this.searchBillsViewModelFactory.Invoke();

        //        return this.searchViewModel;
        //    }
        //}

        private StatusBarViewModel statusBarViewModel;
        public StatusBarViewModel StatusBarViewModel
        {
            get
            {
                return this.statusBarViewModel ?? (this.statusBarViewModel = this.statusBarViewModelFactory.Invoke());
            }
        }

        #endregion

        #region methods

        private void ShowBackupCenter()
        {
            this.windowManager.ShowDialog(
                this.backupCenterViewModelFactory.Invoke(),
                settings: new Dictionary<string, object>
                {
                    //{"ResizeMode", ResizeMode.CanResize},
                    {"SizeToContent", SizeToContent.Width},
                    //{"ResizeDirections", new Thickness(0, 1, 0, 1)},
                    {"CanClose", true}/*,
                    {"ShowInTaskbar", true}*/
                });
        }

        private void ShowSendFeedback()
        {
            this.windowManager.ShowDialog(
                this.sendFeedbackViewModelFactory.Invoke(),
                settings: new Dictionary<string, object>()
                {
                    {"CanClose", true}
                });
        }

        private void ShowSettings()
        {
            this.windowManager.ShowDialog(
                this.settingsViewModelFactory.Invoke(),
                settings: new Dictionary<string, object>()
                {
                    {"CanClose", false}
                });
        }

        #endregion

        #region commands

        private RelayCommand showBackupCenterCommand;
        public RelayCommand ShowBackupCenterCommand
        {
            get
            {
                return this.showBackupCenterCommand ?? (this.showBackupCenterCommand =
                    new RelayCommand(
                        () => this.ShowBackupCenter()));
            }
        }

        private RelayCommand showSendFeedbackCommand;
        public RelayCommand ShowSendFeedbackCommand
        {
            get
            {
                return this.showSendFeedbackCommand ?? (this.showSendFeedbackCommand =
                    new RelayCommand(
                        () => this.ShowSendFeedback()));
            }
        }

        private RelayCommand showSettingsCommand;
        public RelayCommand ShowSettingsCommand
        {
            get
            {
                return this.showSettingsCommand ?? (this.showSettingsCommand =
                    new RelayCommand(
                        () => this.ShowSettings()));
            }
        }

        #endregion
    }
}