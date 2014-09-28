using BillsManager.Models;
using BillsManager.Services.Providers;
using Old.Models;
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

        private readonly static string oldFornitoriDBPath = AppDomain.CurrentDomain.BaseDirectory + @"\old\Fornitori.xml";
        private readonly static string oldAcquistiDBPath = AppDomain.CurrentDomain.BaseDirectory + @"\old\Acquisti.xml";
        private readonly static string oldSpeseDBPath = AppDomain.CurrentDomain.BaseDirectory + @"\old\Spese.xml";
        private readonly static string newDBPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB", "db.bmdb");

        private static uint suppsID = 0;
        private static uint billsID = 0;

        private static IEnumerable<Supplier> GetOldSuppliersConverted()
        {
            var oldSupps = suppliersDataService.GetAllSuppliers(oldFornitoriDBPath);

            var newSupps = new List<Supplier>();

            foreach (var oldSupp in oldSupps)
            {
                suppsID++;
                newSupps.Add(
                    new Supplier(
                        suppsID,
                        oldSupp.Ditta,
                        oldSupp.Indirizzo,
                        string.Empty,
                        oldSupp.Città,
                        string.Empty,
                        oldSupp.Provincia,
                        string.Empty,
                        oldSupp.EMail,
                        oldSupp.Sito,
                        oldSupp.Telefono,
                        string.Empty,
                        oldSupp.Note,
                        oldSupp.Agente,
                        string.Empty,
                        oldSupp.TelefonoAgente));
            }

            return newSupps;
        }

        private static List<Bill> GetOldBills(IEnumerable<Fattura> oldBills, IEnumerable<Supplier> suppliers)
        {
            var oldBillsConverted = new List<Bill>();

            foreach (Fattura oldBill in oldBills)
            {
                var suppID = suppliers.Single(supp => supp.Name == oldBill.Fornitore).ID;
                double interm = oldBill.Importo;

                bool eq = interm == oldBill.Importo;
                double amount = Math.Round(interm, 2, MidpointRounding.ToEven);
                bool eq2 = amount == oldBill.Importo;

                billsID++;
                oldBillsConverted.Add(
                    new Bill(
                        billsID,
                        suppID,
                        oldBill.DataCreazione,
                        oldBill.DataScadenza,
                        oldBill.DataFattura,
                        oldBill.DataPagamento,
                        amount,
                        0,
                        0,
                        oldBill.Numero,
                        oldBill.Note));
            }

            return oldBillsConverted;
        }
        
        static void Main(string[] args)
        {
            var dbProvider = new XMLDBConnector(newDBPath);
            dbProvider.Connect();

            var newSuppliers = GetOldSuppliersConverted();
            foreach (var supp in newSuppliers)
                dbProvider.Add(supp);

            var newBills = GetOldBills(billsDataService.GetAllBills(oldAcquistiDBPath).ToList(), newSuppliers);
            newBills.AddRange(GetOldBills(billsDataService.GetAllBills(oldSpeseDBPath).ToList(), newSuppliers));

            foreach (var bill in newBills)
                dbProvider.Add(bill);

            dbProvider.Save();

            System.Diagnostics.Process.Start(Path.GetDirectoryName(newDBPath));
        }
    }
}