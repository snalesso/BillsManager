using BillsManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BillsManager.Services.Data
{
    public class XMLBackupsProvider : IBackupsProvider
    {
        #region fields

        private const string DB_NAME = @"db";
        private const string DB_DOT_EXTENSION = @".bmdb";
        private const string BACKUP_NAME_FORMAT = @"{0:00}-{1:00}-{2:00}_{3:00}.{4:00}.{5:00}";
        private const string BACKUP_DOT_EXTENSION = @".bmbu";
        private const string DB_TEMP_FOLDER_NAME = @"Temp";

        private const string NS_DATABASE = @"Database";
        private const string NS_BILLS = @"Bills";
        private const string NS_SUPPLIERS = @"Suppliers";
        private const string NS_TAGS = @"Tags";

        private readonly string dbFilePath;

        #endregion

        #region ctor

        public XMLBackupsProvider(
            string backupsDirectoryPath)
        {
            this.location = backupsDirectoryPath;
            this.dbFilePath = Path.Combine(Path.GetDirectoryName(backupsDirectoryPath.Substring(0, backupsDirectoryPath.Length - 1)), @"DB", DB_NAME + DB_DOT_EXTENSION);
        }

        #endregion

        #region properties

        private readonly string location;
        public string Location
        {
            get { return this.location; }
        }

        #endregion

        #region methods

        private bool IsBackupFile(string path)
        {
            var ext = Path.GetExtension(path);
            return ext.ToLowerInvariant() == BACKUP_DOT_EXTENSION;
        }

        public IEnumerable<Backup> GetAll()
        {
            var backups = new List<Backup>();

            if (Directory.Exists(this.Location))
            {
                backups.AddRange(
                    System.IO.Directory.GetFiles(this.Location)
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
            if (File.Exists(this.dbFilePath))
            {
                DateTime creationTime = DateTime.Now;

                XDocument xmlDB = XDocument.Load(this.dbFilePath);
                XDocument xmlBackup = new XDocument();

                xmlBackup.Declaration = new XDeclaration("1.0", "utf-8", null);
                xmlBackup.Add(new XElement("Backup", xmlDB.Root.Elements()));
                xmlBackup.Root.Add(new XAttribute("CreationTime", creationTime));

                this.EnsureBackupsDirectoryExists();

                string newBackupName = string.Format(
                    BACKUP_NAME_FORMAT,
                    creationTime.Date.Day,
                    creationTime.Date.Month,
                    creationTime.Date.Year,
                    creationTime.TimeOfDay.Hours,
                    creationTime.TimeOfDay.Minutes,
                    creationTime.TimeOfDay.Seconds);

                string newBackupSavePath = Path.Combine(this.Location, newBackupName + BACKUP_DOT_EXTENSION);

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
            if (File.Exists(backup.Path))
            {
                /* create a temp copy of the toOverwriteDB in order to prevent data loss
                 * so if it is half-overwritten it isn't lost */

                /* create the temp folder */
                string dbDirectoryPath = Path.GetDirectoryName(Location);
                string tempDirectoryPath = Path.Combine(dbDirectoryPath, DB_TEMP_FOLDER_NAME);
                Directory.CreateDirectory(tempDirectoryPath);

                /* create the temp file */
                string tempDBBackupFilePath = Path.Combine(tempDirectoryPath, Path.GetFileName(this.dbFilePath));
                File.Copy(this.dbFilePath, tempDBBackupFilePath, true);

                try
                {
                    XDocument xmlBackup = XDocument.Load(backup.Path);
                    XDocument xmlDB = new XDocument();

                    xmlDB.Declaration = new XDeclaration("1.0", "utf-8", null);
                    xmlDB.Add(new XElement(
                        "Database",
                        xmlBackup.Root.Element(NS_DATABASE),
                        xmlBackup.Root.Element(NS_SUPPLIERS),
                        xmlBackup.Root.Element(NS_BILLS),
                        xmlBackup.Root.Element(NS_TAGS)));
                    xmlDB.Save(this.dbFilePath);

                    this.AddRollbackDate(xmlBackup, DateTime.Now);
                    xmlBackup.Save(backup.Path);
                }
                catch
                {
                    return false;
                }

                /* delete temp data */
                if (Directory.Exists(tempDirectoryPath))
                    Directory.Delete(tempDirectoryPath, true);

                return true;
            }
            else
                throw new FileNotFoundException("couldn't find " + backup.Path);
        }

        public bool Delete(Backup backup)
        {
            if (File.Exists(backup.Path))
            {
                File.Delete(backup.Path); // TODO: change to move to recycle bin
                return true;
            }
            else
                throw new FileNotFoundException("couldn't find " + backup.Path);
        }

        #region support methods

        void EnsureBackupsDirectoryExists()
        {
            if (!System.IO.Directory.Exists(this.Location))
                System.IO.Directory.CreateDirectory(this.Location);
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