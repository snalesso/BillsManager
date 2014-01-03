using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BillsManager.ViewModels
{
    public partial class ShellViewModel : Conductor<DBsViewModel>.Collection.AllActive,
        IHandle<ActiveDBChangedMessage>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly Func<BackupCenterViewModel> backupCenterViewModelFactory;
        private readonly Func<string, DBsViewModel> dbsViewModelFactory;
        private readonly Func<StatusBarViewModel> statusBarViewModelFactory;
        private readonly Func<SendFeedbackViewModel> sendFeedbackViewModelFactory;

        #endregion

        #region ctor

        public ShellViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            Func<string, DBsViewModel> dbsViewModelFactory,
            Func<BackupCenterViewModel> backupCenterViewModelFactory,
            Func<StatusBarViewModel> statusBarViewModelFactory,
            Func<SendFeedbackViewModel> sendFeedbackViewModelFactory)
        {

            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;
            this.globalEventAggregator.Subscribe(this);

            // FACTORIES
            this.dbsViewModelFactory = dbsViewModelFactory;
            this.backupCenterViewModelFactory = backupCenterViewModelFactory;
            this.statusBarViewModelFactory = statusBarViewModelFactory;
            this.sendFeedbackViewModelFactory = sendFeedbackViewModelFactory;

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        this.globalEventAggregator.Unsubscribe(this);
                    }
                };

            // UI
            this.DisplayName = "Bills Manager"; // TODO: language

            // START
            this.ActivateItem(this.DBsViewModel);
        }

        #endregion

        #region properties

        private DBsViewModel dbsViewModel;
        public DBsViewModel DBsViewModel
        {
            get
            {
                if (this.dbsViewModel == null)
                    this.dbsViewModel = dbsViewModelFactory.Invoke(AppDomain.CurrentDomain.BaseDirectory + @"\Databases\");

                return dbsViewModel;
            }
            //private set
            //{
            //    if (this.dbsViewModel != value)
            //    {
            //        dbsViewModel = value;
            //        this.NotifyOfPropertyChange(() => this.DBsViewModel);
            //    }
            //}
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
            //private set
            //{
            //    if (this.statusBarViewModel != value)
            //    {
            //        this.statusBarViewModel = value;
            //        this.NotifyOfPropertyChange(() => this.StatusBarViewModel);
            //    }
            //}
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

        public void Handle(ActiveDBChangedMessage message)
        {
            var dn = "Bills Manager" + (message.ActiveDB != null ? " - " + message.ActiveDB.DisplayName : string.Empty);
            this.DisplayName = dn;
        }

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