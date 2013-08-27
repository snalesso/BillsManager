using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public class XMLBackupsProvider : IBackupsProvider /* TODO: controllare il controllo dell'esistenza delle directories,
                                                        * per decidere se i comandi tipo per creare un nuovo backup devono essere disabilitati da principio
                                                        * o se controllare ad ogni richiesta di creare un nuovo backup se è fattibile */
    {
        #region fields

        private readonly string dbFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        private readonly string backupsFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Database\Backups\";
        private readonly string billsDBFileName = @"Bills";
        private readonly string suppliersDBFileName = @"Suppliers";
        private readonly string dbExt = ".bmdb";
        private readonly string buExt = ".bmbu";

        #endregion

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

                         return new Backup(
                             b,
                             (DateTime)XBackup.Root.Attribute("CreationTime"),
                             (uint)XBackup.Root.Element("Bills").Attribute("BillsCount"),
                             (uint)XBackup.Root.Element("Suppliers").Attribute("SuppliersCount"),
                             ushort.Parse(XBackup.Root.Attribute("TimesUsedForRollback").Value));
                     });
            }
            else
                return new Backup[] { };
        }

        public bool CreateNew()
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
            xmlBackup.Root.Add(new XAttribute("CreationTime", creationTime)); // TODO: check if creationtime is really needed and where
            xmlBackup.Root.Add(new XAttribute("TimesUsedForRollback", 0));

            this.EnsureBackupsFolderExists();
            string savePath = this.backupsFolderPath;
            savePath += creationTime.Date.Day + "-" + creationTime.Date.Month + "-" + creationTime.Date.Year;
            savePath += "_";
            savePath += creationTime.TimeOfDay.Hours + "." + creationTime.TimeOfDay.Minutes + "." + creationTime.TimeOfDay.Seconds;
            savePath += this.buExt;
            xmlBackup.Save(savePath);

            return true;
        }

        public bool Rollback(Backup backup)
        {
            if (System.IO.File.Exists(backup.Path)) // TODO: move current database to temp folder in order to prevent data losing in case of power interrupt
            {
                XDocument xmlBackup = XDocument.Load(backup.Path);
                XDocument xmlSuppliers = new XDocument();
                XDocument xmlBills = new XDocument();

                xmlSuppliers.Declaration = new XDeclaration("1.0", "utf-8", null);
                xmlSuppliers.Add(new XElement("SupliersDatabase", xmlBackup.Root.Element("Suppliers")));
                xmlSuppliers.Save(this.dbFolderPath + this.suppliersDBFileName + this.dbExt);

                xmlBills.Declaration = new XDeclaration("1.0", "utf-8", null);
                xmlBills.Add(new XElement("BillsDatabase", xmlBackup.Root.Element("Bills")));
                xmlBills.Save(this.dbFolderPath + this.billsDBFileName + this.dbExt);
                               
                return true;
            }

            return false;            
        }

        public bool Delete(Backup backup)
        {
            if (System.IO.File.Exists(backup.Path))
                System.IO.File.Delete(backup.Path);

            return true;
        }

        void EnsureBackupsFolderExists()
        {
            if (!System.IO.Directory.Exists(this.backupsFolderPath))
            {
                System.IO.Directory.CreateDirectory(this.backupsFolderPath);
            }
        }

    }
}
