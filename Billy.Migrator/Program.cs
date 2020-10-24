using BillsManager.Services.Data;
using Billy.Billing.Models;
using Billy.Billing.Persistence.SQL.SQLite3;
using Billy.Billing.Persistence.SQL.SQLite3.Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using Bill_Old = BillsManager.Models.Bill;
//using Supplier_Old = BillsManager.Models.Supplier;

namespace Billy.Migrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting database migration");

            var oldDbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "old", "old.bmdb");

            Console.WriteLine("Loading old data ...");

            var oldProvider = new XMLDbSerializer(oldDbFilePath);
            oldProvider.Connect();

            var oldSuppliers = oldProvider.GetAllSuppliers();
            var oldBills = oldProvider.GetAllBills();
            var oldData = oldSuppliers.ToDictionary(supp => supp, supp => oldBills.Where(bill => bill.SupplierID == supp.ID));

            oldProvider.Disconnect();
            oldProvider = null;

            Console.WriteLine("Writing to new database ...");

            //var sqlite3Connection = new SQLiteConnection(SQLite3BillingConnectionFactory.ConnectionString);
            using (var sqlite3UowFactory = new SQLite3BillingUnitOfWorkFactory(
                  (conn, trans) => new DapperSQLite3SuppliersRepository(conn, trans),
                  (conn, trans) => new DapperSQLite3BillsRepository(conn, trans)))
            using (var uow = await sqlite3UowFactory.CreateAsync())
            {
                for (int i = 0; i < oldData.Count; i++)
                {
                    var oldDataEntry = oldData.ElementAt(i);

                    var newSuppData = new Dictionary<string, object>()
                    {
                        [nameof(Supplier.Email)] = oldDataEntry.Key.eMail?.TrimToNull(),
                        [nameof(Supplier.Fax)] = oldDataEntry.Key.Fax?.TrimToNull(),
                        [nameof(Supplier.Name)] = oldDataEntry.Key.Name?.TrimToNull(),
                        [nameof(Supplier.Notes)] = oldDataEntry.Key.Notes?.TrimToNull(),
                        [nameof(Supplier.Phone)] = oldDataEntry.Key.Phone?.TrimToNull(),
                        [nameof(Supplier.Website)] = oldDataEntry.Key.Website?.TrimToNull(),

                        [nameof(Supplier.Agent)] = new Dictionary<string, object>()
                        {
                            [nameof(Agent.Name)] = oldDataEntry.Key.AgentName?.TrimToNull(),
                            [nameof(Agent.Surname)] = oldDataEntry.Key.AgentSurname?.TrimToNull(),
                            [nameof(Agent.Phone)] = oldDataEntry.Key.AgentPhone?.TrimToNull()
                        },
                        [nameof(Supplier.Address)] = new Dictionary<string, object>()
                        {
                            [nameof(Address.City)] = oldDataEntry.Key.City?.TrimToNull(),
                            [nameof(Address.Country)] = oldDataEntry.Key.Country?.TrimToNull(),
                            [nameof(Address.Number)] = oldDataEntry.Key.Number?.TrimToNull(),
                            [nameof(Address.Province)] = oldDataEntry.Key.Province?.TrimToNull(),
                            [nameof(Address.Street)] = oldDataEntry.Key.Street?.TrimToNull(),
                            [nameof(Address.Zip)] = oldDataEntry.Key.Zip?.TrimToNull()
                        }
                    };

                    try
                    {
                        Supplier newSupp = await uow.Suppliers.CreateAndAddAsync(newSuppData);

                        foreach (var oldBill in oldDataEntry.Value)
                        {
                            var newBillData = new Dictionary<string, object>()
                            {
                                [nameof(Bill.AdditionalCosts)] = oldBill.AdditionalCosts,
                                [nameof(Bill.Agio)] = oldBill.Agio,
                                [nameof(Bill.Amount)] = oldBill.Amount,
                                [nameof(Bill.Code)] = oldBill.Code?.TrimToNull(),
                                [nameof(Bill.DueDate)] = oldBill.DueDate,
                                [nameof(Bill.Notes)] = oldBill.Notes?.TrimToNull(),
                                [nameof(Bill.PaymentDate)] = oldBill.PaymentDate,
                                [nameof(Bill.RegistrationDate)] = oldBill.RegistrationDate,
                                [nameof(Bill.ReleaseDate)] = oldBill.ReleaseDate,
                                [nameof(Bill.SupplierId)] = newSupp.Id,
                            };

                            try
                            {
                                _ = await uow.Bills.CreateAndAddAsync(newBillData);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        throw;
                    }
                }

                await uow.CommitAsync();
            }
        }
    }
}