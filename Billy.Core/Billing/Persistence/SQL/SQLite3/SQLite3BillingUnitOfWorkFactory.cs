﻿using Billy.Billing.Persistence.SQL.SQLite3.Dapper;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public class SQLite3BillingUnitOfWorkFactory : IBillingUnitOfWorkFactory //: IDisposable
    {
        //private readonly IConnectionFactory<SqlConnection> _connectionFactory;
        private SQLiteConnection _connection;

        public SQLite3BillingUnitOfWorkFactory()
        {
        }

        //public SQLite3BillingUnitOfWorkFactory(IConnectionFactory<SqlConnection> connectionFactory)
        //{
        //    this._connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        //}

        private ISuppliersRepository GetSuppliersRepository(SQLiteConnection conn, SQLiteTransactionBase tran)
        {
            // TODO: make implementation agnostic
            return new DapperSQLite3SuppliersRepository(conn, tran);
        }

        public async Task<IBillingUnitOfWork> CreateAsync()
        {
            if (this._connection == null)
            {
                this._connection = new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString);
                await this._connection.OpenAsync();

                // TODO: execute only when needed
                await this.EnsureSchemaAsync();
            }

            return new SQLite3BillingUnitOfWork(this._connection, this.GetSuppliersRepository);
        }

        private async Task EnsureSchemaAsync()
        {
            using (var initTrans = await this._connection.BeginTransactionAsync().ConfigureAwait(false) as SQLiteTransaction)
            {
                try
                {
                    //var drop = "Drop table if exists Supplier";
                    var createSuppliersSQL = SQLite3SupplierQueries.GetCreateTableQuery();
                    var cmd = new SQLiteCommand(
                        //drop + ";" + 
                        createSuppliersSQL,
                        this._connection, initTrans);
                    var result = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    await initTrans.CommitAsync();
                }
                catch (Exception ex)
                {
                    // TODO: handle
                    Debug.WriteLine(ex);
                }
            }
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._connection?.Dispose();
                this._connection = null;
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            //base.Dispose(isDisposing);
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}