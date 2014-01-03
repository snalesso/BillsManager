using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface IBackupsProvider
    {
        string DBName { get; }

        string Location { get; }

        IEnumerable<Backup> GetAll();

        bool CreateNew();

        bool Rollback(Backup backup);

        bool Delete(Backup backup);
    }
}