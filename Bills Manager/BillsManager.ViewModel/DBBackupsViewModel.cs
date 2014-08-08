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
            var all = this.backupsProvider.GetAll();

            var allSorted = all.OrderByDescending(b => b.CreationTime);

            var vms = allSorted.Select(b => new BackupViewModel(b)); // TODO: factory?

            this.BackupViewModels = new ObservableCollection<BackupViewModel>(vms);
        }

        private void CreateBackup()
        {
            var question = new DialogViewModel(
                TranslationManager.Instance.Translate("CreateBackup").ToString(),
                TranslationManager.Instance.Translate("CreateBackupQuestion").ToString() +
                Environment.NewLine +
                TranslationManager.Instance.Translate("OperationMayTakeAWhile").ToString(),
                new[]
                {
                    new DialogResponse(ResponseType.Yes, TranslationManager.Instance.Translate("CreateBackup").ToString()),
                    new DialogResponse(ResponseType.No, TranslationManager.Instance.Translate("No").ToString())
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
            var rollbackQuestion = new DialogViewModel(
                            TranslationManager.Instance.Translate("Rollback").ToString(),
                            TranslationManager.Instance.Translate("ConfirmRollbackQuestion").ToString() +
                            Environment.NewLine +
                            Environment.NewLine + this.GetBackupInfo(backupViewModel.ExposedBackup),
                            new[]
                            {
                                new DialogResponse(
                                    ResponseType.Yes, 
                                    TranslationManager.Instance.Translate("Rollback").ToString(), 
                                    TranslationManager.Instance.Translate("Yes").ToString()),
                                new DialogResponse(
                                    ResponseType.No,
                                    TranslationManager.Instance.Translate("No").ToString())
                            });

            this.windowManager.ShowDialog(rollbackQuestion);

            // URGENT: rollback failed protection

            if (rollbackQuestion.FinalResponse == ResponseType.Yes)
            {
                this.globalEventAggregator.Publish(
                    new RollbackAuthorizationRequest(
                        () =>
                        {
                            if (this.backupsProvider.Rollback(backupViewModel.ExposedBackup))
                            {
                                this.RefreshBackups();
                                this.windowManager.ShowDialog(
                                    new DialogViewModel(
                                        TranslationManager.Instance.Translate("RollbackCompleted").ToString(),
                                        TranslationManager.Instance.Translate("RollbackCompletedMessage").ToString()));
                            }
                            else
                                this.windowManager.ShowDialog(
                                    new DialogViewModel(
                                        TranslationManager.Instance.Translate("RollbackFailed").ToString(),
                                        TranslationManager.Instance.Translate("RollbackFailedMessage").ToString()));
                        },
                        () =>
                        {
                            this.windowManager.ShowDialog(
                                new DialogViewModel(
                                    TranslationManager.Instance.Translate("RollbackCanceled").ToString(),
                                    TranslationManager.Instance.Translate("RollbackCanceledMessage").ToString()));
                        }));
            }
        }

        private void DeleteBackup(BackupViewModel backupViewModel)
        {
            var question = new DialogViewModel(
                TranslationManager.Instance.Translate("DeleteBackup").ToString(),
                TranslationManager.Instance.Translate("DeleteBackupQuestion").ToString() +
                Environment.NewLine +
                Environment.NewLine +
                this.GetBackupInfo(backupViewModel.ExposedBackup),
                new[]
                {
                    new DialogResponse(
                        ResponseType.Yes,
                        TranslationManager.Instance.Translate("Delete").ToString(),
                        TranslationManager.Instance.Translate("Yes").ToString()),
                    new DialogResponse(
                        ResponseType.No,
                        TranslationManager.Instance.Translate("No").ToString())
                });

            this.windowManager.ShowDialog(question);

            if (question.FinalResponse == ResponseType.Yes)
            {
                this.backupsProvider.Delete(backupViewModel.ExposedBackup);
                this.RefreshBackups();
            }
        }

        private string GetBackupInfo(Backup backup)
        {
            // TODO: review informations layout
            return backup.CreationTime.ToLongDateString() + "   " + backup.CreationTime.ToLongTimeString() +
                   Environment.NewLine +
                   Environment.NewLine +
                   backup.BillsCount + " " +
                   TranslationManager.Instance.Translate("Bills").ToString().ToLower(TranslationManager.Instance.CurrentLanguage) +
                   Environment.NewLine +
                   backup.SuppliersCount + " " +
                   TranslationManager.Instance.Translate("Suppliers").ToString().ToLower(TranslationManager.Instance.CurrentLanguage);
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
                        () => this.OpenBackupsFolder());

                return this.openBackupsFolderCommand;
            }
        }

        #endregion
    }
}