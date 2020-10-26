using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Billy.Domain.Persistence.SQL.SQLite3
{
    internal static class SQLite3QueriesHelper
    {
        public const string DbFileExtension = "sqlite3.db";
        public const char ComposedColumnFieldsSeparator = DbSchemaHelper.ComposedColumnFieldsSeparator;

        public static string CreateTable<T>(bool ifNotExists, params string[] columnDefinitions)
        {
            var cols =
#if DEBUG
                Environment.NewLine +
#endif
                string.Join(
            ","
#if DEBUG
                + Environment.NewLine
#endif
                , columnDefinitions);

            return
                $"CREATE TABLE"
                + (ifNotExists ? " IF NOT EXISTS" : string.Empty)
                + $" \"{typeof(T).Name}\""
                + $" ({cols})";
        }

        public static string CreateTable<T>(bool ifNotExists, IEnumerable<string> columnDefinitions)
            => CreateTable<T>(ifNotExists, columnDefinitions.ToArray());

        public static string CreateTable<T>(bool ifNotExists, params SQLColumnDef[] columnDefinitions)
            => CreateTable<T>(ifNotExists, columnDefinitions.Select(cd => cd.GetSQLDef()));

        public static string CreateTable<T>(bool ifNotExists, IEnumerable<SQLColumnDef> columnDefinitions)
            => CreateTable<T>(ifNotExists, columnDefinitions.ToArray());

        public static string CreateTable<T>(bool ifNotExists, IEnumerable<SQLColumnDef<T>> columnDefinitions)
            => CreateTable<T>(ifNotExists, columnDefinitions.Select(cd => cd.GetSQLDef()));

        public static string BuildColName(params string[] fieldNames)
        {
            return $"{string.Join(SQLite3QueriesHelper.ComposedColumnFieldsSeparator, fieldNames)}";
        }

        /* private static string EnsureSuppliersTable()
        {
            return
                CreateTable<Supplier>(
                    true,
                    $"{nameof(Supplier.Id)} integer PRIMARY KEY",
                    $"{nameof(Supplier.Email)} text",
                    $"{nameof(Supplier.Fax)} text",
                    $"{nameof(Supplier.Name)} text NOT NULL UNIQUE",
                    $"{nameof(Supplier.Notes)} text",
                    $"{nameof(Supplier.Phone)} text",
                    $"{nameof(Supplier.Website)} text",
                    // address
                    $"{BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Country))} text",
                    $"{BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Province))} text",
                    $"{BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.City))} text",
                    $"{BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Zip))} text",
                    $"{BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Street))} text",
                    $"{BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Number))} text",
                    // agemt
                    $"{BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name))} text",
                    $"{BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname))} text",
                    $"{BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone))} text");
        }

        private static string EnsureBillsTable()
        {
            return CreateTable<Bill>(true) + " (" +

                string.Join(", ", new[]
                {
                    $"{nameof(Bill.Id)} integer PRIMARY KEY",
                    $"{nameof(Bill.AdditionalCosts)} text NOT NULL UNIQUE" ,
                    $"{nameof(Bill.Agio)} text" ,
                    $"{nameof(Bill.Amount)} text",
                    $"{nameof(Bill.Code)} text" ,
                    $"{nameof(Bill.DueDate)} text" ,
                    $"{nameof(Bill.Notes)} text" ,
                    $"{nameof(Bill.PaymentDate)} text" ,
                    $"{nameof(Bill.RegistrationDate)} text" ,
                    $"{nameof(Bill.ReleaseDate)} text" ,
                    $"{nameof(Bill.SupplierID)} text"
                }) + ")";
        }*/

        public static string GetInsertQueryReturningId(string insertQuery)
        {
            return string.Join(";", insertQuery, "select last_insert_rowid()");
        }
    }
}