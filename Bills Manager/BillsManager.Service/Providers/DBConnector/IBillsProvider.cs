using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface IBillsProvider
    {
        string Path { get; }

        string DBName { get; }

        uint GetLastBillID();

        IEnumerable<Bill> GetAllBills();

        bool Add(Bill bill);

        bool Edit(Bill bill);
        bool Edit(IEnumerable<Bill> bills);

        bool Delete(Bill bill);
        bool Delete(IEnumerable<Bill> bills);
    }
}
