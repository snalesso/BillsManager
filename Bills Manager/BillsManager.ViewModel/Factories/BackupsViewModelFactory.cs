using BillsManager.Service;
using BillsManager.Service.Providers;
using Caliburn.Micro;

namespace BillsManager.ViewModel.Factories
{
    public class BackupsViewModelFactory : IFactory<BackupsViewModel>
    {
        #region fields

        private readonly IBackupsProvider backupsProvider;
        //private readonly IDialogService dialogService;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        #endregion

        public BackupsViewModelFactory(
            IBackupsProvider backupsProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator) // TODO: lazy if the class becomes heavy
        {
            this.backupsProvider = backupsProvider;
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
        }

        public BackupsViewModel Create()
        {
            return new BackupsViewModel(this.backupsProvider, this.windowManager, /*this.dialogService,*/ this.eventAggregator);
        }
    }
}
