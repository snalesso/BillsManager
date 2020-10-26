using BillsManager.v1.Services.Data;
using Billy.Billing.Models;
using Billy.Billing.Persistence.SQL.SQLite3;
using Billy.Billing.Persistence.SQL.SQLite3.Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bill_V1 = BillsManager.v1.Models.Bill;
using Bill_V2 = Billy.Billing.Models.Bill;
using Supplier_V1 = BillsManager.v1.Models.Supplier;
using Supplier_V2 = Billy.Billing.Models.Supplier;

namespace Billy.Migrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting database migration");

            var oldDbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "old", "old.bmdb");

            Console.WriteLine("Loading old data ...");

            var v1_DbDeserializer = new XMLDbSerializer(oldDbFilePath);
            v1_DbDeserializer.Connect();

            var oldSuppliers = v1_DbDeserializer.GetAllSuppliers();
            var oldBills = v1_DbDeserializer.GetAllBills();
            var supplier_bills_v1_KVPs = oldSuppliers.ToDictionary(supp => supp, supp => oldBills.Where(bill => bill.SupplierID == supp.ID));

            v1_DbDeserializer.Disconnect();
            v1_DbDeserializer = null;

            Console.WriteLine("Writing to new database ...");

            //var sqlite3Connection = new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString);
            using var sqlite3UowFactory = new SQLite3BillingUnitOfWorkFactory(
                  (conn, trans) => new DapperSQLite3SuppliersRepository(conn, trans),
                  (conn, trans) => new DapperSQLite3BillsRepository(conn, trans));
            using var uow = await sqlite3UowFactory.CreateAsync();

            try
            {
                foreach (var supplier_v1 in supplier_bills_v1_KVPs.Keys)
                {
                    var newSupp_v2_data = Supplier_V1_To_Supplier_V2_Data(supplier_v1);
                    Supplier_V2 newSupplier_v2 = await uow.Suppliers.CreateAndAddAsync(newSupp_v2_data);

                    foreach (var bill_v1 in supplier_bills_v1_KVPs[supplier_v1])
                    {
                        var bill_v2_data = Bill_V1_To_Bill_V2_Data(bill_v1, newSupplier_v2.Id);
                        _ = await uow.Bills.CreateAndAddAsync(bill_v2_data);
                    }
                }

                await uow.CommitAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await uow.RollbackAsync();
            }
        }

        private static Dictionary<string, object> Supplier_V1_To_Supplier_V2_Data(Supplier_V1 supplier_v1)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Supplier_V2.Email)] = supplier_v1.eMail?.TrimToNull(),
                [nameof(Supplier_V2.Fax)] = supplier_v1.Fax?.TrimToNull(),
                [nameof(Supplier_V2.Name)] = supplier_v1.Name?.TrimToNull(),
                [nameof(Supplier_V2.Notes)] = supplier_v1.Notes?.TrimToNull(),
                [nameof(Supplier_V2.Phone)] = supplier_v1.Phone?.TrimToNull(),
                [nameof(Supplier_V2.Website)] = supplier_v1.Website?.TrimToNull(),

                [nameof(Supplier_V2.Agent)] = new Dictionary<string, object>()
                {
                    [nameof(Agent.Name)] = supplier_v1.AgentName?.TrimToNull(),
                    [nameof(Agent.Surname)] = supplier_v1.AgentSurname?.TrimToNull(),
                    [nameof(Agent.Phone)] = supplier_v1.AgentPhone?.TrimToNull()
                },
                [nameof(Supplier_V2.Address)] = new Dictionary<string, object>()
                {
                    [nameof(Address.City)] = supplier_v1.City?.TrimToNull(),
                    [nameof(Address.Country)] = supplier_v1.Country?.TrimToNull(),
                    [nameof(Address.Number)] = supplier_v1.Number?.TrimToNull(),
                    [nameof(Address.Province)] = supplier_v1.Province?.TrimToNull(),
                    [nameof(Address.Street)] = supplier_v1.Street?.TrimToNull(),
                    [nameof(Address.Zip)] = supplier_v1.Zip?.TrimToNull()
                }
            };
        }

        private static Dictionary<string, object> Bill_V1_To_Bill_V2_Data(Bill_V1 bill_v1, long supplierId)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Bill_V2.AdditionalCosts)] = bill_v1.AdditionalCosts,
                [nameof(Bill_V2.Agio)] = bill_v1.Agio,
                [nameof(Bill_V2.Amount)] = bill_v1.Amount,
                [nameof(Bill_V2.Code)] = bill_v1.Code?.TrimToNull(),
                [nameof(Bill_V2.DueDate)] = bill_v1.DueDate,
                [nameof(Bill_V2.Notes)] = bill_v1.Notes?.TrimToNull(),
                [nameof(Bill_V2.PaymentDate)] = bill_v1.PaymentDate,
                [nameof(Bill_V2.RegistrationDate)] = bill_v1.RegistrationDate,
                [nameof(Bill_V2.ReleaseDate)] = bill_v1.ReleaseDate,
                [nameof(Bill_V2.SupplierId)] = supplierId,
            };
        }
    }
}