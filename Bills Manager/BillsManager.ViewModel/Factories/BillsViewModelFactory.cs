using BillsManager.Service;
using BillsManager.Service.Providers;
using Caliburn.Micro;

namespace BillsManager.ViewModel.Factories
{
    public class BillsViewModelFactory : IFactory<BillsViewModel>
    {
        #region fields

        private readonly IBillsProvider billsProvider;
        private readonly IWindowManager windowManager;
        //private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        #endregion

        public BillsViewModelFactory(
            IBillsProvider billsProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator) // TODO: lazy if the class becomes heavy
        {
            this.billsProvider = billsProvider;
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
        }

        public BillsViewModel Create()
        {
            return new BillsViewModel(this.billsProvider, this.windowManager, /*this.dialogService,*/ this.eventAggregator);
        }
    }
}
