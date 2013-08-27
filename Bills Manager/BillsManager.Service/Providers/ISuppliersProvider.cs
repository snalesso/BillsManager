using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public interface ISuppliersProvider
    {
        uint GetLastID();

        IEnumerable<Supplier> GetAll();

        bool Add(Supplier supplier);

        bool Edit(Supplier supplier);

        bool Delete(Supplier supplier);
    }
}
