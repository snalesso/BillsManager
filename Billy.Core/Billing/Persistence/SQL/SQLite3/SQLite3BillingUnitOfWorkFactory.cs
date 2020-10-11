using Billy.Billing.Persistence;
using Billy.Billing.Persistence.SQL.SQLite3;
using Billy.Billing.Persistence.SQL.SQLite3.Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
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
            }

            return new SQLite3BillingUnitOfWork(this._connection, this.GetSuppliersRepository);
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
