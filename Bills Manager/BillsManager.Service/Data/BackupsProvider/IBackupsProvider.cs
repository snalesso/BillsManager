using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface IBackupsProvider
    {
        string DBName { get; } // TODO: obsolete?

        string Location { get; } // TODO: obsolete?

        IEnumerable<Backup> GetAll();

        bool CreateNew();

        bool Rollback(Backup backup);

        bool Delete(Backup backup);
    }
}