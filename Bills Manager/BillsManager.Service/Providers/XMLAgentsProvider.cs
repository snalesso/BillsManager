using BillsManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace BillsManager.Service.Providers
{
    public class XMLAgentsProvider : IAgentsProvider
    {
        #region fields

        private readonly string agentsDBFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        private readonly string agentsDBFileName = @"Agents";
        private readonly string dbExt = ".bmdb";

        private XDocument xmlAgentsDB;

        #endregion

        #region methods

        public uint GetLastID()
        {
            this.EnsureXDocumentIsInitialized();

            return (uint)this.xmlAgentsDB.Root.Element("Agent").Attribute("LastID");
        }

        public IEnumerable<Agent> GetAll()
        {
            this.EnsureXDocumentIsInitialized();

            var query = from XAgent in this.xmlAgentsDB.Root.Element("Agents").Elements("Agent")
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

        public Agent GetByID(uint agentID)
        {
            this.EnsureXDocumentIsInitialized();

            var agent = from XAgent in this.xmlAgentsDB.Root.Element("Agents").Elements("Agent")
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
            this.EnsureXDocumentIsInitialized();

            this.xmlAgentsDB.Root.Element("Agents").Add(new XElement("Agent", typeof(Agent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(pi =>
                {
                    if (pi.GetValue(agent, null) != null)
                        return new XAttribute(pi.Name, pi.GetValue(agent, null));
                    else
                        return null;
                })));

            this.IncreaseLastIDValue();

            //this.IncreaseAgentsCount();

            this.SaveXDocument();

            return true;
        }

        public bool Edit(Agent agent)
        {
            this.EnsureXDocumentIsInitialized();

            var XAgent = this.xmlAgentsDB.Root.Element("Agents").Elements("Agent")
                .Single(elem => elem.Attribute("ID").Value == agent.ID.ToString());

            typeof(Agent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.Name != "ID")
                .ToList()
                .ForEach(pi =>
                {
                    XAgent.SetAttributeValue(pi.Name, pi.GetValue(agent, null));
                });

            this.SaveXDocument();

            return true;
        }

        public bool Edit(IEnumerable<Agent> agents)
        {
            this.EnsureXDocumentIsInitialized();

            foreach (Agent b in agents)
            {

                var XAgent = this.xmlAgentsDB.Root.Element("Agents").Elements("Agent")
                    .Single(elem => elem.Attribute("ID").Value == b.ID.ToString());

                typeof(Agent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => pi.Name != "ID")
                    .ToList()
                    .ForEach(pi =>
                    {
                        XAgent.SetAttributeValue(pi.Name, pi.GetValue(b, null));
                    });
            }

            this.SaveXDocument();

            return true;
        }

        public bool Delete(Agent agent)
        {
            this.EnsureXDocumentIsInitialized();

            this.xmlAgentsDB.Root.Element("Agents")
                .Elements()
                .Single(elem => elem.Attribute("ID").Value == agent.ID.ToString())
                .Remove();

            //this.DecreaseAgentsCount();

            this.SaveXDocument();

            return true;
        }

        public bool Delete(IEnumerable<Agent> agents)
        {
            this.EnsureXDocumentIsInitialized();

            foreach (Agent b in agents)
            {
                this.xmlAgentsDB.Root.Element("Agents")
                    .Elements()
                    .Single(elem => elem.Attribute("ID").Value == b.ID.ToString())
                    .Remove();
                //this.DecreaseAgentsCount();
            }

            this.SaveXDocument();

            return true;
        }

        #region support methods

        void EnsureXDocumentIsInitialized()
        {
            if (this.xmlAgentsDB == null || this.xmlAgentsDB.BaseUri != this.agentsDBFolder + this.agentsDBFileName + this.dbExt)
            {
                this.EnsureDBFileExists();

                this.xmlAgentsDB = XDocument.Load(this.agentsDBFolder + this.agentsDBFileName + this.dbExt);
            }
        }

        void EnsureDBFileExists()
        {
            if (!System.IO.Directory.Exists(this.agentsDBFolder))
            {
                System.IO.Directory.CreateDirectory(this.agentsDBFolder);
                this.CreateXMLDatabase();
            }
            else
            {
                if (!System.IO.File.Exists(this.agentsDBFolder + this.agentsDBFileName + this.dbExt))
                {
                    this.CreateXMLDatabase();
                }
            }
        }

        void CreateXMLDatabase()
        {
            var newXDoc = new XDocument();

            newXDoc.Declaration = new XDeclaration("1.0", "utf-8", null);
            newXDoc.Add(new XElement("AgentsDatabase", new XElement("Agents", new XAttribute("LastID", 0)/*, new XAttribute("AgentsCount", 0)*/)));
            newXDoc.Root.Add(new XAttribute("CreationDate", DateTime.Today));

            newXDoc.Save(this.agentsDBFolder + this.agentsDBFileName + this.dbExt);
        }

        void IncreaseLastIDValue()
        {
            this.xmlAgentsDB.Root.Element("Agents").Attribute("LastID").SetValue(uint.Parse(this.xmlAgentsDB.Root.Element("Agents").Attribute("LastID").Value) + 1);
        }

        /*void IncreaseAgentsCount()
        {
            this.xmlAgentsDB.Root.Element("Agents").Attribute("AgentsCount").SetValue(uint.Parse(this.xmlAgentsDB.Root.Element("Agents").Attribute("AgentsCount").Value) + 1);
        }

        void DecreaseAgentsCount()
        {
            this.xmlAgentsDB.Root.Element("Agents").Attribute("AgentsCount").SetValue(uint.Parse(this.xmlAgentsDB.Root.Element("Agents").Attribute("AgentsCount").Value) - 1);
        }*/

        void SaveXDocument()
        {
            this.xmlAgentsDB.Save(this.agentsDBFolder + this.agentsDBFileName + this.dbExt);
        }

        #endregion

        #endregion
    }
}
