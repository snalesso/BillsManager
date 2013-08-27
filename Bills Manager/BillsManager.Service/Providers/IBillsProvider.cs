using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.Service.Providers
{
    public interface IBillsProvider
    {
        uint GetLastID();

        IEnumerable<Bill> GetAll();

        bool Add(Bill bill);

        bool Edit(Bill bill);
        bool Edit(IEnumerable<Bill> bills);

        bool Delete(Bill bill);
        bool Delete(IEnumerable<Bill> bills);
    }
}
