using BillsManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace BillsManager.Services.Data
{
    public class XMLDBConnector : IDBConnector
    {
        #region fields

        private const string EXT_DB_DOTTED = @"bmdb";

        private const short START_INDEX = 0;

        private const string NS_DATABASE_ROOT = @"Database";

        private const string NS_BILLS = @"Bills";
        private const string ITEM_BILL = @"Bill";

        private const string NS_SUPPLIERS = @"Suppliers";
        private const string ITEM_SUPPLIER = @"Supplier";

        private const string NS_TAGS = @"Tags";
        private const string ITEM_TAG = @"Tag";

        private const string NS_AGENTS = @"Agents";
        private const string ITEM_AGENT = @"Agent";

        private const string ATT_CREATION_DATE = @"CreationDate";
        private const string ATT_LAST_ID = @"LastID";

        private readonly string dbPath;

        private XDocument xmlDB;

        #endregion

        #region ctor

        public XMLDBConnector(string dbPath)
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
                Debug.WriteLine(this.GetType().Name + " Connect() executed in " + sw.ElapsedMilliseconds + " ms");
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
                            (uint)i.Attribute("ID"),
                            (uint)i.Attribute("SupplierID"),
                            (DateTime)i.Attribute("RegistrationDate"),
                            (DateTime)i.Attribute("DueDate"),
                            (DateTime)i.Attribute("ReleaseDate"),
                            (DateTime?)i.Attribute("PaymentDate"),
                            (Double)i.Attribute("Amount"),
                            (Double)i.Attribute("Agio"),
                            (Double)i.Attribute("AdditionalCosts"),
                            (string)i.Attribute("Code"),
                            (string)i.Attribute("Notes"));
                });

            return q;

            //var query = from XBill in this.xmlDB.Root.Element(NS_BILLS).Elements(ITEM_BILL)
            //            select new
            //            Bill(
            //                (uint)XBill.Attribute("ID"),
            //                (uint)XBill.Attribute("SupplierID"),
            //                (DateTime)XBill.Attribute("RegistrationDate"),
            //                (DateTime)XBill.Attribute("DueDate"),
            //                (DateTime)XBill.Attribute("ReleaseDate"),
            //                (DateTime?)XBill.Attribute("PaymentDate"),
            //                (Double)XBill.Attribute("Amount"),
            //                (Double)XBill.Attribute("Agio"),
            //                (Double)XBill.Attribute("AdditionalCosts"),
            //                (string)XBill.Attribute("Code"),
            //                (string)XBill.Attribute("Notes")
            //            );

            //return query;
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
                .FirstOrDefault(elem => elem.Attribute("ID").Value == bill.ID.ToString());

            if (XBill == null)
                return false;

            XBill.ReplaceWith(this.GetXBill(bill));

            return true;
        }

        //public bool Edit(IEnumerable<Bill> bills)
        //{
        //    foreach (Bill b in bills)
        //    {
        //        var XBill = this.xmlDB.Root.Element(NS_BILLS)
        //            .Elements(ITEM_BILL)
        //            .Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

        //        typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //            .Where(pi => pi.Name != "ID")
        //            .ToList()
        //            .ForEach(pi => XBill.SetAttributeValue(pi.Name, pi.GetValue(b)));
        //    }

        //    return true;
        //}

        public bool Delete(Bill bill)
        {
            var xBill = this.xmlDB.Root.Element(NS_BILLS)
                .Elements()
                .FirstOrDefault(elem => elem.Attribute("ID").Value == bill.ID.ToString());

            if (xBill != null)
                xBill.Remove();

            return true;
        }

        //public bool Delete(IEnumerable<Bill> bills)
        //{
        //    foreach (Bill b in bills)
        //    {
        //        this.xmlDB.Root.Element(NS_BILLS)
        //            .Elements()
        //            .Single(elem => elem.Attribute("ID").Value == b.ID.ToString())
        //            .Remove();
        //    }

        //    return true;
        //}

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
                            (uint)i.Attribute("ID"),
                            (string)i.Attribute("Name"),
                            (string)i.Attribute("Street"),
                            (string)i.Attribute("Number"),
                            (string)i.Attribute("City"),
                            (string)i.Attribute("Zip"),
                            (string)i.Attribute("Province"),
                            (string)i.Attribute("Country"),
                            (string)i.Attribute("eMail"),
                            (string)i.Attribute("Website"),
                            (string)i.Attribute("Phone"),
                            (string)i.Attribute("Fax"),
                            (string)i.Attribute("Notes"),
                            (string)i.Attribute("AgentName"),
                            (string)i.Attribute("AgentSurname"),
                            (string)i.Attribute("AgentPhone"));
                });

            return q;

            //var query = from XSupplier in this.xmlDB.Root.Element(NS_SUPPLIERS)
            //                .Elements(ITEM_SUPPLIER)
            //            select new
            //            Supplier(
            //                (uint)XSupplier.Attribute("ID"),
            //                (string)XSupplier.Attribute("Name"),
            //                (string)XSupplier.Attribute("Street"),
            //                (string)XSupplier.Attribute("Number"),
            //                (string)XSupplier.Attribute("City"),
            //                (string)XSupplier.Attribute("Zip"),
            //                (string)XSupplier.Attribute("Province"),
            //                (string)XSupplier.Attribute("Country"),
            //                (string)XSupplier.Attribute("eMail"),
            //                (string)XSupplier.Attribute("Website"),
            //                (string)XSupplier.Attribute("Phone"),
            //                (string)XSupplier.Attribute("Fax"),
            //                (string)XSupplier.Attribute("Notes"),
            //                (string)XSupplier.Attribute("AgentName"),
            //                (string)XSupplier.Attribute("AgentSurname"),
            //                (string)XSupplier.Attribute("AgentPhone")
            //            );

            //return query;
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
                .FirstOrDefault(elem => elem.Attribute("ID").Value == supplier.ID.ToString());

            if (XSupplier == null)
                return false;

            XSupplier.ReplaceWith(this.GetXSupplier(supplier));

            return true;
        }

        //public bool Edit(IEnumerable<Supplier> suppliers)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Delete(Supplier supplier)
        {
            var xSupplier = this.xmlDB.Root.Element(NS_SUPPLIERS)
                .Elements()
                .FirstOrDefault(elem => elem.Attribute("ID").Value == supplier.ID.ToString());

            if (xSupplier != null)
                xSupplier.Remove();

            //this.DecreaseSuppliersCount();

            return true;
        }

        //public bool Delete(IEnumerable<Supplier> suppliers)
        //{
        //    throw new NotImplementedException();
        //}

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

        //#region tags provider

        //public uint GetLastTagID()
        //{
        //    return (uint)this.xmlDB.Root.Element(NS_TAGS).Attribute(ATT_LAST_ID);
        //}

        //public IEnumerable<Tag> GetAll()
        //{
        //    var query = from XTAG in this.xmlDB.Root.Element(NS_TAGS)
        //                    .Elements(ITEM_TAG)
        //                select new
        //                Tag(
        //                    (uint)XTAG.Attribute("ID"),
        //                    (string)XTAG.Attribute("Name"),
        //                    (string)XTAG.Attribute("Color"));

        //    return query;
        //}

        //public bool Add(Tag tag)
        //{
        //    this.xmlDB.Root.Element(NS_TAGS).Add(
        //        new XElement(ITEM_TAG,
        //            typeof(Tag)
        //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //            .Where(pi => pi.GetValue(tag) != null)
        //            .Select(pi => new XAttribute(pi.Name, pi.GetValue(tag)))));

        //    this.IncreaseLastIDValue(NS_TAGS);

        //    return true;
        //}

        //public bool Edit(Tag tag)
        //{
        //    var XTAG = this.xmlDB.Root.Element(NS_TAGS)
        //        .Elements(ITEM_TAG)
        //        .Single(elem => elem.Attribute("ID").Value == tag.ID.ToString());

        //    typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .Where(pi => pi.Name != "ID")
        //        .ToList()
        //        .ForEach(pi => XTAG.SetAttributeValue(pi.Name, pi.GetValue(tag)));

        //    return true;
        //}

        ////public bool Edit(IEnumerable<Tag> tags)
        ////{
        ////    foreach (Tag b in tags)
        ////    {
        ////        var XTag = this.xmlDB.Root.Element(NS_TAGS)
        ////            .Elements(ITEM_TAG)
        ////            .Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

        ////        typeof(Bill)
        ////            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        ////            .Where(pi => pi.Name != "ID")
        ////            .ToList()
        ////            .ForEach(pi => XTag.SetAttributeValue(pi.Name, pi.GetValue(b)));
        ////    }

        ////    return true;
        ////}

        //public bool Delete(Tag tag) // URGENT: add fail logic
        //{
        //    this.xmlDB.Root.Element(NS_TAGS)
        //        .Elements()
        //        .Single(elem => elem.Attribute("ID").Value == tag.ID.ToString())
        //        .Remove();

        //    return true;
        //}

        ////public bool Delete(IEnumerable<Tag> tags)
        ////{
        ////    foreach (Tag b in tags)
        ////    {
        ////        this.xmlDB.Root.Element(NS_TAGS)
        ////            .Elements()
        ////            .Single(elem => elem.Attribute("ID").Value == b.ID.ToString())
        ////            .Remove();
        ////    }

        ////    return true;
        ////}

        //#endregion

        /*#region agents provider

        public uint GetLastAgentID()
        {
            return (uint)this.xmlDB.Root.Element(ITEM_AGENT).Attribute(ATT_LAST_ID);
        }

        public IEnumerable<Agent> GetAllAgents()
        {
            var query = from XAgent in this.xmlDB.Root.Element(NS_AGENTS).Elements(ITEM_AGENT)
                        select new
                        Agent(
                        (uint)XAgent.Attribute("ID"),
                        (string)XAgent.Attribute("Name"),
                        (string)XAgent.Attribute("Surname"),
                        (string)XAgent.Attribute("FirstPhoneNumber"),
                        (string)XAgent.Attribute("SecondPhoneNumber")
                        );

            return query;
        }

        public Agent GetAgentByID(uint agentID)
        {
            var agent = from XAgent in this.xmlDB.Root.Element(NS_AGENTS).Elements(ITEM_AGENT)
                        where XAgent.Attribute("ID").Value == agentID.ToString()
                        select new
                        Agent(
                        (uint)XAgent.Attribute("ID"),
                        (string)XAgent.Attribute("Name"),
                        (string)XAgent.Attribute("Surname"),
                        (string)XAgent.Attribute("FirstPhoneNumber"),
                        (string)XAgent.Attribute("SecondPhoneNumber")
                        );

            return agent.FirstOrDefault();
        }

        public bool Add(Agent agent)
        {
            this.xmlDB.Root.Element(NS_AGENTS).Add(new XElement(ITEM_AGENT, typeof(Agent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(agent, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(agent, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(NS_AGENTS);

            return true;
        }

        public bool Edit(Agent agent)
        {
            var XAgent = this.xmlDB.Root.Element(NS_AGENTS).Elements(ITEM_AGENT)
                .Single(elem => elem.Attribute("ID").Value == agent.ID.ToString());

            typeof(Agent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.Name != "ID")
                .ToList()
                .ForEach(pi =>
                {
                    XAgent.SetAttributeValue(pi.Name, pi.GetValue(agent, null));
                });

            return true;
        }

        //public bool Edit(IEnumerable<Agent> agents)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Delete(Agent agent)
        {
            this.xmlDB.Root.Element(NS_AGENTS)
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == agent.ID.ToString())
                .Remove();

            return true;
        }

        //public bool Delete(IEnumerable<Agent> agents)
        //{
        //    throw new NotImplementedException();
        //}

#endregion*/

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
                    new XElement(NS_SUPPLIERS, new XAttribute(ATT_LAST_ID, START_INDEX)),
                    new XElement(NS_TAGS, new XAttribute(ATT_LAST_ID, START_INDEX))/*,
                        new XElement(NS_AGENTS, new XAttribute(ATT_LAST_ID, START_INDEX))*/));

            Directory.CreateDirectory(Path.GetDirectoryName(this.dbPath));

            newXDoc.Save(this.dbPath);
        }

        #endregion

        #endregion
    }
}