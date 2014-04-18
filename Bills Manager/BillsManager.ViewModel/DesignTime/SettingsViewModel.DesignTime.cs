using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public partial class SettingsViewModel
    {
        public SettingsViewModel()
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
}