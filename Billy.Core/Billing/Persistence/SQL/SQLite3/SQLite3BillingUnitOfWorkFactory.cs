using Billy.Billing.Persistence.SQL.SQLite3.Dapper;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public class SQLite3BillingUnitOfWorkFactory : IBillingUnitOfWorkFactory, IDisposable
    {
        #region constants & fields

        //private readonly IConnectionFactory<SqlConnection> _connectionFactory;
        private readonly Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        private readonly Func<SQLiteConnection, SQLiteTransactionBase, IBillsRepository> _billsRepositoryFactoryMethod;

        private SQLiteConnection _connection;

        #endregion

        public SQLite3BillingUnitOfWorkFactory(
            //IConnectionFactory<SqlConnection> connectionFactory
            Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository> suppliersRepositoryFactoryMethod,
            Func<SQLiteConnection, SQLiteTransactionBase, IBillsRepository> billsRepositoryFactoryMethod)
        {
            //this._connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this._suppliersRepositoryFactoryMethod = suppliersRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(suppliersRepositoryFactoryMethod));
            this._billsRepositoryFactoryMethod = billsRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(billsRepositoryFactoryMethod));
        }

        public async Task<IBillingUnitOfWork> CreateAsync()
        {
            if (this._connection == null)
            {
                this._connection = new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString);
                await this._connection.OpenAsync().ConfigureAwait(false);

                // TODO: execute only when needed
                await this.EnsureSchemaAsync().ConfigureAwait(false);
            }

            return new SQLite3BillingUnitOfWork(this._connection, this._suppliersRepositoryFactoryMethod, this._billsRepositoryFactoryMethod);
        }

        private async Task EnsureSchemaAsync()
        {
            using (var initTrans = await this._connection.BeginTransactionAsync().ConfigureAwait(false) as SQLiteTransaction)
            {
                try
                {
                    //var drop = "Drop table if exists Supplier";
                    var createSuppliersSQL = SQLite3SupplierQueries.GetCreateTableQuery();
                    var createBillsSQL = SQLite3BillsQueries.GetCreateTableQueryRAW();
                    var createSchemaSQL = string.Join(
                        ";"
#if DEBUG
                        + Environment.NewLine
#endif
                        ,
                        createSuppliersSQL,
                        createBillsSQL);
                    var cmd = new SQLiteCommand(createSchemaSQL, this._connection, initTrans);
                    var result = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    await initTrans.CommitAsync().ConfigureAwait(false);
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
