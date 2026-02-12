using System;
using System.Data.SQLite;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public class SQLite3BillingUnitOfWork : BillingUnitOfWork<SQLiteConnection, SQLiteTransactionBase>, IBillingUnitOfWork
    {
        public SQLite3BillingUnitOfWork(
            SQLiteConnection connection
            , Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository> suppliersRepositoryFactoryMethod
            , Func<SQLiteConnection, SQLiteTransactionBase, IBillsRepository> billsRepositoryFactoryMethod
            )
            : base(
                  connection
                  , suppliersRepositoryFactoryMethod
                  , billsRepositoryFactoryMethod
                  )
        {
        }
    }
}
