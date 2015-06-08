using BillsManager.Services.Providers;
using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class DBViewModel
    {
        public DBViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.dbConnector = new DesignTimeDBConnector();
                this.LoadDesignTimeData();
            }
        }

        public DBViewModel(IDBConnector dbConnector)
        {
            if (Execute.InDesignMode)
            {
                this.dbConnector = dbConnector;
                this.LoadDesignTimeData();
            }
        }

        public void LoadDesignTimeData()
        {
            this.SuppliersViewModel = new SuppliersViewModel();
            this.BillsViewModel = new BillsViewModel();
        }

        private class DesignTimeDBConnector : IDBConnector
        {
            #region IDBConnector Members

            public bool Connect()
            {
                throw new NotImplementedException();
            }

            public bool Save()
            {
                throw new NotImplementedException();
            }

            public void Disconnect()
            {
                throw new NotImplementedException();
            }

            public string DBPath
            {
                get { return "Design time Path"; }
            }

            public string DBName
            {
                get { return "Design time Name"; }
            }

            #endregion

            #region IBillsProvider Members

            public uint GetLastBillID()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Models.Bill> GetAllBills()
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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

            //#region IAgentsProvider Members
            
            //public uint GetLastAgentID()
            //{
            //    throw new NotImplementedException();
            //}

            //public IEnumerable<Models.Agent> GetAllAgents()
            //{
            //    throw new NotImplementedException();
            //}

            //public Models.Agent GetAgentByID(uint agentID)
            //{
            //    throw new NotImplementedException();
            //}

            //public bool Add(Models.Agent agent)
            //{
            //    throw new NotImplementedException();
            //}

            //public bool Edit(Models.Agent agent)
            //{
            //    throw new NotImplementedException();
            //}

            //public bool Edit(IEnumerable<Models.Agent> agents)
            //{
            //    throw new NotImplementedException();
            //}

            //public bool Delete(Models.Agent agent)
            //{
            //    throw new NotImplementedException();
            //}

            //public bool Delete(IEnumerable<Models.Agent> agents)
            //{
            //    throw new NotImplementedException();
            //}

            //#endregion            
        }
    }
#endif
}