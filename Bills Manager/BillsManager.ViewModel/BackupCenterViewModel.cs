using Caliburn.Micro;
using System;

namespace BillsManager.ViewModels
{
    public partial class BackupCenterViewModel : Conductor<DBBackupsViewModel>
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAgregator;
        //private readonly IDBsProvider dbsProvider;

        private readonly Func<DBBackupsViewModel> dbBackupsViewModelFactory;

        #endregion

        #region ctor

        public BackupCenterViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAgregator,
            //IDBsProvider dbsProvider,
            Func<DBBackupsViewModel> dbBackupsViewModelFactory)
        {
            // SERVICES
            this.windowManager = windowManager;
            this.globalEventAgregator = globalEventAgregator;
            //this.dbsProvider = dbsProvider;

            // FACTORIES
            this.dbBackupsViewModelFactory = dbBackupsViewModelFactory;

            // SUBSCRIPTIONS
            this.globalEventAgregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAgregator.Unsubscribe(this);
                };

            // UI
            this.DisplayName = "Backup Center"; // TODO: language

            // START
            this.ActivateItem(this.DBBackupsViewModel);
        }

        #endregion

        #region properties
        
        private DBBackupsViewModel dbBackupsViewModel;
        public DBBackupsViewModel DBBackupsViewModel
        {
            get
            {
                if (this.dbBackupsViewModel == null)
                    this.dbBackupsViewModel = this.dbBackupsViewModelFactory.Invoke();

                return this.dbBackupsViewModel;
            }
        }

        #endregion

        #region methods        
        #endregion
    }
}