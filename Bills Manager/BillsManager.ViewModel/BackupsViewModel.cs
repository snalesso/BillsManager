using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using BillsManager.Model;
using BillsManager.Service;
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

        private ExtendedObservableCollection<BackupViewModel> backupViewModels;
        public ExtendedObservableCollection<BackupViewModel> BackupViewModels
        {
            get
            {
                if (this.backupViewModels == null)
                    this.backupViewModels = new ExtendedObservableCollection<BackupViewModel>();

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
            this.BackupViewModels = new ExtendedObservableCollection<BackupViewModel>(this.backupsProvider.GetAll()
                     .OrderByDescending(b => b.CreationTime)
                     .Select(b => new BackupViewModel(b)));
        }

        private void CreateNewBackup()
        {
            this.backupsProvider.CreateNew();
        }

        private void Rollback(Backup backup)
        {
            //if (this.dialogService.ShowYesNoQuestion
            //    ("Confirm rollback",
            //    "Are you sure you want to rollback to the following backup?" + "\r\n" + "\r\n" +
            //    p.CreationTime.ToLongDateString() + "\r\n" +
            //    p.CreationTime.ToLongTimeString()))

            var question = new DialogViewModel(
                "Rolling back",
                "Are you sure you want to ROLLBACK to the following backup?" +
                Environment.NewLine +
                Environment.NewLine + this.GetBackupInfo(backup),
                new[]
                {
                    // TODO: use a checkbox to speed up the confirm
                    new DialogResponse(ResponseType.Yes, 10),
                    new DialogResponse(ResponseType.No)
                });

            this.windowManager.ShowDialog(question);

            if (question.Response == ResponseType.Yes)
            {
                this.backupsProvider.Rollback(backup);
                this.LoadBackups();

                this.eventAggregator.Publish(new SuppliersNeedRefreshMessage());
                this.eventAggregator.Publish(new BillsNeedRefreshMessage());
            }
        }

        private void DeleteBackup(Backup backup)
        {
            //if (this.dialogService.ShowYesNoQuestion(
            //    "Confirm rollback",
            //    "Are you sure you want to delete the following backup?" + "\r\n" + "\r\n" +
            //        p.CreationTime.ToLongDateString() + "\r\n" +
            //        p.CreationTime.ToLongTimeString()))

            var question = new DialogViewModel(
                "Deleting backup",
                "Are you sure you want to DELETE the following backup?" +
                Environment.NewLine +
                Environment.NewLine + this.GetBackupInfo(backup),
                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, 15),
                                    new DialogResponse(ResponseType.No)
                                });

            this.windowManager.ShowDialog(question);

            if (question.Response == ResponseType.Yes)
            {
                this.backupsProvider.Delete(backup);
                this.LoadBackups();
            }
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
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\");
        }

        private void OpenDBFolder()
        {
            // TODO: do a check
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Database\");
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
                        this.CreateNewBackup();
                        this.LoadBackups();
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
                        p => this.Rollback(p.ExposedBackup),
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
                        p => this.DeleteBackup(p.ExposedBackup),
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