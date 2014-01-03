using BillsManager.Services.Providers;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class DBsViewModel : Conductor<DBViewModel>.Collection.OneActive,
        IHandle<RollbackAuthorizationRequestMessage>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDBsProvider dbsProvider;

        private readonly Func<string, DBViewModel> dbViewModelFactory;
        private readonly Func<IEnumerable<string>, string, DBAddEditViewModel> dbAddViewModelFactory;

        #endregion

        #region ctor

        public DBsViewModel(
            IWindowManager windowManager,
            IEventAggregator eventAggregator,
            IDBsProvider dbsProvider,
            Func<string, DBViewModel> dbViewModelFactory,
            Func<IEnumerable<string>, string, DBAddEditViewModel> dbAddEditViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.dbsProvider = dbsProvider;

            // FACTORIES
            this.dbViewModelFactory = dbViewModelFactory;
            this.dbAddViewModelFactory = dbAddEditViewModelFactory;

            // SUBSCRIPTIONS
            this.eventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed) this.eventAggregator.Unsubscribe(this);
                };

            this.ActivationProcessed +=
                (s, e) =>
                {
                    this.eventAggregator.Publish(new ActiveDBChangedMessage(this.ActiveItem));
                };

            // START
            this.RefreshDBsList();
        }

        #endregion

        #region properties

        private ObservableCollection<DBViewModel> notOpenedDBs; // TODO: readonly property that filter from all dbs those that are in this.items?
        public ObservableCollection<DBViewModel> NotOpenedDBs
        {
            get
            {
                if (this.notOpenedDBs == null)
                {
                    this.notOpenedDBs = new ObservableCollection<DBViewModel>();
                }

                return this.notOpenedDBs;
            }
            //private set
            //{
            //    if (this.notOpenedDBs == value) return;

            //    this.notOpenedDBs = value;
            //    this.NotifyOfPropertyChange(() => this.NotOpenedDBs);

            //}
        }

        private DBViewModel selectedDB;
        public DBViewModel SelectedDB
        {
            get { return this.selectedDB; }
            set
            {
                if (this.selectedDB == value) return;

                this.selectedDB = null;
                this.NotifyOfPropertyChange(() => this.SelectedDB);

                //if (value == null) return;

                this.selectedDB = value;
                this.NotifyOfPropertyChange(() => this.SelectedDB);

                if (!this.NotOpenedDBs.Contains(value))
                {
                    if (value != this.ActiveItem)
                        this.ActiveItem = value;
                }
            }
        }

        public override void CanClose(Action<bool> callback)
        {
            //base.CanClose(callback);

            //foreach (var dbvm in this.Items.ToList()) // URGENT: dbs can be gc'd if ram is oom
            //{
            //    if (!dbvm.IsConnected)
            //        this.MoveToNotOpened(dbvm);
            //}

            //if (this.ActiveItem != null & !this.Items.Contains(this.ActiveItem))
            //{
            //    if (this.SelectedDB != null & !this.NotOpenedDBs.Contains(this.SelectedDB))
            //    {
            //        this.SelectedDB = this.Items.FirstOrDefault();
            //    }
            //    else
            //        this.ActiveItem = this.Items.FirstOrDefault();
            //}

            CloseStrategy.Execute(this.Items.ToList(), (canClose, closeables) =>
            {
                if (!canClose && closeables.Any())
                {
                    if (closeables.Contains(this.ActiveItem))
                    {
                        var itemsCopy = this.Items.ToList();
                        var next = this.ActiveItem;

                        do
                        {
                            var previous = next;
                            next = this.DetermineNextItemToActivate(itemsCopy, itemsCopy.IndexOf(previous));
                            itemsCopy.Remove(previous);
                        }
                        while (closeables.Contains(next));

                        var previousActive = this.ActiveItem;
                        ChangeActiveItem(next, true);
                        //this.Items.Remove(previousActive);
                        this.MoveToNotOpened(previousActive);

                        var stillToClose = closeables.ToList();
                        stillToClose.Remove(previousActive);
                        closeables = stillToClose;
                    }

                    closeables.OfType<IDeactivate>().Apply(x => x.Deactivate(true));
                    //this.Items.RemoveRange(closeables);
                    closeables.Apply(c => this.MoveToNotOpened(c));
                }

                callback(canClose);
            });

        }

        #endregion

        #region methods

        private void RefreshDBsList()
        {
            var availableDBPaths = this.dbsProvider.GetAll();

            // remove the dbs that are loaded (and not opened) but no longer exist
            var loadedDBPaths = this.NotOpenedDBs.Select(dbvm => dbvm.Path).Concat(this.Items.Select(dbvm => dbvm.Path));
            var disappearedDBs = loadedDBPaths.Where(path => !availableDBPaths.Contains(path));

            foreach (var disDBPath in disappearedDBs.ToList())
            {
                var toRemoveDB = this.NotOpenedDBs.Single(dbvm => dbvm.Path == disDBPath);
                toRemoveDB.TryClose();
                this.NotOpenedDBs.Remove(toRemoveDB);
            }

            // add newly found dbs to the notopenedbs list
            var toAddDBPaths = availableDBPaths.Where(avDBPath => !loadedDBPaths.Contains(avDBPath));
            toAddDBPaths.Apply(newDBPath => this.NotOpenedDBs.Add(this.dbViewModelFactory.Invoke(newDBPath)));
        }

        #region CRUD

        private void CreateDB()
        {
            var takenNames = this.Items.Select(dbvm => dbvm.DBName).Concat(this.NotOpenedDBs.Select(dbvm => dbvm.DBName));
            var newDBVM = this.dbAddViewModelFactory.Invoke(takenNames, null);

            if (this.windowManager.ShowDialog(newDBVM).Value == true)
            {
                if (this.dbsProvider.CreateDB(newDBVM.NewDBName.Trim())) // TODO: move trim to adddbvm while typing
                {
                    this.RefreshDBsList();
                    var justAddedDB = this.NotOpenedDBs.Single(dbvm => dbvm.DBName == newDBVM.NewDBName);
                    if (justAddedDB != null && newDBVM.AddAndOpen)
                        this.OpenDB(justAddedDB);
                }
                else
                    this.windowManager.ShowDialog(
                        new DialogViewModel(
                            "Create database failed", // TODO: language
                            "There was a problem during the DB creation process. Please try again."));
            }
        }

        // TODO: create parallel db loading while using another one
        private void OpenDB(DBViewModel dbViewModel)
        {
            if (!this.NotOpenedDBs.Contains(dbViewModel)) return;

            if (dbViewModel.TryConnectDB())
            {
                var wasSelected = this.SelectedDB == dbViewModel;

                this.MoveToOpened(dbViewModel);

                if (this.Items.Contains(this.SelectedDB) | wasSelected | !this.NotOpenedDBs.Contains(this.SelectedDB))
                    this.SelectedDB = dbViewModel;
                else
                    this.ActiveItem = dbViewModel;

                this.eventAggregator.Publish(new ActiveDBChangedMessage(this.ActiveItem));
            }
        }

        private void CloseDB(DBViewModel dbViewModel)
        {
            if (!this.Items.Contains(dbViewModel)) return;

            if (dbViewModel.TryDisconnectDB())
            {
                var wasActive = this.ActiveItem == dbViewModel;
                var wasSelected = this.SelectedDB == this.ActiveItem;
                if (this.SelectedDB == dbViewModel)
                    this.SelectedDB = null;
                else
                    if (dbViewModel == this.ActiveItem)
                        this.ActiveItem = null;

                this.MoveToNotOpened(dbViewModel);

                if (wasActive)
                {
                    var newActive = this.Items.LastOrDefault();
                    if (wasSelected)
                        this.SelectedDB = newActive;
                    else
                        this.ActiveItem = newActive;
                }

                this.eventAggregator.Publish(new ActiveDBChangedMessage(this.ActiveItem));
            }
        }

        private void RenameDB(DBViewModel dbViewModel)
        {
            var takenNames = this.Items.Select(dbvm => dbvm.DBName).Concat(this.NotOpenedDBs.Select(dbvm => dbvm.DBName));
            var renameDBVM = this.dbAddViewModelFactory.Invoke(takenNames, dbViewModel.DBName);
            var oldName = dbViewModel.DBName;

            if (this.windowManager.ShowDialog(renameDBVM).Value == true)
            {
                if (this.dbsProvider.RenameDB(oldName, renameDBVM.NewDBName.Trim())) // TODO: move trim to adddbvm while typing
                    this.RefreshDBsList();
                else
                    this.windowManager.ShowDialog(
                        new DialogViewModel(
                            "Rename database failed", // TODO: language
                            "There was a problem during the DB renaming process, please try again."));
            }
        }

        private void DeleteDB(DBViewModel dbViewModel)
        {
            var firstConfirmDialog = new DialogViewModel( // TODO: language
                "Delete database",
                "Are you sure you want to delete database '" + dbViewModel.DBName + "' ?" +
                Environment.NewLine +
                Environment.NewLine +
                "WARNING: the database will be deleted permanently, " +
                "with no chance to revert the operation.",
                new[] { new DialogResponse(ResponseType.Yes, "DELETE", "Delete '" + dbViewModel.DBName + "'"), new DialogResponse(ResponseType.Cancel) });

            this.windowManager.ShowDialog(
                firstConfirmDialog/*,
                settings: new Dictionary<string, object> { { "MaxWidth", 500 } }*/);

            if (firstConfirmDialog.FinalResponse == ResponseType.Yes) // TODO: dialogView needs a width limit
            {
                var secondConfirmDialog =
                    new DialogViewModel(
                    "Delete database final confirm", // TODO: language
                    "Do you confirm you want to delete database '" + dbViewModel.DBName + "' ?" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "WARNING: the database will be deleted permanently, " +
                    "with no chance to revert the operation.",
                    new[] { new DialogResponse(ResponseType.Yes, "DELETE", "Delete '" + dbViewModel.DBName + "'"), new DialogResponse(ResponseType.Cancel) });

                this.windowManager.ShowDialog(secondConfirmDialog);

                if (secondConfirmDialog.FinalResponse == ResponseType.Yes)
                {
                    if (this.dbsProvider.DeleteDB(dbViewModel.DBName))
                    {
                        this.CloseDB(dbViewModel);

                        this.RefreshDBsList();
                    }
                    else
                        this.windowManager.ShowDialog(
                            new DialogViewModel(
                                "Database delete failed", // TODO: language
                                "Database '" + dbViewModel.DBName + "' couldn't be deleted because an error occurred." +
                                Environment.NewLine +
                                "Please try again."));
                }
            }
        }

        #endregion

        #region utilities

        private void MoveToOpened(DBViewModel dbViewModel)
        {
            this.NotOpenedDBs.Remove(dbViewModel);
            var i = this.GetInsertIndex(dbViewModel, this.Items);
            this.Items.Insert(i, dbViewModel);
        }

        private void MoveToNotOpened(DBViewModel dbViewModel)
        {
            this.Items.Remove(dbViewModel);
            var i = this.GetInsertIndex(dbViewModel, this.NotOpenedDBs);
            this.NotOpenedDBs.Insert(i, dbViewModel);
        }

        private int GetInsertIndex(DBViewModel dbvm, IList<DBViewModel> dbvmList)
        {
            if (dbvm == null)
                throw new ArgumentNullException("dbvm cannot be null");

            if (dbvmList == null)
                throw new ArgumentNullException("dbvmList cannot be null");

            if (dbvmList.Count == 0)
                return 0;

            var names = dbvmList.Select(vm => vm.DBName).Concat(new[] { dbvm.DBName });
            var sortedNames = names.OrderBy(n => n);
            return sortedNames.ToList().IndexOf(dbvm.DBName);
        }

        #endregion

        #region message handlers

        public void Handle(RollbackAuthorizationRequestMessage rollbackAuthorizationRequest)
        {
            if (!this.Items.Select(dbvm => dbvm.DBName).Contains(rollbackAuthorizationRequest.DBName))
                rollbackAuthorizationRequest.ConfirmAuthorization();
            else
                rollbackAuthorizationRequest.NegateAuthorization();
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand createDBCommand;
        public RelayCommand CreateDBCommand
        {
            get
            {
                if (this.createDBCommand == null)
                    this.createDBCommand = new RelayCommand(
                        () => this.CreateDB());

                return this.createDBCommand;
            }
        }

        private RelayCommand<DBViewModel> openDBCommand;
        public RelayCommand<DBViewModel> OpenDBCommand
        {
            get
            {
                if (this.openDBCommand == null)
                    this.openDBCommand = new RelayCommand<DBViewModel>(
                        p => this.OpenDB(p),
                        p => p != null);

                return this.openDBCommand;
            }
        }

        private RelayCommand<DBViewModel> closeDBCommand;
        public RelayCommand<DBViewModel> CloseDBCommand
        {
            get
            {
                if (this.closeDBCommand == null)
                    this.closeDBCommand = new RelayCommand<DBViewModel>(
                        p => this.CloseDB(p),
                        p => p != null);

                return this.closeDBCommand;
            }
        }

        private RelayCommand<DBViewModel> deleteDBCommand;
        public RelayCommand<DBViewModel> DeleteDBCommand
        {
            get
            {
                if (this.deleteDBCommand == null)
                    this.deleteDBCommand = new RelayCommand<DBViewModel>(
                        p => this.DeleteDB(p),
                        p => this.NotOpenedDBs.Contains(p));

                return this.deleteDBCommand;
            }
        }

        private RelayCommand<DBViewModel> renameDBCommand;
        public RelayCommand<DBViewModel> RenameDBCommand
        {
            get
            {
                if (this.renameDBCommand == null)
                    this.renameDBCommand = new RelayCommand<DBViewModel>(
                        p => this.RenameDB(p),
                        p => this.NotOpenedDBs.Contains(p));

                return this.renameDBCommand;
            }
        }

        #endregion
    }
}