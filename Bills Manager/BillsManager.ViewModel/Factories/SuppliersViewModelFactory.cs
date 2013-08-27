using BillsManager.Service;
using BillsManager.Service.Providers;
using Caliburn.Micro;

namespace BillsManager.ViewModel.Factories
{
    public class SuppliersViewModelFactory : IFactory<SuppliersViewModel>
    {
        #region fields

        private readonly ISuppliersProvider suppliersProvider;
        private readonly IWindowManager windowManager;
        //private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        #endregion

        public SuppliersViewModelFactory(
            ISuppliersProvider suppliersProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator) // TODO: lazy if the class becomes heavy
        {
            this.suppliersProvider = suppliersProvider;
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
        }

        public SuppliersViewModel Create()
        {
            return new SuppliersViewModel(this.suppliersProvider, this.windowManager, /*this.dialogService,*/ this.eventAggregator);
        }
    }
}
