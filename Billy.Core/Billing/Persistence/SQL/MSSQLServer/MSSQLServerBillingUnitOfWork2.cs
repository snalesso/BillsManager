using Microsoft.Data.SqlClient;
using System;

namespace Billy.Billing.Persistence.SQL.MSSQLServer
{
    public class MSSQLServerBillingUnitOfWork2 : BillingUnitOfWork<SqlConnection, SqlTransaction>, IBillingUnitOfWork
    {
        public MSSQLServerBillingUnitOfWork2(
            SqlConnection connection
            , Func<SqlConnection, SqlTransaction, ISuppliersRepository> suppliersRepositoryFactoryMethod
            , Func<SqlConnection, SqlTransaction, IBillsRepository> billsRepositoryFactoryMethod
            //, Func<SqlConnection, SqlTransaction, ISupplierSummaries> supplierSummariesViewFactoryMethod
            //, Func<SqlConnection, SqlTransaction, ISupplierHeadersView> supplierHeadersViewFactoryMethod
            //, Func<SqlConnection, SqlTransaction, IDetailedBillsView> detailedBillsViewFactoryMethod
            )
            : base(
                  connection
                  , suppliersRepositoryFactoryMethod
                  , billsRepositoryFactoryMethod
                  //,supplierSummariesViewFactoryMethod
                  //,supplierHeadersViewFactoryMethod
                  //,detailedBillsViewFactoryMethod
                  )
        {
        }
    }
}
