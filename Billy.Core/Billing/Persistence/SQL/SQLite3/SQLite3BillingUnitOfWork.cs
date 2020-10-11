using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public class SQLite3BillingUnitOfWork : BillingUnitOfWork<SQLiteConnection, SQLiteTransactionBase>, IBillingUnitOfWork
    {
        public SQLite3BillingUnitOfWork(
            SQLiteConnection connection,
            Func<SQLiteConnection, SQLiteTransactionBase, ISuppliersRepository> suppliersRepositoryFactoryMethod
            //, Func<SqlConnection, SqlTransaction, IBillsRepository> billsRepositoryFactoryMethod
            )
            : base(
                  connection
                  , suppliersRepositoryFactoryMethod
                  //, billsRepositoryFactoryMethod
                  )
        {
        }
    }
}
