using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface IDBsProvider
    {
        string Location { get; }

        /// <summary>
        /// Provides the paths of the available databases in the operating location.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAll();

        //string GetDBName(string dbPath);

        bool CreateDB(string name); // TODO: all provider use base types or model?

        bool DeleteDB(string name);

        bool RenameDB(string oldName, string newName);
    }
}