using Billy.Billing.Models;
using Billy.Domain.Persistence.SQL;
using Billy.Domain.Persistence.SQL.SQLite3;
using System;
using System.Data.SQLite;
using System.Linq;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    internal static class SQLite3BillsQueries
    {
        private static readonly Lazy<TableDefinition<Bill, Bill>> TableDef = new Lazy<TableDefinition<Bill, Bill>>(
            () => new TableDefinition<Bill, Bill>(
                new[]
                {
                    new SQLite3ColumnDef<Bill>(null, nameof(Bill.Id), TypeAffinity.Int64, isPK: true),

                    new SQLite3ColumnDef<Bill>(s => s.SupplierId.ToString(), nameof(Bill.SupplierId), TypeAffinity.Int64),
                    new SQLite3ColumnDef<Bill>(s => s.Code.Quoted(), nameof(Bill.Code), TypeAffinity.Text, isNotNull: true, isUnique: true),

                    new SQLite3ColumnDef<Bill>(s => s.Amount.ToString(), nameof(Bill.Amount), TypeAffinity.Double),
                    new SQLite3ColumnDef<Bill>(s => s.Agio .ToString() , nameof(Bill.Agio), TypeAffinity.Double),
                    new SQLite3ColumnDef<Bill>(s => s.AdditionalCosts.ToString(), nameof(Bill.AdditionalCosts), TypeAffinity.Double),

                    new SQLite3ColumnDef<Bill>(s => s.ReleaseDate.ToString(), nameof(Bill.ReleaseDate), TypeAffinity.DateTime),
                    new SQLite3ColumnDef<Bill>(s => s.DueDate.ToString(), nameof(Bill.DueDate), TypeAffinity.DateTime),
                    new SQLite3ColumnDef<Bill>(s => s.PaymentDate.ToString(), nameof(Bill.PaymentDate), TypeAffinity.DateTime),
                    new SQLite3ColumnDef<Bill>(s => s.RegistrationDate.ToString(), nameof(Bill.RegistrationDate), TypeAffinity.DateTime),

                    new SQLite3ColumnDef<Bill>(s => s.Notes.Quoted(), nameof(Bill.Notes), TypeAffinity.Text),
                }
            ));

        public static string GetCreateTableQuery()
        {
            return SQLite3QueriesHelper.CreateTable(true, TableDef.Value.ColumnDefs);
        }

        public static string GetCreateTableQueryRAW()
        {
            return
                SQLite3QueriesHelper.CreateTable<Bill>(
                    true,
                    $"\"{nameof(Bill.Id)}\" integer PRIMARY KEY",
                    $"\"{nameof(Bill.SupplierId)}\" integer NOT NULL",
                    $"\"{nameof(Bill.Code)}\" text",

                    $"\"{nameof(Bill.Amount)}\" real NOT NULL",
                    $"\"{nameof(Bill.Agio)}\" real not null",
                    $"\"{nameof(Bill.AdditionalCosts)}\" real not null",

                    $"\"{nameof(Bill.ReleaseDate)}\" datetime not null",
                    $"\"{nameof(Bill.DueDate)}\" datetime not null",
                    $"\"{nameof(Bill.PaymentDate)}\" datetime",
                    $"\"{nameof(Bill.RegistrationDate)}\" datetime not null default current_date",

                    $"\"{nameof(Bill.Notes)}\" text",

                    $"FOREIGN KEY(\"{nameof(Bill.SupplierId)}\") REFERENCES {nameof(Supplier)}(\"{nameof(Supplier.Id)}\") ON DELETE CASCADE"
                    );
        }

        public static string GetDeleteQuery(params uint[] ids)
        {
            return
                $"DELETE FROM {nameof(Bill)} " +
                $"WHERE {nameof(Bill.Id)} " +
                (ids.Length == 1
                ? $"= {ids[0]}"
                : $"IN ({string.Join(",", ids)})");
        }

        public static string GetUpdateQuery(uint id, Bill changes)
        {
            var colSetters = TableDef.Value.ColumnDefs
                .Where(c => c.ColumnValueExtractor != null)
                .Select(c => c.Name + " = " + c.ColumnValueExtractor(changes));

            var colSettersJoined = string.Join(",", colSetters);

            return
                $"UPDATE { nameof(Bill)} " +
                $"SET {colSettersJoined}" +
                $"WHERE {nameof(Bill.Id)} = {id}";
        }

        private static string Quoted(this string source)
        {
            return $"\"{source}\"";
        }

        public static string GetQuery()
        {
            return $"SELECT * FROM {nameof(Bill)}";
        }

        public static string GetInsertQuery(Bill supplier)
        {
            var insertableCols = TableDef.Value.ColumnDefs.Where(def => def.ColumnValueExtractor != null);
            var colNames = string.Join(",", insertableCols.Select(col => col.Name));
            var colValues = string.Join(",", insertableCols.Select(def => def.ColumnValueExtractor.Invoke(supplier)));

            return $"INSERT INTO {nameof(Bill)} ({colNames}) VALUES ({colValues})";
        }
    }
}