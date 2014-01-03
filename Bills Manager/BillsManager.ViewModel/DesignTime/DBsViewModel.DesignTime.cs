#if DEBUG

using BillsManager.Services.Providers;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class DBsViewModel
    {
        public DBsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.dbsProvider = new DesignTimeXMLDBsProvider();
                this.dbViewModelFactory = (s) => new DBViewModel(new DesignTimeXMLDBConnector(s));
                this.LoadDesignTimeData();
            }
        }

        private void LoadDesignTimeData()
        {
            this.RefreshDBsList();

            var openedDB = this.NotOpenedDBs.FirstOrDefault();
            this.NotOpenedDBs.Remove(openedDB);
            this.Items.Add(openedDB);

            this.ActiveItem = this.Items.FirstOrDefault();
            this.SelectedDB = this.NotOpenedDBs.FirstOrDefault();
        }

        private class DesignTimeXMLDBsProvider : IDBsProvider
        {
            #region IDBsProvider Members

            public string Location
            {
                get { return AppDomain.CurrentDomain.BaseDirectory + @"\Databases\"; }
            }

            public IEnumerable<string> GetAll()
            {
                return new[]
                        {
                            @"C:\Bills Manager\Databases\Spese.bmdb", /* C:\Bills Manager\Databases\ .bmdb */
                            @"C:\Bills Manager\Databases\Acquisti.bmdb",
                            @"C:\Bills Manager\Databases\Casa montagna.bmdb",
                            @"C:\Bills Manager\Databases\Casa mare.bmdb"
                        };
            }

            public bool CreateDB(string name)
            {
                throw new NotImplementedException();
            }

            public bool DeleteDB(string name)
            {
                throw new NotImplementedException();
            }

            public bool RenameDB(string oldName, string newName)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private class DesignTimeXMLDBConnector : IDBConnector
        {
            #region ctor

            public DesignTimeXMLDBConnector(string path)
            {
                this.path = path;
            }

            #endregion

            #region IDBConnector Members

            private readonly string path;
            public string Path
            {
                get { return this.path; }
            }

            public string DBName
            {
                get { return System.IO.Path.GetFileNameWithoutExtension(this.Path); }
            }

            public bool Open()
            {
                return true;
            }

            public bool Save()
            {
                return true;
            }

            public void Close()
            {
            }

            #endregion

            #region IBillsProvider Members

            public uint GetLastBillID()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Models.Bill> GetAllBills()
            {
                return null;
            }

            public bool Add(Models.Bill bill)
            {
                throw new NotImplementedException();
            }

            public bool Edit(Models.Bill bill)
            {
                throw new NotImplementedException();
            }

            public bool Edit(IEnumerable<Models.Bill> bills)
            {
                throw new NotImplementedException();
            }

            public bool Delete(Models.Bill bill)
            {
                throw new NotImplementedException();
            }

            public bool Delete(IEnumerable<Models.Bill> bills)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region ISuppliersProvider Members

            public uint GetLastSupplierID()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Models.Supplier> GetAllSuppliers()
            {
                return null;
            }

            public bool Add(Models.Supplier supplier)
            {
                throw new NotImplementedException();
            }

            public bool Edit(Models.Supplier supplier)
            {
                throw new NotImplementedException();
            }

            public bool Edit(IEnumerable<Models.Supplier> suppliers)
            {
                throw new NotImplementedException();
            }

            public bool Delete(Models.Supplier supplier)
            {
                throw new NotImplementedException();
            }

            public bool Delete(IEnumerable<Models.Supplier> suppliers)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IAgentsProvider Members

            public uint GetLastAgentID()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Models.Agent> GetAllAgents()
            {
                throw new NotImplementedException();
            }

            public Models.Agent GetAgentByID(uint agentID)
            {
                throw new NotImplementedException();
            }

            public bool Add(Models.Agent agent)
            {
                throw new NotImplementedException();
            }

            public bool Edit(Models.Agent agent)
            {
                throw new NotImplementedException();
            }

            public bool Edit(IEnumerable<Models.Agent> agents)
            {
                throw new NotImplementedException();
            }

            public bool Delete(Models.Agent agent)
            {
                throw new NotImplementedException();
            }

            public bool Delete(IEnumerable<Models.Agent> agents)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

    }
}

#endif