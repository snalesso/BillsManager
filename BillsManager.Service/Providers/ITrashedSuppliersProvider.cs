using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public interface ITrashedSuppliersProvider
    {
        IEnumerable<Supplier> GetTrash();

        bool MoveToTrash(Supplier supplier);

        bool Restore(uint id);

        bool EmptyTrash();
    }
}
