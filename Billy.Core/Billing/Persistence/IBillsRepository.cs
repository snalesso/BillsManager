using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Persistence
{
    public interface IBillsRepository
    {
        Task<IReadOnlyCollection<Bill>> GetBillsAsync();

        //Task<Bill> CreateAndAddAsync(NewBill newBill);

        //Task UpdateAsync(uint id, IBill changes);

        Task RemoveAsync(uint id);
        Task RemoveAsync(IEnumerable<uint> ids);

        //IObservable<IReadOnlyCollection<Bill>> BillsAddeded { get; }
        //IObservable<IReadOnlyCollection<Bill>> BillsRemoved { get; }
        //IObservable<IReadOnlyCollection<Bill>> BillsUpdated { get; }

        public event EventHandler<IReadOnlyCollection<Bill>> BillsAddeded;
        public event EventHandler<IReadOnlyCollection<Bill>> BillsRemoved;
        public event EventHandler<IReadOnlyCollection<Bill>> BillsUpdated;
    }
}