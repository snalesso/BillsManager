using BillsManager.Models;
using BillsManager.Services.Data;
using Caliburn.Micro;
using System;

namespace BillsManager.ViewModels
{
//#if DEBUG
    //public partial class DBBackupsViewModel : Screen
    //{
    //    #region ctor

    //    public DBBackupsViewModel()
    //    {
    //        if (Execute.InDesignMode)
    //        {
    //            this.LoadDesignTimeData();
    //        }
    //    }

    //    public DBBackupsViewModel(IBackupsProvider backupsProvider)
    //    {
    //        if (Execute.InDesignMode)
    //        {
    //            this.backupsProvider = backupsProvider;
    //            this.LoadDesignTimeData();
    //        }
    //    }

    //    #endregion

    //    #region methods

    //    void LoadDesignTimeData()
    //    {
    //        DateTime currentTime = DateTime.Now;

    //        this.BackupViewModels.Add(
    //            new BackupViewModel(
    //                new Backup(
    //                    AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.ToShortDateString() + " " + currentTime.ToShortTimeString() + ".bmb",
    //                    currentTime.AddDays(-80),
    //                    961,
    //                    38,
    //                    new[]
    //                    {
    //                        DateTime.Today.AddDays(-43),
    //                        DateTime.Today.AddDays(-31),
    //                        DateTime.Today.AddDays(-23),
    //                        DateTime.Today.AddDays(-19),
    //                        DateTime.Today.AddDays(-8)
    //                    })));

    //        this.BackupViewModels.Add(
    //            new BackupViewModel(
    //                new Backup(
    //                    AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.AddDays(-4).ToShortDateString() + " " + currentTime.AddDays(-4).ToShortTimeString() + ".bmb",
    //                    currentTime.AddDays(-4),
    //                    830,
    //                    31,
    //                    new DateTime[] { })));

    //        this.BackupViewModels.Add(
    //            new BackupViewModel(
    //                new Backup(
    //                    AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.AddDays(-10).ToShortDateString() + " " + currentTime.AddDays(-10).ToShortTimeString() + ".bmb",
    //                    currentTime.AddDays(-120),
    //                    680,
    //                    24,
    //                    new[]
    //                    {
    //                        DateTime.Today.AddDays(-91),
    //                        DateTime.Today.AddDays(-33),
    //                        DateTime.Today.AddDays(-13),
    //                        DateTime.Today.AddDays(-5),
    //                        DateTime.Today.AddDays(-4)
    //                    })));

    //        this.BackupViewModels.Add(
    //            new BackupViewModel(
    //                new Backup(
    //                    AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\" + currentTime.AddDays(-18).ToShortDateString() + " " + currentTime.AddDays(-18).ToShortTimeString() + ".bmb",
    //                    currentTime.AddDays(-18),
    //                    680,
    //                    19,
    //                    new DateTime[] { })));
    //    }

    //    #endregion
    //}
//#endif
}