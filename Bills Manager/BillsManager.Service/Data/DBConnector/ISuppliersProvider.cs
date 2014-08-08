using System.Collections.Generic;
using BillsManager.Models;

namespace BillsManager.Services.Providers
{
    public interface ISuppliersProvider
    {
        //string DBPath { get; } // TODO: obsolete?

        //string DBName { get; } // TODO: obsolete?

        uint GetLastSupplierID();

        IEnumerable<Supplier> GetAllSuppliers();

        bool Add(Supplier supplier);
        
        bool Edit(Supplier supplier);
        //bool Edit(IEnumerable<Supplier> suppliers); // TODO: obsolete?

        bool Delete(Supplier supplier);
        //bool Delete(IEnumerable<Supplier> suppliers); // TODO: obsolete?
    }
}