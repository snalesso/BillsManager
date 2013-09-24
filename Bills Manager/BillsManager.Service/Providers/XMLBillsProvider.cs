using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public class XMLBillsProvider : IBillsProvider
    {
        #region fields

        private readonly string billsDBFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        private readonly string billsDBFileName = @"Bills";
        private readonly string dbExt = ".bmdb";

        private XDocument xmlBillsDB;

        #endregion

        #region methods

        public uint GetLastID()
        {
            this.EnsureXDocumentIsInitialized();

            return (uint)this.xmlBillsDB.Root.Element("Bills").Attribute("LastID");
        }

        public IEnumerable<Bill> GetAll()
        {
            this.EnsureXDocumentIsInitialized();

            var query = from XBill in this.xmlBillsDB.Root.Element("Bills").Elements("Bill")
                        select new
                        Bill(
                        (uint)XBill.Attribute("ID"),
                        (DateTime)XBill.Attribute("RegistrationDate"),
                        (DateTime)XBill.Attribute("DueDate"),
                        (DateTime?)XBill.Attribute("PaymentDate"),
                        (DateTime)XBill.Attribute("ReleaseDate"),
                        (double)XBill.Attribute("Amount"),
                        (uint)XBill.Attribute("SupplierID"),
                        (string)XBill.Attribute("Notes"),
                        (string)XBill.Attribute("Code")
                        );

            return query;

            //var bills = new List<Bill>();

            //foreach (XElement XBill in this.xmlBillsDB.Root.Element("Bills").Elements("Bill"))
            //{
            //    var newBill = new Bill((uint)XBill.Attribute("ID"));

            //    newBill.RegistrationDate = (DateTime)XBill.Attribute("RegistrationDate");
            //    newBill.DueDate = (DateTime)XBill.Attribute("DueDate");
            //    newBill.PaymentDate = (DateTime?)XBill.Attribute("PaymentDate");
            //    newBill.ReleaseDate = (DateTime)XBill.Attribute("ReleaseDate");
            //    newBill.Amount = (double)XBill.Attribute("Amount");
            //    newBill.Supplier = (string)XBill.Attribute("Supplier");
            //    newBill.Notes = (string)XBill.Attribute("Notes");
            //    newBill.Code = (string)XBill.Attribute("Code");

            //    bills.Add(newBill);
            //}

            //return bills;
        }

        public bool Add(Bill bill)
        {
            this.EnsureXDocumentIsInitialized();

            this.xmlBillsDB.Root.Element("Bills").Add(new XElement("Bill", typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(bill, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(bill, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue();

            //this.IncreaseBillsCount();

            this.SaveXDocument();

            return true;
        }

        public bool Edit(Bill bill)
        {
            this.EnsureXDocumentIsInitialized();

            var XBill = this.xmlBillsDB.Root.Element("Bills").Elements("Bill").Single(elem => elem.Attribute("ID").Value == bill.ID.ToString());

            typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
            {
                XBill.SetAttributeValue(pi.Name, pi.GetValue(bill, null));
            });

            this.SaveXDocument();

            return true;
        }

        public bool Edit(IEnumerable<Bill> bills)
        {
            this.EnsureXDocumentIsInitialized();

            foreach (Bill b in bills)
            {

                var XBill = this.xmlBillsDB.Root.Element("Bills").Elements("Bill").Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

                typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
                {
                    XBill.SetAttributeValue(pi.Name, pi.GetValue(b, null));
                });
            }

            this.SaveXDocument();

            return true;
        }

        public bool Delete(Bill bill)
        {
            this.EnsureXDocumentIsInitialized();

            this.xmlBillsDB.Root.Element("Bills").Elements().Single(elem => elem.Attribute("ID").Value == bill.ID.ToString()).Remove();

            //this.DecreaseBillsCount();

            this.SaveXDocument();

            return true;
        }

        public bool Delete(IEnumerable<Bill> bills)
        {
            this.EnsureXDocumentIsInitialized();

            foreach (Bill b in bills)
            {
                this.xmlBillsDB.Root.Element("Bills").Elements().Single(elem => elem.Attribute("ID").Value == b.ID.ToString()).Remove();
                //this.DecreaseBillsCount();
            }

            this.SaveXDocument();

            return true;
        }

        #region support methods

        void EnsureXDocumentIsInitialized()
        {
            if (this.xmlBillsDB == null || this.xmlBillsDB.BaseUri != this.billsDBFolder + this.billsDBFileName + this.dbExt)
            {
                this.EnsureDBFileExists();

                this.xmlBillsDB = XDocument.Load(this.billsDBFolder + this.billsDBFileName + this.dbExt);
            }
        }

        void EnsureDBFileExists()
        {
            if (!System.IO.Directory.Exists(this.billsDBFolder))
            {
                System.IO.Directory.CreateDirectory(this.billsDBFolder);
                this.CreateXMLDatabase();
            }
            else
            {
                if (!System.IO.File.Exists(this.billsDBFolder + this.billsDBFileName + this.dbExt))
                {
                    this.CreateXMLDatabase();
                }
            }
        }

        void CreateXMLDatabase()
        {
            var newXDoc = new XDocument();

            newXDoc.Declaration = new XDeclaration("1.0", "utf-8", null);
            newXDoc.Add(new XElement("BillsDatabase", new XElement("Bills", new XAttribute("LastID", 0)/*, new XAttribute("BillsCount", 0)*/)));
            newXDoc.Root.Add(new XAttribute("CreationDate", DateTime.Today));

            newXDoc.Save(this.billsDBFolder + this.billsDBFileName + this.dbExt);
        }

        void IncreaseLastIDValue()
        {
            this.xmlBillsDB.Root.Element("Bills").Attribute("LastID").SetValue(uint.Parse(this.xmlBillsDB.Root.Element("Bills").Attribute("LastID").Value) + 1);
        }

        /*void IncreaseBillsCount()
        {
            this.xmlBillsDB.Root.Element("Bills").Attribute("BillsCount").SetValue(uint.Parse(this.xmlBillsDB.Root.Element("Bills").Attribute("BillsCount").Value) + 1);
        }

        void DecreaseBillsCount()
        {
            this.xmlBillsDB.Root.Element("Bills").Attribute("BillsCount").SetValue(uint.Parse(this.xmlBillsDB.Root.Element("Bills").Attribute("BillsCount").Value) - 1);
        }*/

        void SaveXDocument()
        {
            this.xmlBillsDB.Save(this.billsDBFolder + this.billsDBFileName + this.dbExt);
        }

        #endregion

        #endregion
    }
}