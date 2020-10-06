using System;
using Billy.Domain.Billing.Persistence;
using Microsoft.Data.SqlClient;

namespace Billy.Domain.Billing.Persistence.SQL.MSSQLServer
{
    public class MSSQLServerBillingUnitOfWork2 : BillingUnitOfWork<SqlConnection, SqlTransaction>, IBillingUnitOfWork
    {
        public MSSQLServerBillingUnitOfWork2(
            SqlConnection connection,
            Func<SqlConnection, SqlTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod
            //, Func<SqlConnection, SqlTransaction, IBillsRepository> billsRepositoryFactoryMethod
            )
            : base(
                  connection
                  , suppliersRepositoryFactoryMethod
                  //, billsRepositoryFactoryMethod
                  )
        {
        }

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private bool _isDisposed = false;

        // use this in derived class
        protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        //protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // free managed resources here
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            base.Dispose(isDisposing);
        }

        // remove if in derived class
        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
        //    this.Dispose(true);
        //}

        #endregion
    }
}
