using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
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
        // main region
        private readonly Func<DBViewModel> dbViewModelFactory;
        // tools
        private readonly Func<BackupCenterViewModel> backupCenterViewModelFactory;
        private readonly Func<SendFeedbackViewModel> sendFeedbackViewModelFactory;
        // other UI regions
        private readonly Func<StatusBarViewModel> statusBarViewModelFactory;

        #endregion

        #region ctor

        public ShellViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            Func<DBViewModel> dbViewModelFactory,
            Func<StatusBarViewModel> statusBarViewModelFactory,
            Func<BackupCenterViewModel> backupCenterViewModelFactory,
            Func<SendFeedbackViewModel> sendFeedbackViewModelFactory)
        {

            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;
            this.globalEventAggregator.Subscribe(this);

            // FACTORIES
            this.dbViewModelFactory = dbViewModelFactory;
            this.statusBarViewModelFactory = statusBarViewModelFactory;
            this.backupCenterViewModelFactory = backupCenterViewModelFactory;
            this.sendFeedbackViewModelFactory = sendFeedbackViewModelFactory;

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
                };

            // UI
            var sb = this.StatusBarViewModel; // initialize the status bar in order to receive db first load notifications
            this.DisplayName = "Bills Manager"; // TODO: language

            // START
            this.ActivateItem(this.DBViewModel);
            this.DBViewModel.Connect();
        }

        #endregion

        #region properties

        private DBViewModel dbViewModel;
        public DBViewModel DBViewModel
        {
            get
            {
                if (this.dbViewModel == null)
                    this.dbViewModel = this.dbViewModelFactory.Invoke();

                return this.dbViewModel;
            }
        }

        private StatusBarViewModel statusBarViewModel;
        public StatusBarViewModel StatusBarViewModel
        {
            get
            {
                if (this.statusBarViewModel == null)
                    this.statusBarViewModel = this.statusBarViewModelFactory.Invoke();

                return this.statusBarViewModel;
            }
        }

        #endregion

        #region methods

        private void ShowBackupCenter()
        {
            this.windowManager.ShowDialog(this.backupCenterViewModelFactory.Invoke(), settings: new Dictionary<string, object>
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

        #region message handlers

        //public void Handle(ActiveDBChangedMessage message)
        //{
        //    var dn = "Bills Manager" + (message.ActiveDB != null ? " - " + message.ActiveDB.DisplayName : string.Empty);
        //    this.DisplayName = dn;
        //}

        #endregion

        #endregion

        #region commands

        private RelayCommand showBackupCenterCommand;
        public RelayCommand ShowBackupCenterCommand
        {
            get
            {
                if (this.showBackupCenterCommand == null)
                    this.showBackupCenterCommand = new RelayCommand(
                    () => this.ShowBackupCenter());

                return this.showBackupCenterCommand;
            }
        }

        private RelayCommand showSendFeedbackCommand;
        public RelayCommand ShowSendFeedbackCommand
        {
            get
            {
                if (this.showSendFeedbackCommand == null)
                    this.showSendFeedbackCommand = new RelayCommand(
                        () => this.ShowSendFeedback());

                return this.showSendFeedbackCommand;
            }
        }

        #endregion
    }
}