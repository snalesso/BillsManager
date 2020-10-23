using Billy.Billing.Persistence.SQL;
using System.Data.SQLite;

namespace Billy.Billing.Persistence.Dapper
{
    public class SQLite3DapperSuppliersRepository : DapperSuppliersRepository
    {
        public SQLite3DapperSuppliersRepository(SQLiteConnection connection, SQLiteTransactionBase transaction = null)
            : base(connection, transaction)
        {
        }

        protected override string GetSelectScopeIdentitySQL() => "select last_insert_rowid()";
    }
}
