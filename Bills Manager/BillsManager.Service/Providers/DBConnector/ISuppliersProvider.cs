using System.Collections.Generic;
using BillsManager.Models;

namespace BillsManager.Services.Providers
{
    public interface ISuppliersProvider
    {
        string Path { get; }

        string DBName { get; }

        uint GetLastSupplierID();

        IEnumerable<Supplier> GetAllSuppliers();

        bool Add(Supplier supplier);

        bool Edit(Supplier supplier);
        bool Edit(IEnumerable<Supplier> suppliers);

        bool Delete(Supplier supplier);
        bool Delete(IEnumerable<Supplier> suppliers);
    }
}
