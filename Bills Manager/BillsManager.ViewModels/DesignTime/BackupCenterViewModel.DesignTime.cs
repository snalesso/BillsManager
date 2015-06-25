using BillsManager.Services.Data;
using Caliburn.Micro;
using System.Linq;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class BackupCenterViewModel
    {
        public BackupCenterViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
            //var db1 = new DBBackupsViewModel(new DesignTimeBackupsProvider("Negozio"));
            //var db2 = new DBBackupsViewModel(new DesignTimeBackupsProvider("Pelletteria"));
            //var db3 = new DBBackupsViewModel(new DesignTimeBackupsProvider("Casa"));

            //this.Items.Add(db1);
            //this.Items.Add(db2);
            //this.Items.Add(db3);

            //this.ActiveItem = this.Items.FirstOrDefault();
            //this.ActiveItem = db1;
        }

        private class DesignTimeBackupsProvider : IBackupsProvider
        {
            public DesignTimeBackupsProvider(string name)
            {
                this.dbName = name;
            }

            #region IBackupsProvider Members

            private readonly string dbName;
            public string DBName
            {
                get { return this.dbName; }
            }

            public string Location
            {
                get { return "Location - " + this.DBName; }
            }

            public System.Collections.Generic.IEnumerable<Models.Backup> GetAll()
            {
                throw new System.NotImplementedException();
            }

            public bool CreateNew()
            {
                throw new System.NotImplementedException();
            }

            public bool Rollback(Models.Backup backup)
            {
                throw new System.NotImplementedException();
            }

            public bool Delete(Models.Backup backup)
            {
                throw new System.NotImplementedException();
            }

            #endregion
        }
    }
#endif
}