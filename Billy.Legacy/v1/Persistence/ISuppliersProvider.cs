using BillsManager.v1.Models;
using System.Collections.Generic;

namespace BillsManager.v1.Services.Data
{
    public interface ISuppliersProvider
    {
        uint GetLastSupplierID();

        IEnumerable<Supplier> GetAllSuppliers();

        bool Add(Supplier supplier);
        
        bool Edit(Supplier supplier);
        //bool Edit(IEnumerable<Supplier> suppliers); // TODO: obsolete?

        bool Delete(Supplier supplier);
        //bool Delete(IEnumerable<Supplier> suppliers); // TODO: obsolete?
    }
}