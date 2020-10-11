using Billy.Billing.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public class BillingUnitOfWork : IBillingUnitOfWork
    {
        private readonly DbConnection _connection;
        private readonly DbTransaction _transaction;

        private readonly Func<DbConnection, DbTransaction, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        //private readonly Func<DbConnection, DbTransaction, IBillsRepository> _billsRepositoryFactoryMethod;

        public BillingUnitOfWork(
            DbConnection connection
            //, Func<DbConnection, DbTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod
            , Func<DbConnection, DbTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod
            //, Func<DbConnection, DbTransaction, IBillsRepository> billsRepositoryFactoryMethod
            )
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._suppliersRepositoryFactoryMethod = suppliersRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(suppliersRepositoryFactoryMethod));
            //this._billsRepositoryFactoryMethod = billsRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(billsRepositoryFactoryMethod));

            this._transaction = this._connection.BeginTransaction();
            this.Suppliers = this._suppliersRepositoryFactoryMethod.Invoke(this._connection, this._transaction);
        }

        //private ISuppliersRepository _suppliers;
        public ISuppliersRepository Suppliers { get; }
        // => this._suppliers ??=new DapperMSSQLServerSuppliersRepository(this._connection, this._transaction);
        // => this._suppliers ??= this._suppliersRepositoryFactoryMethod.Invoke(this._connection, this._transaction);
        //{
        //    get
        //    {
        //        if (this._suppliers == null)
        //            this._suppliers = this._suppliersRepositoryFactoryMethod.Invoke(this._connection, this._transaction                        );

        //        return this._suppliers;
        //    }
        //}

        //private readonly IBillsRepository _bills;
        public IBillsRepository Bills => throw new NotImplementedException();
        //this._bills ?? (this._bills = this._billsRepositoryFactoryMethod.Invoke(this._connection, this._transaction));

        public async Task CommitAsync()
        {
            if (this._transaction == null)
                // TODO: this is handled transparently but should be logged as an error since it should never happen
                return;

            await this._transaction.CommitAsync();
            //await this._transaction.DisposeAsync();

            //this._transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (this._transaction == null)
                // TODO: this is handled transparently but should be logged as an error since it should never happen
                return;

            await this._transaction.RollbackAsync();
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
