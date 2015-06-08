using System;
using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class BackupViewModel
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
                new[]
                {
                    DateTime.Today.AddDays(-43),
                    DateTime.Today.AddDays(-31),
                    DateTime.Today.AddDays(-23),
                    DateTime.Today.AddDays(-19),
                    DateTime.Today.AddDays(-8)
                });
        }

        #endregion
    }
#endif
}