using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.MSSQLServer
{
    public class MSSQLServerBillingUnitOfWorkFactory : IBillingUnitOfWorkFactory, IDisposable
    {
        #region constants & fields

        //private readonly IConnectionFactory<SqlConnection> _connectionFactory;
        private readonly Func<SqlConnection, SqlTransaction, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        private readonly Func<SqlConnection, SqlTransaction, IBillsRepository> _billsRepositoryFactoryMethod;

        private SqlConnection _connection;

        #endregion

        public MSSQLServerBillingUnitOfWorkFactory(
            //IConnectionFactory<SqlConnection> connectionFactory
            Func<SqlConnection, SqlTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod,
            Func<SqlConnection, SqlTransaction, IBillsRepository> billsRepositoryFactoryMethod)
        {
            //this._connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this._suppliersRepositoryFactoryMethod = suppliersRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(suppliersRepositoryFactoryMethod));
            this._billsRepositoryFactoryMethod = billsRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(billsRepositoryFactoryMethod));
        }

        public async Task<IBillingUnitOfWork> CreateAsync()
        {
            if (this._connection == null)
            {
                this._connection = new SqlConnection(MSSQLServerBillingConnectionFactory.ConnectionString);
                await this._connection.OpenAsync().ConfigureAwait(false);
            }

            return new MSSQLServerBillingUnitOfWork2(this._connection, this._suppliersRepositoryFactoryMethod, this._billsRepositoryFactoryMethod);
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
