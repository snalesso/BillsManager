using Billy.Billing.Persistence.SQL;
using Microsoft.Data.SqlClient;

namespace Billy.Billing.Persistence.Dapper
{
    public class MSSQLServerDapperSuppliersRepository : DapperSuppliersRepository
    {
        public MSSQLServerDapperSuppliersRepository(SqlConnection connection, SqlTransaction transaction = null)
            : base(connection, transaction)
        {
        }

        protected override string GetSelectScopeIdentitySQL() => "SELECT SCOPE_IDENTITY()";
    }
}
