using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface IBillsProvider
    {
        //string DBPath { get; } // TODO: obsolete?

        //string DBName { get; } // TODO: obsolete?

        uint GetLastBillID();

        IEnumerable<Bill> GetAllBills();

        bool Add(Bill bill); // TODO: change name to New/CreateNew

        bool Edit(Bill bill);
        bool Edit(IEnumerable<Bill> bills); // TODO: unused?

        bool Delete(Bill bill);
        bool Delete(IEnumerable<Bill> bills); // TODO: unused?
    }
}