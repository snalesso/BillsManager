using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BillsManager.Model;
using System.Linq;
using System.Reflection;

namespace BillsManager.Service.Providers
{
    public class XMLSuppliersProvider : ISuppliersProvider
    {
        #region fields

        private readonly string suppliersDBFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        private readonly string suppliersDBFileName = @"Suppliers";
        private readonly string dbExt = ".bmdb";

        private XDocument xmlSuppliersDB;

        #endregion

        #region methods

        public uint GetLastID()
        {
            this.EnsureXDocumentIsInitialized();

            return (uint)this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("LastID");
        }

        public IEnumerable<Supplier> GetAll()
        {
            this.EnsureXDocumentIsInitialized();

            var query = from XBill in this.xmlSuppliersDB.Root.Element("Suppliers").Elements("Supplier")
                        select new
                        Supplier(
                        uint.Parse(XBill.Attribute("ID").Value),
                        (string)XBill.Attribute("Name"),
                        (string)XBill.Attribute("Street"),
                        (string)XBill.Attribute("Number"),
                        (string)XBill.Attribute("City"),
                        ushort.Parse(XBill.Attribute("Zip").Value),
                        (string)XBill.Attribute("Province"),
                        (string)XBill.Attribute("Country"),
                        (string)XBill.Attribute("eMail"),
                        (string)XBill.Attribute("Website"),
                        (string)XBill.Attribute("Phone"),
                        (string)XBill.Attribute("Notes")
                        );

            return query;
        }

        public bool Add(Supplier supplier)
        {
            this.EnsureXDocumentIsInitialized();

            this.xmlSuppliersDB.Root.Element("Suppliers").Add(new XElement("Supplier", typeof(Supplier).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(supplier, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(supplier, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue();

            this.IncreaseSuppliersCount();

            this.SaveXDocument();

            return true;
        }

        public bool Edit(Supplier supplier)
        {
            this.EnsureXDocumentIsInitialized();

            var XSupplier = this.xmlSuppliersDB.Root.Element("Suppliers").Elements("Supplier").Single(elem => elem.Attribute("ID").Value == supplier.ID.ToString());

            typeof(Supplier).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
            {
                XSupplier.SetAttributeValue(pi.Name, pi.GetValue(supplier, null));
            });

            this.SaveXDocument();

            return true;
        }

        public bool Delete(Supplier supplier)
        {
            this.EnsureXDocumentIsInitialized();

            this.xmlSuppliersDB.Root.Element("Suppliers").Elements().Single(elem => elem.Attribute("ID").Value == supplier.ID.ToString()).Remove();

            this.DecreaseSuppliersCount();

            this.SaveXDocument();

            return true;
        }

        #region support methods

        void EnsureXDocumentIsInitialized()
        {
            if (this.xmlSuppliersDB == null || this.xmlSuppliersDB.BaseUri != this.suppliersDBFolder + this.suppliersDBFileName + this.dbExt)
            {
                this.EnsureDBFileExists();

                this.xmlSuppliersDB = XDocument.Load(this.suppliersDBFolder + this.suppliersDBFileName + this.dbExt);
            }
        }

        void EnsureDBFileExists()
        {
            if (!System.IO.Directory.Exists(this.suppliersDBFolder))
            {
                System.IO.Directory.CreateDirectory(this.suppliersDBFolder);
                this.CreateXMLDatabase();
            }
            else
            {
                if (!System.IO.File.Exists(this.suppliersDBFolder + this.suppliersDBFileName + this.dbExt))
                {
                    this.CreateXMLDatabase();
                }
            }
        }

        void CreateXMLDatabase()
        {
            var newXDoc = new XDocument();

            newXDoc.Declaration = new XDeclaration("1.0", "utf-8", null);
            newXDoc.Add(new XElement("SuppliersDatabase", new XElement("Suppliers", new XAttribute("LastID", 0), new XAttribute("SuppliersCount", 0))));
            newXDoc.Root.Add(new XAttribute("CreationDate", DateTime.Today));

            newXDoc.Save(this.suppliersDBFolder + this.suppliersDBFileName + this.dbExt);
        }

        void IncreaseLastIDValue()
        {
            this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("LastID").SetValue(uint.Parse(this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("LastID").Value) + 1);
        }

        void IncreaseSuppliersCount()
        {
            this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("SuppliersCount").SetValue(uint.Parse(this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("SuppliersCount").Value) + 1);
        }

        void DecreaseSuppliersCount()
        {
            this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("SuppliersCount").SetValue(uint.Parse(this.xmlSuppliersDB.Root.Element("Suppliers").Attribute("SuppliersCount").Value) - 1);
        }

        void SaveXDocument()
        {
            this.xmlSuppliersDB.Save(this.suppliersDBFolder + this.suppliersDBFileName + this.dbExt);
        }

        #endregion

        #endregion
    }
}
