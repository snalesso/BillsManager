namespace Billy.Billing.Persistence.SQL.SQLite3
{
    internal static class SQLite3BillingQueries
    {
        public static string GetCreateDatabaseSQL() =>
            "create database if not exists ";

        public static string GetCreateSuppliersTableSQL() =>
            "create database if not exists ";

    }
}