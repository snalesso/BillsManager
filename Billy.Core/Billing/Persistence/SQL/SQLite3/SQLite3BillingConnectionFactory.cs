﻿using Billy.Domain.Persistence;
using Billy.Domain.Persistence.SQL.SQLite3;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public class SQLite3BillingConnectionFactory : IConnectionFactory<SQLiteConnection>//, IDisposable
    {
        public const string DbFileName = nameof(Billing) + "." + SQLite3QueriesHelper.DbFileExtension;
        public const string ConnectionString =
            "Version=3;" +
            ";Data Source=" + DbFileName +
            ";FailIfMissing=False";

        // TODO: investigate connection pooling: no need to cache connection?
        //private SqlConnection _connection;

        public Task<SQLiteConnection> CreateAsync()
        {
            //var csb = new SqlConnectionStringBuilder();

            //csb.Authentication = SqlAuthenticationMethod.NotSpecified;
            //csb.IntegratedSecurity = true;

            //this._connection ??= new SqlConnection(MSSQLServerBillingConnectionFactory.ConnectionString);

            //return Task.FromResult(this._connection);

            return Task.FromResult(
                //new SqlConnection(MSSQLServerBillingConnectionFactory.ConnectionString)
                this.Create()
                );
        }

        public SQLiteConnection Create()
        {
            //return this._connection ??= new SqlConnection(MSSQLServerBillingConnectionFactory.ConnectionString);
            return new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString);
        }

        //#region IDisposable

        //// https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        //private bool _isDisposed = false;

        //// use this in derived class
        //// protected override void Dispose(bool isDisposing)
        //// use this in non-derived class
        //protected virtual void Dispose(bool isDisposing)
        //{
        //    if (this._isDisposed)
        //        return;

        //    if (isDisposing)
        //    {
        //        // free managed resources here
        //        this._connection?.Dispose();
        //        this._connection = null;
        //    }

        //    // free unmanaged resources (unmanaged objects) and override a finalizer below.
        //    // set large fields to null.

        //    this._isDisposed = true;

        //    // remove in non-derived class
        //    //base.Dispose(isDisposing);
        //}

        //// remove if in derived class
        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
        //    this.Dispose(true);
        //}

        //#endregion
    }
}
