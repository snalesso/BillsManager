﻿using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.DB
{
    public interface ISuppliersRepository
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