using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillsManager.Models;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace BillsManager.Services.DB
{
    public class LINQToDBDBService : IDBService, IDisposable
    {
        public const string DBFileName = "db.sqlite";

        //private LinqToDB.DataProvider.SQLite.SQLiteTools xx = null;
        //private readonly LinqToDB.DataProvider.SQLite.SQLiteDataProvider _dbConnection;
        private readonly ConnectionStringSettings _connectionString = new ConnectionStringSettings
        {
            Name = "Database",
            ProviderName = LinqToDB.ProviderName.SQLite,
            ConnectionString = "Data Source=" + DBFileName
        };
        private readonly IDataProvider _dataProvider;

        private DataConnection _dataConnection;
        private uint? _lastBillID;
        private uint? _lastSupplierID;


        public LINQToDBDBService()
        {
            //this._dataProvider = new LinqToDB.DataProvider.SQLite.SQLiteDataProvider();
        }

        public bool Add(Bill bill)
        {
            return this._dataConnection.Insert(bill) > 0;
        }

        public bool Add(Supplier supplier)
        {
            return this._dataConnection.Insert(supplier) > 0;
        }

        public bool Connect()
        {
            if (this._dataConnection == null)
            {
                this._dataConnection = LinqToDB.DataProvider.SQLite.SQLiteTools.CreateDataConnection(this._connectionString.ConnectionString);
                //this._dataConnection.Connection.Open();

                return this._dataConnection != null;
            }


            switch (this._dataConnection.Connection.State)
            {
                case ConnectionState.Open:
                    return true;

                case ConnectionState.Closed:
                case ConnectionState.Broken:
                    this._dataConnection.Connection.Open();
                    return true;

                case ConnectionState.Connecting:
                case ConnectionState.Fetching:
                case ConnectionState.Executing:
                    return true;

                default:
                    return true;
            }

        }

        public bool Delete(Bill bill)
        {
            return this._dataConnection.Delete(bill) > 0;
        }

        public bool Delete(Supplier supplier)
        {
            return this._dataConnection.Delete(supplier) > 0;
        }

        public void Disconnect()
        {
            if (this._dataConnection != null)
            {
                this._dataConnection.Close();
            }
        }

        public void Dispose()
        {
            if (this._dataConnection != null)
            {
                this._dataConnection.Close();
                this._dataConnection.Dispose();
                //this._dbConnection = null;
            }
        }

        public bool Edit(Bill bill)
        {
            return this._dataConnection.Update<Bill>(bill) > 0;
        }

        public bool Edit(Supplier supplier)
        {
            return this._dataConnection.Update<Supplier>(supplier) > 0;
        }

        public IEnumerable<Bill> GetAllBills()
        {
            return this._dataConnection.GetTable<Bill>().ToList().AsReadOnly();
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            return this._dataConnection.GetTable<Supplier>().ToList().AsReadOnly();
        }

        public uint GetLastBillID()
        {
            return this._dataConnection.GetTable<Bill>().Max(b => b.ID);
        }

        public uint GetLastSupplierID()
        {
            return this._dataConnection.GetTable<Supplier>().Max(s => s.ID);
        }

        public bool Save()
        {
            throw new NotSupportedException();
        }
    }
}
