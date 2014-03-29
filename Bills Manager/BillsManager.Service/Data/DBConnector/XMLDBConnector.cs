using BillsManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace BillsManager.Services.Providers
{
    public class XMLDBConnector : IDBConnector
    {
        #region fields

        private const string EXT_DB_DOTTED = @"bmdb";

        private const short START_INDEX = 0;

        private const string NS_DATABASE = @"Database";

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

        private XDocument xmlDB;

        #endregion

        #region ctor

        public XMLDBConnector(string fullDbPath)
        {
            this.EnsureDbFileExists(fullDbPath);

            this.DBPath = fullDbPath;
        }

        #endregion

        #region properties

        private string dbPath;
        public string DBPath
        {
            get { return this.dbPath; }
            private set
            {
                if (this.dbPath == value) return;

                this.dbPath = value;
            }
        }

        /*public string DBName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.DBPath); }
        }*/

        #endregion

        #region methods

        #region connector level methods

        public bool Open()
        {
            try
            {
                this.xmlDB = XDocument.Load(this.DBPath);
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
                this.xmlDB.Save(this.DBPath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Close()
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
            var query = from XBill in this.xmlDB.Root.Element(NS_BILLS).Elements(ITEM_BILL)
                        select new
                        Bill(
                            (uint)XBill.Attribute("ID"),
                            /*(uint)XBill.Attribute("TagID"),*/
                            (DateTime)XBill.Attribute("RegistrationDate"),
                            (DateTime)XBill.Attribute("DueDate"),
                            (DateTime?)XBill.Attribute("PaymentDate"),
                            (DateTime)XBill.Attribute("ReleaseDate"),
                            (double)XBill.Attribute("Amount"),
                            /*(double)XBill.Attribute("Gain"),
                            (double)XBill.Attribute("Expense"),*/
                            (uint)XBill.Attribute("SupplierID"),
                            (string)XBill.Attribute("Notes"),
                            (string)XBill.Attribute("Code")
                        );

            return query;
        }

        public bool Add(Bill bill)
        {
            this.xmlDB.Root.Element(NS_BILLS).Add(new XElement(ITEM_BILL, typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(bill, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(bill, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(NS_BILLS);

            return true;
        }

        public bool Edit(Bill bill)
        {
            var XBill = this.xmlDB.Root.Element(NS_BILLS).Elements(ITEM_BILL)
                .Single(elem => elem.Attribute("ID").Value == bill.ID.ToString());

            typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.Name != "ID")
                .ToList()
                .ForEach(pi =>
                {
                    XBill.SetAttributeValue(pi.Name, pi.GetValue(bill, null));
                });

            return true;
        }

        public bool Edit(IEnumerable<Bill> bills)
        {
            foreach (Bill b in bills)
            {
                var XBill = this.xmlDB.Root.Element(NS_BILLS).Elements(ITEM_BILL).Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

                typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
                {
                    XBill.SetAttributeValue(pi.Name, pi.GetValue(b, null));
                });
            }

            return true;
        }

        public bool Delete(Bill bill)
        {
            this.xmlDB.Root.Element(NS_BILLS)
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == bill.ID.ToString())
                .Remove();

            return true;
        }

        public bool Delete(IEnumerable<Bill> bills)
        {
            foreach (Bill b in bills)
            {
                this.xmlDB.Root.Element(NS_BILLS).Elements().Single(elem => elem.Attribute("ID").Value == b.ID.ToString()).Remove();
            }

            return true;
        }

        #endregion

        #region suppliers provider

        public uint GetLastSupplierID()
        {
            return (uint)this.xmlDB.Root.Element(NS_SUPPLIERS).Attribute(ATT_LAST_ID);
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            var query = from XSupplier in this.xmlDB.Root.Element(NS_SUPPLIERS).Elements(ITEM_SUPPLIER)
                        select new
                        Supplier(
                            uint.Parse(XSupplier.Attribute("ID").Value),
                            (string)XSupplier.Attribute("Name"),
                            (string)XSupplier.Attribute("Street"),
                            (string)XSupplier.Attribute("Number"),
                            (string)XSupplier.Attribute("City"),
                            (string)XSupplier.Attribute("Zip"),
                            (string)XSupplier.Attribute("Province"),
                            (string)XSupplier.Attribute("Country"),
                            (string)XSupplier.Attribute("eMail"),
                            (string)XSupplier.Attribute("Website"),
                            (string)XSupplier.Attribute("Phone"),
                            (string)XSupplier.Attribute("Fax"),
                            (string)XSupplier.Attribute("Notes"),
                            (string)XSupplier.Attribute("AgentName"),
                            (string)XSupplier.Attribute("AgentSurname"),
                            (string)XSupplier.Attribute("AgentPhone")
                        );

            return query;
        }

        public bool Add(Supplier supplier)
        {
            this.xmlDB.Root.Element(NS_SUPPLIERS).Add(new XElement(ITEM_SUPPLIER, typeof(Supplier).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(supplier, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(supplier, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(NS_SUPPLIERS);

            return true;
        }

        public bool Edit(Supplier supplier)
        {
            var XSupplier = this.xmlDB.Root.Element(NS_SUPPLIERS).Elements(ITEM_SUPPLIER).Single(elem => elem.Attribute("ID").Value == supplier.ID.ToString());

            typeof(Supplier).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
            {
                XSupplier.SetAttributeValue(pi.Name, pi.GetValue(supplier, null));
            });

            return true;
        }

        public bool Edit(IEnumerable<Supplier> suppliers)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Supplier supplier)
        {
            this.xmlDB.Root.Element(NS_SUPPLIERS).Elements().Single(elem => elem.Attribute("ID").Value == supplier.ID.ToString()).Remove();

            //this.DecreaseSuppliersCount();

            return true;
        }

        public bool Delete(IEnumerable<Supplier> suppliers)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region tags provider

        public uint GetLastTagID()
        {
            return (uint)this.xmlDB.Root.Element(NS_TAGS).Attribute(ATT_LAST_ID);
        }

        public IEnumerable<Tag> GetAll()
        {
            var query = from XTAG in this.xmlDB.Root.Element(NS_TAGS).Elements(ITEM_TAG)
                        select new
                        Tag(
                            uint.Parse(XTAG.Attribute("ID").Value),
                            (string)XTAG.Attribute("Name"),
                            (string)XTAG.Attribute("Color")
                        );

            return query;
        }

        public bool Add(Tag tag)
        {
            this.xmlDB.Root.Element(NS_TAGS).Add(new XElement(ITEM_TAG, typeof(Tag).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(tag, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(tag, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(NS_TAGS);

            return true;
        }

        public bool Edit(Tag tag)
        {
            var XTAG = this.xmlDB.Root.Element(NS_TAGS).Elements(ITEM_TAG)
                .Single(elem => elem.Attribute("ID").Value == tag.ID.ToString());

            typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.Name != "ID")
                .ToList()
                .ForEach(pi =>
                {
                    XTAG.SetAttributeValue(pi.Name, pi.GetValue(tag, null));
                });

            return true;
        }

        public bool Edit(IEnumerable<Tag> tags)
        {
            foreach (Tag b in tags)
            {
                var XTag = this.xmlDB.Root.Element(NS_TAGS).Elements(ITEM_TAG).Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

                typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
                {
                    XTag.SetAttributeValue(pi.Name, pi.GetValue(b, null));
                });
            }

            return true;
        }

        public bool Delete(Tag tag) // URGENT: add fail logic
        {
            this.xmlDB.Root.Element(NS_TAGS)
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == tag.ID.ToString())
                .Remove();

            return true;
        }

        public bool Delete(IEnumerable<Tag> tags)
        {
            foreach (Tag b in tags)
            {
                this.xmlDB.Root.Element(NS_TAGS).Elements().Single(elem => elem.Attribute("ID").Value == b.ID.ToString()).Remove();
            }

            return true;
        }

        #endregion

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

        public bool Edit(IEnumerable<Agent> agents)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Agent agent)
        {
            this.xmlDB.Root.Element(NS_AGENTS)
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == agent.ID.ToString())
                .Remove();

            return true;
        }

        public bool Delete(IEnumerable<Agent> agents)
        {
            throw new NotImplementedException();
        }

        #endregion*/

        #region support methods

        void IncreaseLastIDValue(string ns)
        {
            this.xmlDB.Root.Element(ns).Attribute(ATT_LAST_ID)
                .SetValue(uint.Parse(this.xmlDB.Root.Element(ns).Attribute(ATT_LAST_ID).Value) + 1);
        }

        private void EnsureDbFileExists(string fullDbFilePath)
        {
            if (File.Exists(fullDbFilePath)) return;

            var newXDoc = new XDocument();

            try
            {
                newXDoc.Declaration = new XDeclaration("1.0", "utf-8", null);
                newXDoc.Add(
                    new XElement(
                        NS_DATABASE,
                        new XAttribute(ATT_CREATION_DATE, DateTime.Today),
                        new XElement(NS_BILLS, new XAttribute(ATT_LAST_ID, START_INDEX)),
                        new XElement(NS_SUPPLIERS, new XAttribute(ATT_LAST_ID, START_INDEX)),
                        new XElement(NS_TAGS, new XAttribute(ATT_LAST_ID, START_INDEX))/*,
                        new XElement(NS_AGENTS, new XAttribute(ATT_LAST_ID, START_INDEX))*/));

                Directory.CreateDirectory(Path.GetDirectoryName(fullDbFilePath));

                newXDoc.Save(fullDbFilePath);
            }
            catch { }
        }

        #endregion

        #endregion
    }
}