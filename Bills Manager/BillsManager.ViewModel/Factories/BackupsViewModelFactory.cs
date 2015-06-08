using BillsManager.Services;
using BillsManager.Services.Providers;
using Caliburn.Micro;

namespace BillsManager.ViewModels.Factories
{
    public class BackupsViewModelFactory : IFactory<BackupsViewModel>
    {
        #region fields

        private readonly IBackupsProvider backupsProvider;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        #endregion

        public BackupsViewModelFactory(
            IBackupsProvider backupsProvider,
            IWindowManager windowManager,
            IEventAggregator eventAggregator) // TODO: lazy if the class becomes heavy
        {
            this.backupsProvider = backupsProvider;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
        }

        public BackupsViewModel Create()
        {
            return new BackupsViewModel(this.windowManager, this.eventAggregator, this.backupsProvider);
        }
    }
}
