using BillsManager.Models;
using BillsManager.Services.Providers;
using Old.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConverter
{
    class Program
    {
        private static BillsDataService billsDataService = new BillsDataService();
        private static SuppliersDataService suppliersDataService = new SuppliersDataService();

        private readonly static string oldFornitoriDBPath = AppDomain.CurrentDomain.BaseDirectory + @"\db old\Fornitori.xml";
        private readonly static string oldAcquistiDBPath = AppDomain.CurrentDomain.BaseDirectory + @"\db old\Acquisti.xml";
        private readonly static string oldSpeseDBPath = AppDomain.CurrentDomain.BaseDirectory + @"\db old\Spese.xml";
        private readonly static string newDBsDir = AppDomain.CurrentDomain.BaseDirectory + @"\dbs\";

        private static IDBConnector CreateDB(
            IDBsProvider dbsProvider,
            string newDBName)
        {
            dbsProvider.CreateDB(newDBName);
            return new XMLDBConnector(dbsProvider.GetAll().Single(dbPath => Path.GetFileNameWithoutExtension(dbPath) == newDBName));
        }

        private static void AddSuppliers(IDBConnector dbConn, string oldFornitoriDBPath)
        {
            var fornitori = suppliersDataService.GetAllSuppliers(oldFornitoriDBPath);

            foreach (var forn in fornitori)
            {
                dbConn.Add(new Supplier(
                    dbConn.GetLastSupplierID() + 1,
                    forn.Ditta,
                    forn.Indirizzo,
                    string.Empty,
                    forn.Città,
                    string.Empty,
                    forn.Provincia,
                    string.Empty,
                    forn.EMail,
                    forn.Sito,
                    forn.Telefono,
                    string.Empty,
                    forn.Note,
                    forn.Agente,
                    string.Empty,
                    forn.TelefonoAgente));
            }
        }

        private static void AddBills(IDBConnector dbConn, string oldFattureDBPath)
        {
            var fatture = billsDataService.GetAllBills(oldFattureDBPath);

            var suppliers = dbConn.GetAllSuppliers();

            foreach (var fattura in fatture)
            {
                var id = suppliers.Single(supp => supp.Name == fattura.Fornitore).ID;
                double interm = fattura.Importo;

                bool eq = interm == fattura.Importo;
                double amount = Math.Round(interm, 2, MidpointRounding.ToEven);
                bool eq2 = amount == fattura.Importo;

                dbConn.Add(new Bill(
                    dbConn.GetLastBillID() + 1,
                    fattura.DataCreazione,
                    fattura.DataScadenza,
                    fattura.DataPagamento,
                    fattura.DataFattura,
                    amount,
                    id,
                    fattura.Note,
                    fattura.Numero
                    ));
            }
        }

        static void Main(string[] args)
        {
            var dbsProvider = new XMLDBsProvider(newDBsDir);

            var acquistiDB = CreateDB(dbsProvider, "Acquisti");
            acquistiDB.Open();
            AddSuppliers(acquistiDB, oldFornitoriDBPath);
            AddBills(acquistiDB, oldAcquistiDBPath);
            acquistiDB.Save();
            acquistiDB.Close();

            var speseDB = CreateDB(dbsProvider, "Spese");
            speseDB.Open();
            AddSuppliers(speseDB, oldFornitoriDBPath);
            AddBills(speseDB, oldSpeseDBPath);
            speseDB.Save();
            speseDB.Close();

            System.Diagnostics.Process.Start(newDBsDir);
        }
    }
}