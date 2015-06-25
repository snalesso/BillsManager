using BillsManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.Services.Data
{
    public partial class MockedDBConnector : IDBConnector
    {
        private readonly int suppliersCount, billsPerSupplierCount;

        private IList<Bill> bills;
        private IList<Supplier> suppliers;

        public MockedDBConnector(int suppliersCount, int billsPerSupplierCount)
        {
            this.billsPerSupplierCount = billsPerSupplierCount;
            this.suppliersCount = suppliersCount;
        }

        public MockedDBConnector()
            : this(8, 13)
        {
        }

        #region IDBConnector Members

        public bool Connect()
        {
            this.CreateFakeData();

            return true;
        }

        private void CreateFakeData()
        {
            this.CreateFakeSuppliers();
            this.CreateFakeBills();
            this.PaySomeBills();
        }

        private void CreateFakeBills()
        {
            if (this.bills == null)
                this.bills = new List<Bill>();

            var r = new Random();

            foreach (Supplier supplier in this.suppliers)
            {
                for (int i = 1; i <= this.billsPerSupplierCount; i++)
                {
                    this.bills.Add(
                        new Bill(
                            (uint)i,
                            supplier.ID,
                            DateTime.Today,
                            DateTime.Today.AddDays(r.Next(-2, 17 + 1)), // due
                            DateTime.Today.AddDays(-r.Next(7 + 1)), // release
                            (DateTime?)null, // payment
                            (double)r.Next(-2000, 2000 + 1),
                            (double)r.Next(40),
                            (double)r.Next(50),
                            string.Format("{0:00}{1:0000}", supplier.ID, i),
                            string.Format("Notes {0}-{1}", supplier.ID, i)));
                }
            }
        }

        private void CreateFakeSuppliers()
        {
            if (this.suppliers == null)
                this.suppliers = new List<Supplier>();

            for (int i = 1; i <= this.suppliersCount; i++)
            {
                this.suppliers.Add(
                    new Supplier(
                        (uint)i,
                        string.Format("Name of Supplier #{0:0000}", i),
                        i + "st.",
                        i.ToString(),
                        "City #" + i,
                        string.Format("{0:00000}", i),
                        "PV",
                        "Country #" + i,
                        string.Format("{0}@suppliers.bm", i),
                        string.Format("www.{0}.suppliers.bm", i),
                        string.Format("TEL {0:000 000 000 000}", i),
                        string.Format("FAX {0:000 000 000 000}", i),
                        "Notes of Supplier #" + i,
                        "AN #" + i,
                        "AS #" + i,
                        "AP #" + i
                        ));
            }
        }

        private void PaySomeBills()
        {
            if (suppliers.Count > 1)
            {
                var r = new Random();

                //var minPerc = (int)Math.Floor(this.suppliers.Count * 0.61);
                //var maxPerc = (int)Math.Floor(this.suppliers.Count * 0.78);

                //var suppliersToPayCount = r.Next(minPerc, maxPerc + 1);
                var suppliersToPayCount = (int)Math.Floor(this.suppliers.Count * 0.82);

                var paidSuppliers = new List<Supplier>();

                while (paidSuppliers.Count < suppliersToPayCount)
                {
                    var s = this.suppliers[r.Next(this.suppliers.Count)];
                    if (s == null)
                    {
                        throw new ArgumentNullException();
                    }
                    if (!paidSuppliers.Contains(s))
                    {
                        var sBills = this.bills.Where(b => b.SupplierID == s.ID);
                        foreach (Bill b in sBills)
                        {
                            b.PaymentDate = DateTime.Today; //b.ReleaseDate.AddDays(r.Next(0, DateTime.Today.Subtract(b.ReleaseDate).Days + 1));
                            if (b.PaymentDate < b.ReleaseDate)
                                throw new ArgumentOutOfRangeException();
                        }

                        paidSuppliers.Add(s);
                    }
                }
            }
        }

        public bool Save()
        {
            return true;
        }

        public void Disconnect()
        {
            this.bills = null;
            this.suppliers = null;
        }

        #endregion

        #region IBillsProvider Members

        public uint GetLastBillID()
        {
            var last = this.bills.LastOrDefault();

            if (last != null)
                return last.ID;

            return 0;
        }

        public IEnumerable<Models.Bill> GetAllBills()
        {
            return this.bills;
        }

        public bool Add(Models.Bill bill)
        {
            return true;
        }

        public bool Edit(Models.Bill bill)
        {
            return true;
        }

        public bool Delete(Models.Bill bill)
        {
            return true;
        }

        #endregion

        #region ISuppliersProvider Members

        public uint GetLastSupplierID()
        {
            var last = this.suppliers.LastOrDefault();

            if (last != null)
                return last.ID;

            return 0;
        }

        public IEnumerable<Models.Supplier> GetAllSuppliers()
        {
            return this.suppliers;
        }

        public bool Add(Models.Supplier supplier)
        {
            return true;
        }

        public bool Edit(Models.Supplier supplier)
        {
            return true;
        }

        public bool Delete(Models.Supplier supplier)
        {
            return true;
        }

        #endregion
    }
}
