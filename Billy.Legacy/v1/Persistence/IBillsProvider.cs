using BillsManager.v1.Models;
using System.Collections.Generic;

namespace BillsManager.v1.Services.Data
{
    public interface IBillsProvider
    {
        uint GetLastBillID();

        IEnumerable<Bill> GetAllBills();

        bool Add(Bill bill);

        bool Edit(Bill bill);
        //bool Edit(IEnumerable<Bill> bills); // TODO: obsolete?

        bool Delete(Bill bill);
        //bool Delete(IEnumerable<Bill> bills); // TODO: obsolete?
    }
}