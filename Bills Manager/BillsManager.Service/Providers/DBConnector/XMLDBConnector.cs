using BillsManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace BillsManager.Services.Providers
{
    public class XMLDBConnector : IDBConnector
    {
        #region fields

        // TODO: make constants
        private readonly string dbFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        //private readonly string dbExtension = @"bmdb";

        //private readonly string databaseNamespace = @"Database";

        private readonly string billsNamespace = @"Bills";
        private readonly string billItem = @"Bill";

        private readonly string suppliersNamespace = @"Suppliers";
        private readonly string supplierItem = @"Supplier";

        private readonly string agentsNamespace = @"Agents";
        private readonly string agentItem = @"Agent";

        //private readonly string creationDateAttribute = @"CreationDate";
        private readonly string lastIDAttribute = @"LastID";

        private XDocument xmlDB;

        #endregion

        #region ctor

        public XMLDBConnector(string dbPath)
        {
            if (!System.IO.File.Exists(dbPath))
                throw new System.IO.FileNotFoundException("Couldn't find DB file.", dbPath); // TODO: language

            this.Path = dbPath;
        }

        #endregion

        #region properties

        private string path;
        public string Path
        {
            get
            {
                return this.path;
            }
            private set
            {
                if (this.path != value)
                    this.path = value;
            }
        }

        public string DBName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.Path); }
        }

        #endregion

        #region methods

        #region connector level methods

        public bool Open()
        {
            try
            {
                this.xmlDB = XDocument.Load(this.Path);
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
                this.xmlDB.Save(this.Path);
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
            return (uint)this.xmlDB.Root.Element(this.billsNamespace).Attribute(this.lastIDAttribute);
        }

        public IEnumerable<Bill> GetAllBills()
        {
            var query = from XBill in this.xmlDB.Root.Element(this.billsNamespace).Elements(this.billItem)
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
        }

        public bool Add(Bill bill)
        {
            this.xmlDB.Root.Element(this.billsNamespace).Add(new XElement(this.billItem, typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(bill, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(bill, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(this.billsNamespace);

            return true;
        }

        public bool Edit(Bill bill)
        {
            var XBill = this.xmlDB.Root.Element(this.billsNamespace).Elements(this.billItem)
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
                var XBill = this.xmlDB.Root.Element(this.billsNamespace).Elements(this.billItem).Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

                typeof(Bill).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.Name != "ID").ToList().ForEach(pi =>
                {
                    XBill.SetAttributeValue(pi.Name, pi.GetValue(b, null));
                });
            }

            return true;
        }

        public bool Delete(Bill bill)
        {
            this.xmlDB.Root.Element(this.billsNamespace)
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == bill.ID.ToString())
                .Remove();

            return true;
        }

        public bool Delete(IEnumerable<Bill> bills)
        {
            foreach (Bill b in bills)
            {
                this.xmlDB.Root.Element("Bills").Elements().Single(elem => elem.Attribute("ID").Value == b.ID.ToString()).Remove();
            }

            return true;
        }

        #endregion

        #region suppliers provider

        public uint GetLastSupplierID()
        {
            return (uint)this.xmlDB.Root.Element(this.suppliersNamespace).Attribute(this.lastIDAttribute);
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            var query = from XSupplier in this.xmlDB.Root.Element(this.suppliersNamespace).Elements(this.supplierItem)
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
            this.xmlDB.Root.Element(this.suppliersNamespace).Add(new XElement(this.supplierItem, typeof(Supplier).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(supplier, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(supplier, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(this.suppliersNamespace);

            return true;
        }

        public bool Edit(Supplier supplier)
        {
            var XSupplier = this.xmlDB.Root.Element(this.suppliersNamespace).Elements(this.supplierItem).Single(elem => elem.Attribute("ID").Value == supplier.ID.ToString());

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
            this.xmlDB.Root.Element(this.suppliersNamespace).Elements().Single(elem => elem.Attribute("ID").Value == supplier.ID.ToString()).Remove();

            //this.DecreaseSuppliersCount();

            return true;
        }

        public bool Delete(IEnumerable<Supplier> suppliers)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region agents provider

        public uint GetLastAgentID()
        {
            return (uint)this.xmlDB.Root.Element(this.agentItem).Attribute(this.lastIDAttribute);
        }

        public IEnumerable<Agent> GetAllAgents()
        {
            var query = from XAgent in this.xmlDB.Root.Element(this.agentsNamespace).Elements(this.agentItem)
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
            var agent = from XAgent in this.xmlDB.Root.Element(this.agentsNamespace).Elements(this.agentItem)
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
            this.xmlDB.Root.Element(this.agentsNamespace).Add(new XElement(this.agentItem, typeof(Agent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(agent, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(agent, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue(this.agentsNamespace);

            return true;
        }

        public bool Edit(Agent agent)
        {
            var XAgent = this.xmlDB.Root.Element(this.agentsNamespace).Elements(this.agentItem)
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
            this.xmlDB.Root.Element(this.agentsNamespace)
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == agent.ID.ToString())
                .Remove();

            return true;
        }

        public bool Delete(IEnumerable<Agent> agents)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region support methods

        void IncreaseLastIDValue(string ns)
        {
            this.xmlDB.Root.Element(ns).Attribute(this.lastIDAttribute)
                .SetValue(uint.Parse(this.xmlDB.Root.Element(ns).Attribute(this.lastIDAttribute).Value) + 1);
        }

        #endregion

        #endregion
    }
}