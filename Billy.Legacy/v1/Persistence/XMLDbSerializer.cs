using BillsManager.v1.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace BillsManager.v1.Services.Data
{
    public class XMLDbSerializer : IDBConnector
    {
        #region fields

        private const string EXT_DB_DOTTED = @"bmdb";

        private const short START_INDEX = 0;

        private const string NS_DATABASE_ROOT = @"Database";

        private const string NS_BILLS = nameof(Bill) + "s";
        private const string ITEM_BILL = nameof(Bill);

        private const string NS_SUPPLIERS = nameof(Supplier)+"s";
        private const string ITEM_SUPPLIER = nameof(Supplier);

        private const string ATT_CREATION_DATE = @"CreationDate";
        private const string ATT_LAST_ID = @"LastID";

        private readonly string dbPath;

        private XDocument xmlDB;

        #endregion

        #region ctor

        public XMLDbSerializer(string dbPath)
        {
            this.dbPath = dbPath;
            this.EnsureDbFileExists();
        }

        #endregion

        #region methods

        #region connector level methods

        public bool Connect()
        {
            try
            {
#if DEBUG
                var sw = new Stopwatch();
                sw.Start();
#endif
                this.xmlDB = XDocument.Load(this.dbPath);
#if DEBUG
                sw.Stop();
                Debug.WriteLine(string.Empty);
                Debug.WriteLine(this.GetType().Name + " Connect() executed in " + sw.Elapsed);
                Debug.WriteLine(string.Empty);
#endif
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Save()
        {
            try
            {
                this.xmlDB.Save(this.dbPath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Disconnect()
        {
            this.xmlDB = null;
        }

        #endregion

        #region bills provider

        public uint GetLastBillID()
        {
            return (uint)this.xmlDB.Root.Element(NS_BILLS).Attribute(ATT_LAST_ID);
        }

        public IEnumerable<Bill> GetAllBills()
        {
            var q =
                this.xmlDB.Root
                .Element(NS_BILLS)
                .Elements(ITEM_BILL)
                .Select(
                i =>
                {
                    return
                        new Bill(
                            (uint)i.Attribute(nameof(Bill.ID)),
                            (uint)i.Attribute(nameof(Bill.SupplierID)),
                            (DateTime)i.Attribute(nameof(Bill.RegistrationDate)),
                            (DateTime)i.Attribute(nameof(Bill.DueDate)),
                            (DateTime)i.Attribute(nameof(Bill.ReleaseDate)),
                            (DateTime?)i.Attribute(nameof(Bill.PaymentDate)),
                            (Double)i.Attribute(nameof(Bill.Amount)),
                            (Double)i.Attribute(nameof(Bill.Agio)),
                            (Double)i.Attribute(nameof(Bill.AdditionalCosts)),
                            (string)i.Attribute(nameof(Bill.Code)),
                            (string)i.Attribute(nameof(Bill.Notes)));
                });

            return q;
        }

        public bool Add(Bill bill)
        {
            this.xmlDB.Root.Element(NS_BILLS).Add(this.GetXBill(bill));

            this.IncreaseLastIDValue(NS_BILLS);

            return true;
        }

        public bool Edit(Bill bill)
        {
            var XBill = this.xmlDB.Root.Element(NS_BILLS)
                .Elements(ITEM_BILL)
                .FirstOrDefault(elem => elem.Attribute(nameof(Bill.ID)).Value == bill.ID.ToString());

            if (XBill == null)
                return false;

            XBill.ReplaceWith(this.GetXBill(bill));

            return true;
        }

        public bool Delete(Bill bill)
        {
            var xBill = this.xmlDB.Root.Element(NS_BILLS)
                .Elements()
                .FirstOrDefault(elem => elem.Attribute(nameof(Bill.ID)).Value == bill.ID.ToString());

            if (xBill != null)
                xBill.Remove();

            return true;
        }

        private XElement GetXBill(Bill bill)
        {
            return
                new XElement(ITEM_BILL,
                    typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => pi.GetValue(bill) != null)
                    .Select(pi => new XAttribute(pi.Name, pi.GetValue(bill))));
        }

        #endregion

        #region suppliers provider

        public uint GetLastSupplierID()
        {
            return (uint)this.xmlDB.Root.Element(NS_SUPPLIERS).Attribute(ATT_LAST_ID);
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            var q =
                this.xmlDB.Root
                .Element(NS_SUPPLIERS)
                .Elements(ITEM_SUPPLIER)
                .Select(
                i =>
                {
                    return
                        new Supplier(
                            (uint)i.Attribute(nameof(Supplier.ID)),
                            (string)i.Attribute(nameof(Supplier.Name)),
                            (string)i.Attribute(nameof(Supplier.Street)),
                            (string)i.Attribute(nameof(Supplier.Number)),
                            (string)i.Attribute(nameof(Supplier.City)),
                            (string)i.Attribute(nameof(Supplier.Zip)),
                            (string)i.Attribute(nameof(Supplier.Province)),
                            (string)i.Attribute(nameof(Supplier.Country)),
                            (string)i.Attribute(nameof(Supplier.eMail)),
                            (string)i.Attribute(nameof(Supplier.Website)),
                            (string)i.Attribute(nameof(Supplier.Phone)),
                            (string)i.Attribute(nameof(Supplier.Fax)),
                            (string)i.Attribute(nameof(Supplier.Notes)),
                            (string)i.Attribute(nameof(Supplier.AgentName)),
                            (string)i.Attribute(nameof(Supplier.AgentSurname)),
                            (string)i.Attribute(nameof(Supplier.AgentPhone)));
                });

            return q;
        }

        public bool Add(Supplier supplier)
        {
            this.xmlDB.Root.Element(NS_SUPPLIERS).Add(this.GetXSupplier(supplier));

            this.IncreaseLastIDValue(NS_SUPPLIERS);

            return true;
        }

        public bool Edit(Supplier supplier)
        {
            var XSupplier = this.xmlDB.Root.Element(NS_SUPPLIERS)
                .Elements(ITEM_SUPPLIER)
                .FirstOrDefault(elem => elem.Attribute(nameof(Supplier.ID)).Value == supplier.ID.ToString());

            if (XSupplier == null)
                return false;

            XSupplier.ReplaceWith(this.GetXSupplier(supplier));

            return true;
        }

        public bool Delete(Supplier supplier)
        {
            var xSupplier = this.xmlDB.Root.Element(NS_SUPPLIERS)
                .Elements()
                .FirstOrDefault(elem => elem.Attribute(nameof(Supplier.ID)).Value == supplier.ID.ToString());

            if (xSupplier != null)
                xSupplier.Remove();

            return true;
        }

        private XElement GetXSupplier(Supplier supplier)
        {
            return
                new XElement(ITEM_SUPPLIER,
                    typeof(Supplier)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => pi.GetValue(supplier) != null)
                    .Select(pi => new XAttribute(pi.Name, pi.GetValue(supplier))));
        }

        #endregion
        
        #region support methods

        void IncreaseLastIDValue(string ns)
        {
            this.xmlDB.Root.Element(ns).Attribute(ATT_LAST_ID)
                .SetValue(uint.Parse(this.xmlDB.Root.Element(ns).Attribute(ATT_LAST_ID).Value) + 1);
        }

        private void EnsureDbFileExists()
        {
            if (File.Exists(this.dbPath)) return;

            var newXDoc = new XDocument();

            newXDoc.Declaration = new XDeclaration("1.0", "utf-8", null);
            newXDoc.Add(
                new XElement(
                    NS_DATABASE_ROOT,
                    new XAttribute(ATT_CREATION_DATE, DateTime.Today),
                    new XElement(NS_BILLS, new XAttribute(ATT_LAST_ID, START_INDEX)),
                    new XElement(NS_SUPPLIERS, new XAttribute(ATT_LAST_ID, START_INDEX))));

            Directory.CreateDirectory(Path.GetDirectoryName(this.dbPath));

            newXDoc.Save(this.dbPath);
        }

        #endregion

        #endregion
    }
}