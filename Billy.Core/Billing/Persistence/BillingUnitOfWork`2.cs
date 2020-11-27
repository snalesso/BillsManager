using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    // TODO: just use DBConnection and DBTransaction and use covariant constructor in derived class
    public class BillingUnitOfWork<TConnection, TTransaction> : IBillingUnitOfWork
        where TConnection : DbConnection
        where TTransaction : DbTransaction
    {
        private readonly TConnection _connection;
        private readonly TTransaction _transaction;

        //private readonly DbConnection _connection;
        //private DbTransaction _transaction;
        //private readonly Func<DbConnection, DbTransaction, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        //private readonly Func<DbConnection, DbTransaction, IBillsRepository> _billsRepositoryFactoryMethod;

        #region repository

        private readonly Func<TConnection, TTransaction, ISuppliersRepository> _suppliersRepositoryFactoryMethod;
        private readonly Func<TConnection, TTransaction, IBillsRepository> _billsRepositoryFactoryMethod;
        private readonly Func<TConnection, TTransaction, ISupplierSummaries> _supplierSummariesViewFactoryMethod;
        private readonly Func<TConnection, TTransaction, ISupplierHeadersView> _supplierHeadersViewFactoryMethod;
        private readonly Func<TConnection, TTransaction, IDetailedBillsView> _detailedBillsViewFactoryMethod;

        public BillingUnitOfWork(
            TConnection connection
            , Func<TConnection, TTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod
            , Func<TConnection, TTransaction, IBillsRepository> billsRepositoryFactoryMethod
            //, Func<TConnection, TTransaction, ISupplierSummaries> supplierSummariesViewFactoryMethod
            //, Func<TConnection, TTransaction, ISupplierHeadersView> supplierHeadersViewFactoryMethod
            //, Func<TConnection, TTransaction, IDetailedBillsView> detailedBillsViewFactoryMethod
            )
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._suppliersRepositoryFactoryMethod = suppliersRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(suppliersRepositoryFactoryMethod));
            this._billsRepositoryFactoryMethod = billsRepositoryFactoryMethod ?? throw new ArgumentNullException(nameof(billsRepositoryFactoryMethod));
            //this._supplierSummariesViewFactoryMethod = supplierSummariesViewFactoryMethod ?? throw new ArgumentNullException(nameof(supplierSummariesViewFactoryMethod));
            //this._supplierHeadersViewFactoryMethod = supplierHeadersViewFactoryMethod ?? throw new ArgumentNullException(nameof(supplierHeadersViewFactoryMethod));
            //this._detailedBillsViewFactoryMethod = detailedBillsViewFactoryMethod ?? throw new ArgumentNullException(nameof(detailedBillsViewFactoryMethod));

            // TODO: use async and create as late as possible!
            this._transaction = (TTransaction)this._connection.BeginTransaction(IsolationLevel.Serializable);
        }

        #region tables

        private ISuppliersRepository _suppliers;
        public ISuppliersRepository Suppliers => this._suppliers ??= this._suppliersRepositoryFactoryMethod.Invoke(this._connection, this._transaction);

        private IBillsRepository _bills;
        public IBillsRepository Bills => this._bills ??= this._billsRepositoryFactoryMethod.Invoke(this._connection, this._transaction);

        #endregion

        #region views

        private ISupplierSummaries _supplierSummaries;
        public ISupplierSummaries SupplierSummaries => this._supplierSummaries ??= this._supplierSummariesViewFactoryMethod.Invoke(this._connection, this._transaction);

        private ISupplierHeadersView _supplierHeaders;
        public ISupplierHeadersView SupplierHeaders => this._supplierHeaders ??= this._supplierHeadersViewFactoryMethod.Invoke(this._connection, this._transaction);

        private IDetailedBillsView _detailedBills;
        public IDetailedBillsView DetailedBills => this._detailedBills ??= this._detailedBillsViewFactoryMethod.Invoke(this._connection, this._transaction);

        #endregion

        #endregion

        //#region cqrs

        //private readonly Func<TConnection, TTransaction, IReadBillingModel> _readBillingModelFactoryMethod;
        //private readonly Func<TConnection, TTransaction, IWriteBillingModel> _writeBillingModelFactoryMethod;

        //public BillingUnitOfWork(
        //    TConnection connection
        //    , Func<TConnection, TTransaction, IReadBillingModel> readBillingModelFactoryMethod
        //    , Func<TConnection, TTransaction, IWriteBillingModel> writeBillingModelFactoryMethod
        //    )
        //{
        //    this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
        //    this._readBillingModelFactoryMethod = readBillingModelFactoryMethod ?? throw new ArgumentNullException(nameof(readBillingModelFactoryMethod));
        //    this._writeBillingModelFactoryMethod = writeBillingModelFactoryMethod ?? throw new ArgumentNullException(nameof(writeBillingModelFactoryMethod));

        //    // TODO: use async and create as late as possible!
        //    this._transaction = (TTransaction)this._connection.BeginTransaction(IsolationLevel.Serializable);
        //}

        //private IReadBillingModel _readBillingModel;
        //public IReadBillingModel Read => this._readBillingModel ??= this._readBillingModelFactoryMethod.Invoke(this._connection, this._transaction);

        //private IWriteBillingModel _writeBillingModel;
        //public IWriteBillingModel Write => this._writeBillingModel ??= this._writeBillingModelFactoryMethod.Invoke(this._connection, this._transaction);

        //#endregion

        public async Task CommitAsync()
        {
            if (this._transaction == null)
                // TODO: this is handled transparently but should be logged as an error since it should never happen
                return;

            await this._transaction.CommitAsync().ConfigureAwait(false);
            // TODO: verify if following code should be added. Even though transaction is set to null repositories still hold a reference to it,
            //await this._transaction.DisposeAsync();

            //this._transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (this._transaction == null)
                // TODO: this is handled transparently but should be logged as an error since it should never happen
                return;

            await this._transaction.RollbackAsync().ConfigureAwait(false);
            // TODO: verify if following code should be added
            //await this._transaction.DisposeAsync();

            //this._transaction = null;
        }

        #region IDisposable

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