using Microsoft.Data.SqlClient;
using System;

namespace Billy.Billing.Persistence.SQL.MSSQLServer
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
    }
}
