using BillsManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BillsManager.Services.Providers
{
    public class XMLBackupsProvider : IBackupsProvider
    {
        #region fields

        //private readonly string dbFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Database\";
        //private readonly string backupsFolderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Backups\";

        //private const string BILLS_DB_FILENAME = @"Bills";
        //private const string SUPPLIERS_DB_FILENAME = @"Suppliers";
        private const string DB_EXTENSION = @"bmdb";
        private const string DB_DOT_EXTENSION = @".bmdb";
        private const string BACKUP_EXTENSION = @"bmbu";
        private const string BACKUP_DOT_EXTENSION = @".bmbu";
        private const string BACKUPS_DIRECTORY_NAME = @"Backups";
        private const string DB_TEMP_FOLDER_NAME = @"Temp";

        private readonly string dbDirectoryPath;
        private readonly string dbFilePath;
        private readonly string backupsDirectoryPath;

        #endregion

        #region ctor

        public XMLBackupsProvider(
            string dbFilePath)
        {
            this.dbFilePath = dbFilePath; // TODO: clenaup
            this.dbDirectoryPath = System.IO.Path.GetDirectoryName(dbFilePath);
            this.backupsDirectoryPath = System.IO.Path.Combine(this.dbDirectoryPath, BACKUPS_DIRECTORY_NAME);
            this.dbName = System.IO.Path.GetFileNameWithoutExtension(dbFilePath);
        }

        #endregion

        #region properties

        public string Location
        {
            get { return this.backupsDirectoryPath; }
        }

        private readonly string dbName;
        public string DBName
        {
            get { return this.dbName; }
        }

        #endregion

        #region methods

        private bool IsBackupFile(string path)
        {
            var ext = System.IO.Path.GetExtension(path);
            return ext.ToLowerInvariant() == BACKUP_DOT_EXTENSION;
        }

        public IEnumerable<Backup> GetAll()
        {
            var backups = new List<Backup>();

            if (System.IO.Directory.Exists(this.backupsDirectoryPath))
            {
                backups.AddRange(
                    System.IO.Directory.GetFiles(this.backupsDirectoryPath)
                    .Where(b => this.IsBackupFile(b))
                    .Select(b =>
                    {
                        XDocument xmlBackup = XDocument.Load(b);

                        List<DateTime> rollbackDates = new List<DateTime>();

                        if (xmlBackup.Root.Element("RollbackDates") != null)
                            foreach (var date in xmlBackup.Root.Element("RollbackDates").Elements())
                                rollbackDates.Add(DateTime.Parse(date.Value));

                        return new Backup(
                            b,
                            (DateTime)xmlBackup.Root.Attribute("CreationTime"),
                            (uint)xmlBackup.Root.Element("Bills").Elements().Count(),
                            (uint)xmlBackup.Root.Element("Suppliers").Elements().Count(),
                            rollbackDates);
                    }));
            }

            return backups;
        }

        public bool CreateNew()
        {
            if (System.IO.File.Exists(this.dbFilePath))
            {
                DateTime creationTime = DateTime.Now;

                XDocument xmlDB = XDocument.Load(this.dbFilePath);
                XDocument xmlBackup = new XDocument();

                xmlBackup.Declaration = new XDeclaration("1.0", "utf-8", null);
                xmlBackup.Add(new XElement("Backup", xmlDB.Root.Elements()));
                xmlBackup.Root.Add(new XAttribute("CreationTime", creationTime));

                this.EnsureBackupsDirectoryExists();

                string newBackupName = string.Empty;
                newBackupName += creationTime.Date.Day + "-" + creationTime.Date.Month + "-" + creationTime.Date.Year;
                newBackupName += "_";
                newBackupName += creationTime.TimeOfDay.Hours + "." + creationTime.TimeOfDay.Minutes + "." + creationTime.TimeOfDay.Seconds;
                //newBackupName += BACKUP_DOT_EXTENSION;

                string newBackupSavePath = System.IO.Path.Combine(this.backupsDirectoryPath, newBackupName + BACKUP_DOT_EXTENSION);

                xmlBackup.Save(newBackupSavePath);

                return true;
            }

            return false;
        }

        //<Database CreationDate="2013-11-26T00:00:00+01:00">
        //  <Bills LastID="7">
        //    <Bill />
        //  </Bills>
        //  <Suppliers LastID="1">
        //    <Supplier />
        //  </Suppliers>
        //  <Agents LastID="0" />
        //</Database>

        public bool Rollback(Backup backup)
        {
            if (System.IO.File.Exists(backup.Path))
            {
                /* create a temp copy of the toOverwriteDB in order to prevent data loss
                 * so if it is half-overwritten it isn't lost */

                /* create the temp folder */
                string tempDirectoryPath = System.IO.Path.Combine(this.dbDirectoryPath, DB_TEMP_FOLDER_NAME);
                System.IO.Directory.CreateDirectory(tempDirectoryPath);

                /* create the temp file */
                string tempDBBackupFilePath = System.IO.Path.Combine(tempDirectoryPath, System.IO.Path.GetFileName(this.dbFilePath));
                System.IO.File.Copy(this.dbFilePath, tempDBBackupFilePath, true);

                try
                {
                    XDocument xmlBackup = XDocument.Load(backup.Path);
                    XDocument xmlDB = new XDocument();

                    xmlDB.Declaration = new XDeclaration("1.0", "utf-8", null);
                    xmlDB.Add(new XElement("Database", xmlBackup.Root.Elements()));
                    xmlDB.Save(this.dbFilePath);

                    this.AddRollbackDate(xmlBackup, DateTime.Now);
                    xmlBackup.Save(backup.Path);
                }
                catch
                {
                    return false;
                }

                /* delete temp data */
                if (System.IO.Directory.Exists(tempDirectoryPath))
                    System.IO.Directory.Delete(tempDirectoryPath, true);

                return true;
            }
            else
                throw new FileNotFoundException("couldn't find " + backup.Path);
        }

        public bool Delete(Backup backup)
        {
            if (System.IO.File.Exists(backup.Path))
            {
                System.IO.File.Delete(backup.Path); // TODO: change to move to recycle bin
                return true;
            }
            else
                throw new FileNotFoundException("couldn't find " + backup.Path);
        }

        #region support methods

        void EnsureBackupsDirectoryExists()
        {
            if (!System.IO.Directory.Exists(this.backupsDirectoryPath))
                System.IO.Directory.CreateDirectory(this.backupsDirectoryPath);
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