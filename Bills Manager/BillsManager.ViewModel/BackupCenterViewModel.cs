using BillsManager.Services.Providers;
using Caliburn.Micro;
using System;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class BackupCenterViewModel : Conductor<DBBackupsViewModel>.Collection.OneActive
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAgregator;
        private readonly IDBsProvider dbsProvider;

        private readonly Func<string, DBBackupsViewModel> dbBackupsViewModelFactory;

        #endregion

        #region ctor

        public BackupCenterViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAgregator,
            IDBsProvider dbsProvider,
            Func<string, DBBackupsViewModel> dbBackupsViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAgregator = globalEventAgregator;
            this.dbsProvider = dbsProvider;

            // FACTORIES
            this.dbBackupsViewModelFactory = dbBackupsViewModelFactory;

            // SUBSCRIPTIONS
            this.globalEventAgregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    this.globalEventAgregator.Unsubscribe(this);
                };

            // UI
            this.DisplayName = "Backup Center"; // TODO: language

            // START
            this.LoadBackupGroups();
        }

        #endregion

        #region methods

        private void LoadBackupGroups()
        {
            var all = this.dbsProvider.GetAll();

            var range = all.Select(dbLocation => this.dbBackupsViewModelFactory(dbLocation));

            this.Items.AddRange(range);

            this.ActivateItem(this.Items.FirstOrDefault());

            // TODO: load first, async load of the others
        }

        #endregion
    }
}