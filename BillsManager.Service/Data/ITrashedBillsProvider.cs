using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public interface ITrashedBillsProvider
    {
        IEnumerable<Bill> GetTrash();

        bool MoveToTrash(Bill bill);

        bool Restore(uint id);

        bool EmptyTrash();
    }
}
