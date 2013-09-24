using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using BillsManager.Model;
using BillsManager.Service.Providers;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BackupsViewModel : Screen // TODO: add a background watcher or smth similar to keep an eye on the backups folder
    {
        #region fields

        private readonly IBackupsProvider backupsProvider;
        private readonly IWindowManager windowManager;
        //private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        private readonly string dbDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        private readonly string buDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\Backups\";

        #endregion

        #region ctor

        public BackupsViewModel(
            IBackupsProvider backupsProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator)
        {
            this.backupsProvider = backupsProvider;
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);

            this.DisplayName = "Backup Center";

            this.LoadBackups();
        }

        #endregion

        #region properties

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

        private void LoadBackups()
        {
            this.BackupViewModels = new ObservableCollection<BackupViewModel>(this.backupsProvider.GetAll()
                     .OrderByDescending(b => b.CreationTime)
                     .Select(b => new BackupViewModel(b)));
        }

        private void CreateNewBackup()
        {
            this.backupsProvider.CreateNew();
        }

        private void Rollback(Backup backup)
        {
            this.backupsProvider.Rollback(backup);

            this.eventAggregator.Publish(new SuppliersNeedRefreshMessage());
            this.eventAggregator.Publish(new BillsNeedRefreshMessage());
        }

        private void DeleteBackup(Backup backup)
        {
            this.backupsProvider.Delete(backup);
        }

        private string GetBackupInfo(Backup backup)
        {
            return backup.CreationTime.ToLongDateString() + "   " + backup.CreationTime.ToLongTimeString() +
                   Environment.NewLine +
                   Environment.NewLine +
                   backup.BillsCount + " bills" +
                   Environment.NewLine +
                   backup.SuppliersCount + " suppliers";
        }

        private void OpenBackupsFolder()
        {
            if (System.IO.Directory.Exists(this.buDirectory))
                Process.Start(this.buDirectory);
            else
                this.windowManager.ShowDialog(new DialogViewModel(
                    "Directory not found",
                    "Couldn't find any database folder." +
                    Environment.NewLine +
                    "Is this the first run?"),
                    settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }); // TODO: language
        }

        private void OpenDBFolder()
        {
            if (System.IO.Directory.Exists(this.dbDirectory))
                Process.Start(this.dbDirectory);
            else
                this.windowManager.ShowDialog(new DialogViewModel(
                    "Directory not found",
                    "Couldn't find any database folder." +
                    Environment.NewLine +
                    "Is this the first run?"),
                    settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }); // TODO: language
        }

        #endregion

        #region commands

        private RelayCommand createNewBackupCommand;
        public RelayCommand CreateNewBackupCommand
        {
            get
            {
                if (this.createNewBackupCommand == null) this.createNewBackupCommand = new RelayCommand(
                    () =>
                    {
                        var question = new DialogViewModel(
                            "New backup creation",
                            "Are you sure you want to create a new backup?", // TODO: language
                            new[]
                            {
                                new DialogResponse(ResponseType.Yes, "Create new backup"),
                                new DialogResponse(ResponseType.No)
                            });

                        this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

                        if (question.Response == ResponseType.Yes)
                        {
                            this.CreateNewBackup();
                            this.LoadBackups();
                        }
                    });

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
                        p =>
                        {
                            //if (this.dialogService.ShowYesNoQuestion
                            //    ("Confirm rollback",
                            //    "Are you sure you want to rollback to the following backup?"
                            var question = new DialogViewModel(
                                "Rolling back",
                                "Are you sure you want to ROLLBACK to the following backup?" + // TODO: language
                                Environment.NewLine +
                                Environment.NewLine + this.GetBackupInfo(p.ExposedBackup),
                                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, "Rollback", "Confirm rollback"),
                                    new DialogResponse(ResponseType.No)
                                });

                            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

                            if (question.Response == ResponseType.Yes)
                            {
                                this.Rollback(p.ExposedBackup);
                                this.LoadBackups();
                            }
                        },
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
                        p =>
                        {
                            //if (this.dialogService.ShowYesNoQuestion(
                            //    "Confirm rollback",
                            //    "Are you sure you want to delete the following backup?"
                            var question = new DialogViewModel(
                                "Deleting backup",
                                "Are you sure you want to DELETE the following backup?" +
                                Environment.NewLine +
                                Environment.NewLine +
                                this.GetBackupInfo(p.ExposedBackup),
                                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, "Delete", "Confirm delete"),
                                    new DialogResponse(ResponseType.No)
                                });

                            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

                            if (question.Response == ResponseType.Yes)
                            {
                                this.DeleteBackup(p.ExposedBackup);
                                this.LoadBackups();
                            }
                        },
                        p => p != null);

                return this.deleteBackupCommand;
            }
        }

        private RelayCommand openDBFolderCommand;
        public RelayCommand OpenDBFolderCommand
        {
            get
            {
                if (this.openDBFolderCommand == null)
                    this.openDBFolderCommand = new RelayCommand(() => this.OpenDBFolder());

                return this.openDBFolderCommand;
            }
        }

        private RelayCommand openBackupsFolderCommand;
        public RelayCommand OpenBackupsFolderCommand
        {
            get
            {
                if (this.openBackupsFolderCommand == null)
                    this.openBackupsFolderCommand = new RelayCommand(() => this.OpenBackupsFolder());

                return this.openBackupsFolderCommand;
            }
        }

        #endregion
    }
}