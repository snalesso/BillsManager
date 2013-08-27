using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        }

        #endregion

        #region properties

        private ObservableCollection<BackupViewModel> backupViewModels;
        public ObservableCollection<BackupViewModel> BackupViewModels
        {
            get
            {
                if (this.backupViewModels == null)
                    if (!Execute.InDesignMode) // TODO: separate dt
                    {
                        this.RefreshBackupsList(false);
                    }
                    // DESIGN TIME
                    else
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

        void RefreshBackupsList(bool notify = true) // TODO: show a busy indicator
        {
            this.backupViewModels = new ObservableCollection<BackupViewModel>(this.backupsProvider.GetAll().Select(b => new BackupViewModel(b)));
            if (notify) this.NotifyOfPropertyChange(() => this.BackupViewModels);
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
                        this.backupsProvider.CreateNew();
                        this.RefreshBackupsList();
                    });

                return this.createNewBackupCommand;
            }
        }

        private RelayCommand<BackupViewModel> rollbackBackupCommand;
        public RelayCommand<BackupViewModel> RollbackBackupCommand
        {
            get
            {
                if (this.rollbackBackupCommand == null)
                    this.rollbackBackupCommand = new RelayCommand<BackupViewModel>(
                        p =>
                        {
                            //if (this.dialogService.ShowYesNoQuestion
                            //    ("Confirm rollback",
                            //    "Are you sure you want to rollback to the following backup?" + "\r\n" + "\r\n" +
                            //    p.CreationTime.ToLongDateString() + "\r\n" +
                            //    p.CreationTime.ToLongTimeString()))

                            var question = new DialogViewModel(
                                "Rolling back",
                                "Are you sure you want to rollback to the following backup?" + "\r\n" + "\r\n" +
                                p.CreationTime.ToLongDateString() + "\r\n" +
                                p.CreationTime.ToLongTimeString(),
                                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, 15),
                                    new DialogResponse(ResponseType.No)
                                });

                            this.windowManager.ShowDialog(question);

                            if (question.Response == ResponseType.Yes)
                            {
                                this.backupsProvider.Rollback(p.ExposedBackup);
                                this.RefreshBackupsList();

                                this.eventAggregator.Publish(new SuppliersNeedRefreshMessage());
                                this.eventAggregator.Publish(new BillsNeedRefreshMessage());
                            }
                        },
                        p => p != null);

                return this.rollbackBackupCommand;
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
                            //    "Are you sure you want to delete the following backup?" + "\r\n" + "\r\n" +
                            //        p.CreationTime.ToLongDateString() + "\r\n" +
                            //        p.CreationTime.ToLongTimeString()))

                            var question = new DialogViewModel(
                                "Deleting backup",
                                "Are you sure you want to delete the following backup?" + "\r\n" + "\r\n" +
                                p.CreationTime.ToLongDateString() + "\r\n" +
                                p.CreationTime.ToLongTimeString(),
                                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, 15),
                                    new DialogResponse(ResponseType.No)
                                });
                            
                            this.windowManager.ShowDialog(question);

                            if (question.Response == ResponseType.Yes)
                            {
                                this.backupsProvider.Delete(p.ExposedBackup);
                                this.RefreshBackupsList();
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
                if (this.openDBFolderCommand == null) this.openDBFolderCommand = new RelayCommand(
                    () => Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Database\"));

                return this.openDBFolderCommand;
            }
        }

        private RelayCommand openBackupsFolderCommand;
        public RelayCommand OpenBackupsFolderCommand
        {
            get
            {
                if (this.openBackupsFolderCommand == null) this.openBackupsFolderCommand = new RelayCommand(
                    () => Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\"));

                return this.openBackupsFolderCommand;
            }
        }

        #endregion
    }
}