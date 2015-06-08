using BillsManager.Localization;
using BillsManager.Models;
using BillsManager.Services.Providers;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BillsManager.ViewModels
{
    // TODO: make conductor?
    public partial class DBBackupsViewModel : Screen
    {
        #region fields

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
                        this.globalEventAggregator.Unsubscribe(this);
                };

            // START
            this.RefreshBackups();
        }

        #endregion

        #region properties

        private ObservableCollection<BackupViewModel> backupViewModels;
        public ObservableCollection<BackupViewModel> BackupViewModels
        {
            get
            {
                return this.backupViewModels ?? (this.backupViewModels = new ObservableCollection<BackupViewModel>());
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
            var all = this.backupsProvider.GetAll();

            var allSorted = all.OrderByDescending(b => b.CreationTime);

            var vms = allSorted.Select(b => new BackupViewModel(b)); // IDEA: factory?

            this.BackupViewModels = new ObservableCollection<BackupViewModel>(vms);
        }

        private void CreateBackup()
        {
            DialogViewModel createBackupConfirmDialog =
                DialogViewModel.Show(
                    DialogType.Question,
                    TranslationManager.Instance.Translate("CreateBackup"),
                    TranslationManager.Instance.Translate("CreateBackupQuestion") +
                    Environment.NewLine +
                    TranslationManager.Instance.Translate("OperationMayTakeAWhile"))
                .YesNo(
                    TranslationManager.Instance.Translate("CreateBackup"),
                    TranslationManager.Instance.Translate("No"));

            this.windowManager.ShowDialog(createBackupConfirmDialog);

            if (createBackupConfirmDialog.FinalResponse == ResponseType.Yes)
            {
                this.backupsProvider.CreateNew();
                this.RefreshBackups();
            }
        }

        private void Rollback(BackupViewModel backupViewModel)
        {
            DialogViewModel rollbackConfirmDialog =
                DialogViewModel.Show(
                    DialogType.Question,
                    TranslationManager.Instance.Translate("Rollback"),
                    TranslationManager.Instance.Translate("ConfirmRollbackQuestion") +
                    Environment.NewLine +
                    Environment.NewLine + this.GetBackupInfo(backupViewModel.ExposedBackup))
                .YesNo(
                    TranslationManager.Instance.Translate("Rollback"),
                    TranslationManager.Instance.Translate("No"));

            this.windowManager.ShowDialog(rollbackConfirmDialog);

            // URGENT: rollback failed protection

            if (rollbackConfirmDialog.FinalResponse == ResponseType.Yes)
            {
                this.globalEventAggregator.PublishOnUIThread(
                    new RollbackAuthorizationRequest(
                        () =>
                        {
                            if (this.backupsProvider.Rollback(backupViewModel.ExposedBackup))
                            {
                                this.RefreshBackups();
                                this.windowManager.ShowDialog(
                                    DialogViewModel.Show(
                                        DialogType.Information,
                                        TranslationManager.Instance.Translate("RollbackCompleted"),
                                        TranslationManager.Instance.Translate("RollbackCompletedMessage"))
                                    .Ok());
                            }
                            else
                                this.windowManager.ShowDialog(
                                    DialogViewModel.Show(
                                        DialogType.Error,
                                        TranslationManager.Instance.Translate("RollbackFailed"),
                                        TranslationManager.Instance.Translate("RollbackFailedMessage"))
                                    .Ok());
                        },
                        () =>
                        {
                            this.windowManager.ShowDialog(
                                DialogViewModel.Show(
                                    DialogType.Information,
                                    TranslationManager.Instance.Translate("RollbackCanceled"),
                                    TranslationManager.Instance.Translate("RollbackCanceledMessage"))
                                .Ok());
                        }));
            }
        }

        private void DeleteBackup(BackupViewModel backupViewModel)
        {
            DialogViewModel deleteBackupConfirmDialog =
                DialogViewModel.Show(
                    DialogType.Question,
                    TranslationManager.Instance.Translate("DeleteBackup"),
                    TranslationManager.Instance.Translate("DeleteBackupQuestion") +
                    Environment.NewLine +
                    Environment.NewLine +
                    this.GetBackupInfo(backupViewModel.ExposedBackup))
                .YesNo();

            this.windowManager.ShowDialog(deleteBackupConfirmDialog);

            if (deleteBackupConfirmDialog.FinalResponse == ResponseType.Yes)
            {
                this.backupsProvider.Delete(backupViewModel.ExposedBackup);
                this.RefreshBackups();
            }
        }

        private string GetBackupInfo(Backup backup)
        {
            return backup.CreationTime.ToLongDateString() + "   " + backup.CreationTime.ToLongTimeString() +
                   Environment.NewLine +
                   Environment.NewLine +
                   backup.BillsCount + " " +
                   TranslationManager.Instance.Translate("Bills").ToLower(TranslationManager.Instance.CurrentLanguage) +
                   Environment.NewLine +
                   backup.SuppliersCount + " " +
                   TranslationManager.Instance.Translate("Suppliers").ToLower(TranslationManager.Instance.CurrentLanguage);
        }

        private void OpenBackupsFolder()
        {
            if (Directory.Exists(this.backupsProvider.Location))
                Process.Start(this.backupsProvider.Location);
        }

        #endregion

        #region commands

        private RelayCommand createNewBackupCommand;
        public RelayCommand CreateNewBackupCommand
        {
            get
            {
                return this.createNewBackupCommand ?? (this.createNewBackupCommand =
                    new RelayCommand(
                        () => this.CreateBackup()));
            }
        }

        private RelayCommand<BackupViewModel> rollbackCommand;
        public RelayCommand<BackupViewModel> RollbackCommand
        {
            get
            {
                return this.rollbackCommand ?? (this.rollbackCommand =
                    new RelayCommand<BackupViewModel>(
                        p => this.Rollback(p),
                        p => p != null));

            }
        }

        private RelayCommand<BackupViewModel> deleteBackupCommand;
        public RelayCommand<BackupViewModel> DeleteBackupCommand
        {
            get
            {
                return this.deleteBackupCommand ?? (this.deleteBackupCommand =
                    new RelayCommand<BackupViewModel>(
                        p => this.DeleteBackup(p),
                        p => p != null));
            }
        }

        private RelayCommand openBackupsFolderCommand;
        public RelayCommand OpenBackupsFolderCommand
        {
            get
            {
                return this.openBackupsFolderCommand ?? (this.openBackupsFolderCommand =
                    new RelayCommand(
                        () => this.OpenBackupsFolder()));
            }
        }

        #endregion
    }
}