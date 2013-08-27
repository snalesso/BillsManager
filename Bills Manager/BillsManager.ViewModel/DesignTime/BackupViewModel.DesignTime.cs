using System;
using BillsManager.Model;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BackupViewModel : PropertyChangedBase
    {
        #region ctor

        public BackupViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        #endregion

        #region methods

        void LoadDesignTimeData()
        {
            DateTime currentTime = DateTime.Now;
            this.ExposedBackup = new Backup(
                AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.ToShortDateString() + " " + currentTime.ToShortTimeString() + ".bmb",
                currentTime,
                837,
                32,
                3);
        }

        #endregion
    }
}
