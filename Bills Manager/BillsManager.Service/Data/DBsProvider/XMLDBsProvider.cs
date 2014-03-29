using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BillsManager.Services.Providers
{
    public class XMLDBsProvider : IDBsProvider
    {
        #region fields

        //private readonly string dbExtension = @"bmdb";
        // TODO: make constants
        private const short START_INDEX = 0;

        private readonly string dbDottedExtension = @".bmdb";

        private readonly string databaseNamespace = @"Database";
        private readonly string billsNamespace = @"Bills";
        private readonly string suppliersNamespace = @"Suppliers";
        private readonly string agentsNamespace = @"Agents";

        private readonly string creationDateAttribute = @"CreationDate";
        private readonly string lastIDAttribute = @"LastID";

        #endregion

        #region ctor

        public XMLDBsProvider(string dbsLocation)
        {
            if (!Directory.Exists(dbsLocation))
                Directory.CreateDirectory(dbsLocation);

            this.Location = dbsLocation;
        }

        #endregion

        #region IDBsProvider Members

        public string Location { get; private set; }

        public IEnumerable<string> GetAll()
        {
            if (!System.IO.Directory.Exists(this.Location))
                throw new DirectoryNotFoundException("Couldn't find the databases folder " + this.Location); // TODO: language

            var subDirs = System.IO.Directory.GetDirectories(this.Location);

            var ipotheticalDBs = subDirs.Select(dir => Path.Combine(dir, dir.Split(Path.DirectorySeparatorChar).LastOrDefault() + this.dbDottedExtension));

            return ipotheticalDBs.Where(ipdb => File.Exists(ipdb));
        }

        public bool CreateDB(string name)
        {
            var newXDoc = new XDocument();

            try
            {
                newXDoc.Declaration = new XDeclaration("1.0", "utf-8", null);
                newXDoc.Add(
                    new XElement(
                        this.databaseNamespace,
                        new XAttribute(this.creationDateAttribute, DateTime.Today),
                        new XElement(this.billsNamespace, new XAttribute(this.lastIDAttribute, START_INDEX)),
                        new XElement(this.suppliersNamespace, new XAttribute(this.lastIDAttribute, START_INDEX)),
                        new XElement(this.agentsNamespace, new XAttribute(this.lastIDAttribute, START_INDEX))));

                var dbSavePath = System.IO.Path.Combine(this.Location, name, name + this.dbDottedExtension);

                Directory.CreateDirectory(Path.GetDirectoryName(dbSavePath));

                newXDoc.Save(dbSavePath);
            }
            catch { return false; }

            return true;
        }

        public bool DeleteDB(string name)
        {
            string dbToDeleteDirPath = System.IO.Path.Combine(this.Location, name);

            if (System.IO.Directory.Exists(dbToDeleteDirPath))

                try { this.DeleteDirectory(dbToDeleteDirPath); }
                catch { return false; }

            else
                return false;

            return true;
        }

        public bool RenameDB(string oldName, string newName)
        {
            var oldDBFilePath = Path.Combine(this.Location, oldName, oldName + this.dbDottedExtension);

            if (File.Exists(oldDBFilePath))
            {
                var newDBFilePath = Path.Combine(this.Location, oldName, newName + this.dbDottedExtension);
                File.Move(oldDBFilePath, newDBFilePath);

                var oldDBDirPath = Path.GetDirectoryName(newDBFilePath);
                var newDBDirPath = Path.Combine(Path.GetDirectoryName(oldDBDirPath), newName);
                Directory.Move(oldDBDirPath, newDBDirPath);

                return true;
            }
            else
                throw new FileNotFoundException("couldn't find the file " + oldDBFilePath);
        }

        #endregion

        #region support

        private void DeleteDirectory(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                this.DeleteDirectory(dir);
            }

            Directory.Delete(dirPath, false);
        }

        #endregion
    }
}