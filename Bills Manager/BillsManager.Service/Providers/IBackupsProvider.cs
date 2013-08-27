using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public interface IBackupsProvider
    {
        IEnumerable<Backup> GetAll();

        bool CreateNew();

        bool Rollback(Backup backup);

        bool Delete(Backup backup);
    }
}
