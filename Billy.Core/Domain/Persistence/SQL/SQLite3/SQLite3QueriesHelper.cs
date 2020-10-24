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
        private static readonly string DbFileExtension = "sqlite3.db";
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
        {
            return CreateTable<T>(ifNotExists, columnDefinitions.ToArray());
        }

        public static string CreateTable<T>(bool ifNotExists, params SQLColumnDef[] columnDefinitions)
        {
            return CreateTable<T>(ifNotExists, columnDefinitions.Select(cd => cd.GetSQLDef()));
        }

        public static string CreateTable<T>(bool ifNotExists, IEnumerable<SQLColumnDef> columnDefinitions)
        {
            return CreateTable<T>(ifNotExists, columnDefinitions.ToArray());
        }

        public static string CreateTable<T>(bool ifNotExists, IEnumerable<SQLColumnDef<T>> columnDefinitions)
        {
            return CreateTable<T>(ifNotExists, columnDefinitions.Select(cd => cd.GetSQLDef()));
        }

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

        // TODO: verify if this stuff is all needed
        //public static void EnsureDatabase()
        //{

        //    if (!File.Exists(DbFileExtension))
        //    {
        //        // TODO: async?
        //        SQLiteConnection.CreateFile(DbFileExtension);
        //    }

        //    // TODO: this should be moved to specific implementations which use SQLite3
        //    var cs = new SQLiteConnectionStringBuilder
        //    {
        //        DataSource = DbFileExtension,
        //        SyncMode = SynchronizationModes.Normal,
        //        DefaultIsolationLevel = IsolationLevel.Serializable,
        //        FailIfMissing = false,
        //        //Flags = SQLiteConnectionFlags.
        //        ToFullPath = true,
        //        Version = 3
        //    };
        //    //cs.SyncMode = SynchronizationModes.Full;
        //    // TODO: improve config
        //    SQLiteConnection connection = new SQLiteConnection("Data Source = " + DbFileExtension);
        //}

        public static string GetInsertQueryReturningId(string insertQuery)
        {
            return string.Join(";", insertQuery, "select last_insert_rowid()");
        }

        //string[] dogNames = Utility.GetRandomName(3, 5, 9);
        //var existedDogs = connection.Query<Dog>("SELECT * FROM Dog WHERE Name IN @name", new { name = dogNames });
        //var notExistedDogNames = dogNames.Except(existedDogs.Select(x => x.Name));
        //var anonymousDogName = notExistedDogNames.Select(x => new { name = x }).ToArray();
        //Execute(connection, "INSERT INTO Dog VALUES(null,@name)", anonymousDogName);
        //string[] personNames = Utility.GetRandomName(4, 10, 999);
        //var existedSuppliers = connection.Query<Supplier>("SELECT * FROM Supplier WHERE Name IN @name", new { name = personNames });
        //var notExistedSupplierNames = personNames.Except(existedSuppliers.Select(x => x.Name));
        //int[] dogIDs = connection.Query<Dog>("SELECT * FROM Dog").Select(x => x.ID).ToArray().GetRandomElement(notExistedSupplierNames.Count());
        //var anonymousSupplierNameDogId = Utility.Combine(notExistedSupplierNames, dogIDs).Select(x => new { name = x.Item1, createTime = DateTime.Now, dogID = x.Item2 }).ToArray();
        //Execute(connection, "INSERT INTO Supplier VALUES(null,@name,@createTime,@dogID)", anonymousSupplierNameDogId);
        //var result = connection.Query<Supplier, Dog, Supplier>("SELECT * FROM Supplier INNER JOIN Dog ON Supplier.DogID = Dog.ID", (person, dog) => { person.Dog = dog; return person; });
        //int index = 0;
        //});
    }
}