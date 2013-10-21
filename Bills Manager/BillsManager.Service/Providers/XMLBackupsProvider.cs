using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public class XMLBackupsProvider : IBackupsProvider
    {
        #region fields

        private readonly string dbFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        private readonly string backupsFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Backups\";
        private readonly string dbTempFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Temp\";

        private readonly string billsDBFileName = @"Bills";
        private readonly string suppliersDBFileName = @"Suppliers";
        private readonly string dbExt = ".bmdb";
        private readonly string buExt = ".bmbu";

        #endregion

        #region methods

        public IEnumerable<Backup> GetAll()
        {
            this.EnsureBackupsFolderExists();

            if (System.IO.Directory.Exists(this.backupsFolderPath))
            {
                return System.IO.Directory.GetFiles(this.backupsFolderPath)
                     .Where(b => System.IO.Path.GetExtension(b) == this.buExt)
                     .Select(b =>
                     {
                         XDocument XBackup;

                         XBackup = XDocument.Load(b);

                         List<DateTime> rollbackDates = new List<DateTime>();

                         if (XBackup.Root.Element("RollbackDates") != null)
                             foreach (var date in XBackup.Root.Element("RollbackDates").Elements())
                                 rollbackDates.Add(DateTime.Parse(date.Value));

                         return new Backup(
                             b,
                             (DateTime)XBackup.Root.Attribute("CreationTime"),
                             (uint)XBackup.Root.Element("Bills").Elements().Count(),
                             (uint)XBackup.Root.Element("Suppliers").Elements().Count(),
                             rollbackDates);
                     });
            }
            else
                return new Backup[] { };
        }

        public bool CreateNew()
        {
            if (System.IO.File.Exists(this.dbFolderPath + this.billsDBFileName + this.dbExt) |
                System.IO.File.Exists(this.dbFolderPath + this.suppliersDBFileName + this.dbExt))
            {

                DateTime creationTime = DateTime.Now;

                XDocument xmlSuppliers = XDocument.Load(this.dbFolderPath + this.suppliersDBFileName + this.dbExt);
                XDocument xmlBills = XDocument.Load(this.dbFolderPath + this.billsDBFileName + this.dbExt);

                XDocument xmlBackup = new XDocument();
                xmlBackup.Declaration = new XDeclaration("1.0", "utf-8", null);

                xmlBackup.Add(new XElement("Backup",
                    xmlSuppliers.Root.Element("Suppliers"),
                    xmlBills.Root.Element("Bills")));

                // xmlBackup.Root.Element("Suppliers").Add(new XAttribute("LastID", xmlSuppliers.Root.Attribute("LastID").Value));
                // xmlBackup.Root.Element("Bills").Add(new XAttribute("LastID", xmlBills.Root.Attribute("LastID").Value));

                // xmlBackup.Root.Add(new XAttribute("SuppliersCount", xmlBackup.Root.Element("Suppliers").Descendants("Supplier").Count()));
                // xmlBackup.Root.Add(new XAttribute("BillsCount", xmlBackup.Root.Element("Bills").Descendants("Bill").Count()));

                xmlBackup.Root.Add(new XAttribute("CreationTime", creationTime));
                //xmlBackup.Root.Add(new XAttribute("TimesUsedForRollback", 0));

                this.EnsureBackupsFolderExists();
                string savePath = this.backupsFolderPath;
                savePath += creationTime.Date.Day + "-" + creationTime.Date.Month + "-" + creationTime.Date.Year;
                savePath += "_";
                savePath += creationTime.TimeOfDay.Hours + "." + creationTime.TimeOfDay.Minutes + "." + creationTime.TimeOfDay.Seconds;
                savePath += this.buExt;
                xmlBackup.Save(savePath);

                return true;
            }

            return false;
        }

        public bool Rollback(Backup backup)
        {
            if (System.IO.File.Exists(backup.Path))
            {
                System.IO.Directory.CreateDirectory(this.dbTempFolder);

                foreach (var file in System.IO.Directory.GetFiles(this.dbFolderPath))
                    System.IO.File.Copy(file, this.dbTempFolder + System.IO.Path.GetFileName(file),true);

                XDocument xmlBackup = XDocument.Load(backup.Path);
                XDocument xmlSuppliers = new XDocument();
                XDocument xmlBills = new XDocument();

                xmlSuppliers.Declaration = new XDeclaration("1.0", "utf-8", null);
                xmlSuppliers.Add(new XElement("SupliersDatabase", xmlBackup.Root.Element("Suppliers")));
                xmlSuppliers.Save(this.dbFolderPath + this.suppliersDBFileName + this.dbExt);

                xmlBills.Declaration = new XDeclaration("1.0", "utf-8", null);
                xmlBills.Add(new XElement("BillsDatabase", xmlBackup.Root.Element("Bills")));
                xmlBills.Save(this.dbFolderPath + this.billsDBFileName + this.dbExt);

                this.AddRollbackDate(xmlBackup, DateTime.Now);

                xmlBackup.Save(backup.Path);

                if (System.IO.Directory.Exists(this.dbTempFolder))
                    System.IO.Directory.Delete(this.dbTempFolder, true);

                return true;
            }

            return false;
        }

        public bool Delete(Backup backup)
        {
            if (System.IO.File.Exists(backup.Path))
                System.IO.File.Delete(backup.Path); // TODO: change to move to recycle bin

            return true;
        }

        #region support methods

        void EnsureBackupsFolderExists()
        {
            if (!System.IO.Directory.Exists(this.backupsFolderPath))
            {
                System.IO.Directory.CreateDirectory(this.backupsFolderPath);
            }
        }

        void AddRollbackDate(XDocument xmlBackup, DateTime rollbackDate)
        {
            if (xmlBackup == null)
                throw new NotImplementedException("xmlBackup cannot be null");

            var rollbackDates = xmlBackup.Root.Element("RollbackDates");

            if (rollbackDates == null)
            {
                rollbackDates = new XElement("RollbackDates");
                xmlBackup.Root.Add(rollbackDates);
            }

            rollbackDates.Add(new XElement("RollbackDate", rollbackDate));
        }

        #endregion

        #endregion
    }
}
