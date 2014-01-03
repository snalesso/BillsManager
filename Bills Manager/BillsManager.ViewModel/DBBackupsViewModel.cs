using BillsManager.Models;
using BillsManager.Services.Providers;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace BillsManager.ViewModels
{
    // TODO: make conductor?
    public partial class DBBackupsViewModel : Screen
    {
        #region fields

        // TODO: cleanup unused fields
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly IBackupsProvider backupsProvider;

        #endregion

        #region ctor

        public DBBackupsViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            IBackupsProvider backupsProvider)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;
            this.backupsProvider = backupsProvider;

            // SUBSCRIPTIONS
            this.globalEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        this.globalEventAggregator.Unsubscribe(this);
                    }
                };

            // START
            this.RefreshBackups();
        }

        #endregion

        #region properties

        public string Location
        {
            get { return this.backupsProvider.Location; }
        } // TODO: replace in this vm where it is not used directly

        public string DBName
        {
            get { return this.backupsProvider.DBName; }
        }

        private ObservableCollection<BackupViewModel> backupViewModels;
        public ObservableCollection<BackupViewModel> BackupViewModels
        {
            get
            {
                if (this.backupViewModels == null)
                    this.backupViewModels = new ObservableCollection<BackupViewModel>();

                return this.backupViewModels;
            }
            protected set
            {
                if (this.backupViewModels != value)
                {
                    this.backupViewModels = value;
                    this.NotifyOfPropertyChange(() => this.BackupViewModels);
                }
            }
        }

        private BackupViewModel selectedBackupViewModel;
        public BackupViewModel SelectedBackupViewModel
        {
            get { return this.selectedBackupViewModel; }
            set
            {
                if (this.selectedBackupViewModel != value)
                {
                    this.selectedBackupViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SelectedBackupViewModel);
                }
            }
        }

        #endregion

        #region methods

        private void RefreshBackups()
        {
            this.BackupViewModels = new ObservableCollection<BackupViewModel>();

            var all = this.backupsProvider.GetAll();

            var ordAll = all.OrderByDescending(b => b.CreationTime);

            var vms = ordAll.Select(b => new BackupViewModel(b)); // TODO: factory?

            vms.Apply(vm => this.BackupViewModels.Add(vm));
        }

        private void CreateBackup()
        {
            var question = new DialogViewModel(
                "Create backup",
                "Are you sure you want to create a new backup?", // TODO: language
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Create new backup"),
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question);

            if (question.FinalResponse == ResponseType.Yes)
            {
                this.backupsProvider.CreateNew();
                this.RefreshBackups();
            }
        }

        private void Rollback(BackupViewModel backupViewModel)
        {
            this.globalEventAggregator.Publish(
                new RollbackAuthorizationRequestMessage(
                    this.DBName,
                    () =>
                    {
                        var question = new DialogViewModel(
                            "Rollback",
                            "Are you sure you want to ROLLBACK to the following backup?" + // TODO: language
                            Environment.NewLine +
                            Environment.NewLine + this.GetBackupInfo(backupViewModel.ExposedBackup),
                            new[]
                            {
                                new DialogResponse(ResponseType.Yes, "Rollback", "Confirm rollback"),
                                new DialogResponse(ResponseType.No)
                            });

                        this.windowManager.ShowDialog(question);

                        if (question.FinalResponse == ResponseType.Yes)
                        {
                            this.backupsProvider.Rollback(backupViewModel.ExposedBackup);
                            this.RefreshBackups();
                        }
                    },
                () =>
                {
                    this.windowManager.ShowDialog(
                        new DialogViewModel(
                            "Rollback failed", // TODO: language
                            "Couldn't execute rollback on '" + this.DBName + "' because the database is opened." +
                            Environment.NewLine +
                            Environment.NewLine +
                            "Please close it and retry."));
                }));
        }

        private void DeleteBackup(BackupViewModel backupViewModel)
        {
            var question = new DialogViewModel(
                "Delete backup",
                "Are you sure you want to DELETE the following backup?" +
                Environment.NewLine +
                Environment.NewLine +
                this.GetBackupInfo(backupViewModel.ExposedBackup),
                new[]
                {
                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"),
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question);

            if (question.FinalResponse == ResponseType.Yes)
            {
                this.backupsProvider.Rollback(backupViewModel.ExposedBackup);
                this.RefreshBackups();
            }
        }

        private string GetBackupInfo(Backup backup)
        {
            return backup.CreationTime.ToLongDateString() + "   " + backup.CreationTime.ToLongTimeString() +
                   Environment.NewLine +
                   Environment.NewLine +
                   backup.BillsCount + " bills" + // TODO: language
                   Environment.NewLine +
                   backup.SuppliersCount + " suppliers";
        }

        private void OpenBackupsFolder()
        {
            if (System.IO.Directory.Exists(this.backupsProvider.Location))
                Process.Start(this.backupsProvider.Location);
        }

        #endregion

        #region commands

        private RelayCommand createNewBackupCommand;
        public RelayCommand CreateNewBackupCommand
        {
            get
            {
                if (this.createNewBackupCommand == null)
                    this.createNewBackupCommand = new RelayCommand(
                    () => this.CreateBackup());

                return this.createNewBackupCommand;
            }
        }

        private RelayCommand<BackupViewModel> rollbackCommand;
        public RelayCommand<BackupViewModel> RollbackCommand
        {
            get
            {
                if (this.rollbackCommand == null)
                    this.rollbackCommand = new RelayCommand<BackupViewModel>(
                        p => this.Rollback(p),
                        p => p != null);

                return this.rollbackCommand;

            }
        }

        private RelayCommand<BackupViewModel> deleteBackupCommand;
        public RelayCommand<BackupViewModel> DeleteBackupCommand
        {
            get
            {
                if (this.deleteBackupCommand == null)
                    this.deleteBackupCommand = new RelayCommand<BackupViewModel>(
                        p => this.DeleteBackup(p),
                        p => p != null);

                return this.deleteBackupCommand;
            }
        }

        private RelayCommand openBackupsFolderCommand;
        public RelayCommand OpenBackupsFolderCommand
        {
            get
            {
                if (this.openBackupsFolderCommand == null)
                    this.openBackupsFolderCommand = new RelayCommand(
                        () => this.OpenBackupsFolder(),
                        () => System.IO.Directory.Exists(this.backupsProvider.Location));

                return this.openBackupsFolderCommand;
            }
        }

        #endregion
    }
}