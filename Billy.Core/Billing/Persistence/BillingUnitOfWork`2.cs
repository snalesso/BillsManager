using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public class BillingUnitOfWork<TConnection, TTransaction>
        : IBillingUnitOfWork
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        private readonly TConnection _connection;
        private readonly TTransaction _transaction;
        private readonly Func<TConnection, TTransaction, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        //private readonly Func<TConnection, TTransaction, IBillsRepository> _billsRepositoryFactoryMethod;

        //private readonly DbConnection _connection;
        //private DbTransaction _transaction;
        //private readonly Func<DbConnection, DbTransaction, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        //private readonly Func<DbConnection, DbTransaction, IBillsRepository> _billsRepositoryFactoryMethod;

        public BillingUnitOfWork(
            TConnection connection
            , Func<TConnection, TTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod
            //, Func<TConnection, TTransaction, IBillsRepository> billsRepositoryFactoryMethod
            )
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._suppliersRepositoryFactoryMethod = suppliersRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(suppliersRepositoryFactoryMethod));
            //this._billsRepositoryFactoryMethod = billsRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(billsRepositoryFactoryMethod));

            // TODO: use async and create as late as possible!
            this._transaction = (TTransaction)this._connection.BeginTransaction(IsolationLevel.Serializable);
        }

        private ISuppliersRepository _suppliers;
        public ISuppliersRepository Suppliers => this._suppliers ??= this._suppliersRepositoryFactoryMethod.Invoke(this._connection, this._transaction);

        private IBillsRepository _bills;
        public IBillsRepository Bills => throw new NotImplementedException();
            //this._bills ??= this._billsRepositoryFactoryMethod.Invoke(this._connection, this._transaction);

        public async Task CommitAsync()
        {
            if (this._transaction == null)
                // TODO: this is handled transparently but should be logged as an error since it should never happen
                return;

            await this._transaction.CommitAsync();
            // TODO: verify if following code should be added. Even though transaction is set to null repositories still hold a reference to it,
            //await this._transaction.DisposeAsync();

            //this._transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (this._transaction == null)
                // TODO: this is handled transparently but should be logged as an error since it should never happen
                return;

            await this._transaction.RollbackAsync();
            // TODO: verify if following code should be added
            //await this._transaction.DisposeAsync();

            //this._transaction = null;
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
            {
                return;
            }

            if (isDisposing)
            {
                // free managed resources here
                this._transaction?.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            //// remove in non-derived class
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