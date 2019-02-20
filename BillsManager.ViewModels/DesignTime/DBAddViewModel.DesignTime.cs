using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class DBAddEditViewModel
    {
        public DBAddEditViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.takenDBNames = new[]
                {
                    @"C:\Bills Manager\Databases\Spese.bmdb", /* C:\Bills Manager\Databases\ .bmdb */
                    @"C:\Bills Manager\Databases\Acquisti.bmdb",
                    @"C:\Bills Manager\Databases\Casa montagna.bmdb",
                    @"C:\Bills Manager\Databases\Casa mare.bmdb"
                };

                this.LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
            this.NewDBName = "This is the new name!";
        }
    }
#endif
}