using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using Billy.Domain.Billing.Models;
using Billy.Domain.Persistence.SQL;
using Billy.Domain.Persistence.SQL.SQLite3;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    internal static class SQLite3SupplierQueries
    {
        private static readonly Lazy<TableDefinition<Supplier, Supplier>> TableDef = new Lazy<TableDefinition<Supplier, Supplier>>(
            () => new TableDefinition<Supplier, Supplier>(
                new[]
                {
                    new SQLite3ColumnDef<Supplier>(null, nameof(Supplier.Id), TypeAffinity.Int64, isPK: true),
                    new SQLite3ColumnDef<Supplier>(s => s.Email.Quoted(), nameof(Supplier.Email), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Fax.Quoted(), nameof(Supplier.Fax), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Name.Quoted(), nameof(Supplier.Name), TypeAffinity.Text, isNotNull: true, isUnique: true),
                    new SQLite3ColumnDef<Supplier>(s => s.Notes.Quoted(), nameof(Supplier.Notes), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Phone.Quoted(), nameof(Supplier.Phone), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Website.Quoted(), nameof(Supplier.Website), TypeAffinity.Text),

                    new SQLite3ColumnDef<Supplier>(s => s.Address.Country.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Country)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Address.Province.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Province)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Address.City.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.City)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Address.Zip.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Zip)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Address.Street.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Street)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Address.Number.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Number)), TypeAffinity.Text),

                    new SQLite3ColumnDef<Supplier>(s => s.Agent.Name.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Agent.Surname.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname)), TypeAffinity.Text),
                    new SQLite3ColumnDef<Supplier>(s => s.Agent.Phone.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone)), TypeAffinity.Text),
                }
            ));

        //private static readonly Lazy<IReadOnlyList<SQLite3ColumnDef<Supplier>>> ColumnDefs = new Lazy<IReadOnlyList<SQLite3ColumnDef<Supplier>>>(
        //    () => new[]
        //    {
        //        new SQLite3ColumnDef<Supplier>(null, nameof(Supplier.Id), TypeAffinity.Int64, isPK: true),
        //        new SQLite3ColumnDef<Supplier>(s => s.Email.Quoted(), nameof(Supplier.Email), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Fax.Quoted(), nameof(Supplier.Fax), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Name.Quoted(), nameof(Supplier.Name), TypeAffinity.Text, isNotNull: true, isUnique: true),
        //        new SQLite3ColumnDef<Supplier>(s => s.Notes.Quoted(), nameof(Supplier.Notes), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Phone.Quoted(), nameof(Supplier.Phone), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Website.Quoted(), nameof(Supplier.Website), TypeAffinity.Text),

        //        new SQLite3ColumnDef<Supplier>(s => s.Address.Country.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Country)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Address.Province.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Province)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Address.City.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.City)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Address.Zip.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Zip)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Address.Street.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Street)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Address.Number.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Number)), TypeAffinity.Text),

        //        new SQLite3ColumnDef<Supplier>(s => s.Agent.Name.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Agent.Surname.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname)), TypeAffinity.Text),
        //        new SQLite3ColumnDef<Supplier>(s => s.Agent.Phone.Quoted(), SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone)), TypeAffinity.Text),
        //    }
        //    .ToList()
        //    .AsReadOnly());

        public static string GetCreateTableQuery_old()
        {
            return
                SQLite3QueriesHelper.CreateTable<Supplier>(
                    true,
                    $"{nameof(Supplier.Id)} integer PRIMARY KEY",
                    $"{nameof(Supplier.Email)} text",
                    $"{nameof(Supplier.Fax)} text",
                    $"{nameof(Supplier.Name)} text NOT NULL UNIQUE",
                    $"{nameof(Supplier.Notes)} text",
                    $"{nameof(Supplier.Phone)} text",
                    $"{nameof(Supplier.Website)} text",
                    // address
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Country))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Province))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.City))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Zip))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Street))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Number))} text",
                    // agemt
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname))} text",
                    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone))} text");
        }

        public static string GetDeleteQuery(params uint[] ids)
        {
            return
                $"DELETE FROM {nameof(Supplier)} " +
                $"WHERE {nameof(Supplier.Id)} " +
                (ids.Length == 1
                ? $" = {ids[0]}"
                : $"IN ({string.Join(",", ids)})");
        }

        public static string GetUpdateQuery(uint id, Supplier changes)
        {
            var colSetters = SQLite3SupplierQueries.TableDef.Value.ColumnDefs
                .Where(c => c.ColumnValueExtractor != null)
                .Select(c => c.Name + " = " + c.ColumnValueExtractor(changes));

            var colSettersJoined = string.Join(",", colSetters);

            return
                $"UPDATE { nameof(Supplier)} " +
                $"SET {colSettersJoined}" +
                $"WHERE {nameof(Supplier.Id)} = {id}";
        }

        private static string Quoted(this string source)
        {
            return $"\"{source}\"";
        }

        public static string GetCreateTableQuery()
        {
            return SQLite3QueriesHelper.CreateTable(true, TableDef.Value.ColumnDefs);
        }

        public static string GetQuery()
        {
            return $"SELECT * FROM {nameof(Supplier)}";


            //SQLite3QueriesHelper.CreateTable<Supplier>(
            //    true,
            //    $"{nameof(Supplier.Id)} integer PRIMARY KEY",
            //    $"{nameof(Supplier.Email)} text",
            //    $"{nameof(Supplier.Fax)} text",
            //    $"{nameof(Supplier.Name)} text NOT NULL UNIQUE",
            //    $"{nameof(Supplier.Notes)} text",
            //    $"{nameof(Supplier.Phone)} text",
            //    $"{nameof(Supplier.Website)} text",
            //    // address
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Country))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Province))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.City))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Zip))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Street))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Number))} text",
            //    // agemt
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone))} text");
        }

        public static string GetInsertQuery(Supplier supplier)
        {
            var insertableCols = TableDef.Value.ColumnDefs.Where(def => def.ColumnValueExtractor != null);
            var colNames = string.Join(",", insertableCols.Select(col => col.Name));
            var colValues = string.Join(",", insertableCols.Select(def => def.ColumnValueExtractor.Invoke(supplier)));

            return $"INSERT INTO {nameof(Supplier)} ({colNames}) VALUES ({colValues})";


            //SQLite3QueriesHelper.CreateTable<Supplier>(
            //    true,
            //    $"{nameof(Supplier.Id)} integer PRIMARY KEY",
            //    $"{nameof(Supplier.Email)} text",
            //    $"{nameof(Supplier.Fax)} text",
            //    $"{nameof(Supplier.Name)} text NOT NULL UNIQUE",
            //    $"{nameof(Supplier.Notes)} text",
            //    $"{nameof(Supplier.Phone)} text",
            //    $"{nameof(Supplier.Website)} text",
            //    // address
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Country))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Province))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.City))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Zip))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Street))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Address), nameof(Supplier.Address.Number))} text",
            //    // agemt
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname))} text",
            //    $"{SQLite3QueriesHelper.BuildColName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone))} text");
        }
    }
}