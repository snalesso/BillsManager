using System;
using BillsManager.Model;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
#if DEBUG
    public partial class BackupsViewModel : Screen
    {
        #region ctor

        public BackupsViewModel()
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

            this.BackupViewModels.Add(
                new BackupViewModel(
                    new Backup(
                        AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.ToShortDateString() + " " + currentTime.ToShortTimeString() + ".bmb",
                        currentTime,
                        961,
                        38,
                        new[]
                        {
                            DateTime.Today.AddDays(-43),
                            DateTime.Today.AddDays(-31),
                            DateTime.Today.AddDays(-23),
                            DateTime.Today.AddDays(-19),
                            DateTime.Today.AddDays(-8)
                        })));

            this.BackupViewModels.Add(
                new BackupViewModel(
                    new Backup(
                        AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.AddDays(-4).ToShortDateString() + " " + currentTime.AddDays(-4).ToShortTimeString() + ".bmb",
                        currentTime.AddDays(-4),
                        830,
                        31,
                        new[]
                        {
                            DateTime.Today.AddDays(-43),
                            DateTime.Today.AddDays(-31),
                            DateTime.Today.AddDays(-23),
                            DateTime.Today.AddDays(-19),
                            DateTime.Today.AddDays(-8)
                        })));

            this.BackupViewModels.Add(
                new BackupViewModel(
                    new Backup(
                        AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.AddDays(-10).ToShortDateString() + " " + currentTime.AddDays(-10).ToShortTimeString() + ".bmb",
                        currentTime.AddDays(-10),
                        680,
                        24,
                        new[]
                        {
                            DateTime.Today.AddDays(-43),
                            DateTime.Today.AddDays(-31),
                            DateTime.Today.AddDays(-23),
                            DateTime.Today.AddDays(-19),
                            DateTime.Today.AddDays(-8)
                        })));

            this.BackupViewModels.Add(
                new BackupViewModel(
                    new Backup(
                        AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.AddDays(-18).ToShortDateString() + " " + currentTime.AddDays(-18).ToShortTimeString() + ".bmb",
                        currentTime.AddDays(-18),
                        680,
                        19,
                        new[]
                        {
                            DateTime.Today.AddDays(-43),
                            DateTime.Today.AddDays(-31),
                            DateTime.Today.AddDays(-23),
                            DateTime.Today.AddDays(-19),
                            DateTime.Today.AddDays(-8)
                        })));
        }

        #endregion
    }
#endif
}