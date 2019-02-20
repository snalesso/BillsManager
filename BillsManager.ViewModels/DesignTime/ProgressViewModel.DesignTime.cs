using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public partial class ProgressViewModel
    {
        public ProgressViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.message = "Loading ...";
                this.LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
        }
    }
}