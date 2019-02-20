using BillsManager.Services;
using BillsManager.Services.Providers;
using Caliburn.Micro;

namespace BillsManager.ViewModels.Factories
{
    public class SuppliersViewModelFactory : IFactory<SuppliersViewModel>
    {
        #region fields

        private readonly ISuppliersProvider suppliersProvider;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        #endregion

        public SuppliersViewModelFactory(
            ISuppliersProvider suppliersProvider,
            IWindowManager windowManager,
            IEventAggregator eventAggregator) // TODO: lazy if the class becomes heavy
        {
            this.suppliersProvider = suppliersProvider;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
        }

        public SuppliersViewModel Create()
        {
            return new SuppliersViewModel(this.windowManager, this.eventAggregator, this.suppliersProvider);
        }
    }
}
