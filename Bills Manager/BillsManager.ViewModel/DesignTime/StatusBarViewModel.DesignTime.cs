using Caliburn.Micro;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class StatusBarViewModel
    {
        public StatusBarViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
        }
    }
#endif
}