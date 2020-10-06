using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Billy.Domain.Billing.Persistence.SQL.SQLite3
{
    public class SQLite3BillingUnitOfWork : IBillingUnitOfWork
    {
        private readonly SQLiteConnection _connection;
        private readonly SQLiteTransactionBase _transaction;

        private readonly Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        private readonly Func<SQLiteConnection, SQLiteTransactionBase, IBillsRepository> _billsRepositoryFactoryMethod;


        public SQLite3BillingUnitOfWork(
            SQLiteConnection connection
            , Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository> suppliersRepositoryFactoryMethod
            //, Func<SQLiteConnection, SQLiteTransactionBase, IBillsRepository> billsRepositoryFactoryMethod
            )
        //: base(connection, suppliersRepositoryFactoryMethod, billsRepositoryFactoryMethod)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));

            this._suppliersRepositoryFactoryMethod = suppliersRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(suppliersRepositoryFactoryMethod));
            //this._billsRepositoryFactoryMethod = billsRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(billsRepositoryFactoryMethod));

            // TODO: use async and create as late as possible!
            this._transaction = this._connection.BeginTransaction(IsolationLevel.Serializable);
        }

        private ISuppliersRepository _suppliers;
        public ISuppliersRepository Suppliers => this._suppliers ??= this._suppliersRepositoryFactoryMethod.Invoke(this._connection, this._transaction);

        private IBillsRepository _bills;
        public IBillsRepository Bills => this._bills ??= this._billsRepositoryFactoryMethod.Invoke(this._connection, this._transaction);

        public async Task CommitAsync()
        {
            // TODO: handle transaction == null, if so then this UOW lifetime is terminated and shouldn't be accesed ever again
            if (this._transaction == null)
                throw new InvalidOperationException("Transaction has already been closed and cannot be commited.");

            await this._transaction?.CommitAsync();

            //return Task.CompletedTask;
        }

        public async Task RollbackAsync()
        {
            // TODO: handle transaction == null, if so then this UOW lifetime is terminated and shouldn't be accesed ever again
            if (this._transaction == null)
                throw new InvalidOperationException("Transaction has already been closed and cannot be commited.");

            await this._transaction?.RollbackAsync();

            //return Task.CompletedTask;
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
