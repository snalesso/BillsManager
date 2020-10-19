using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface ISuppliersRepository
    {
        // TODO: handle db errors for missing/invalid values etc.
        Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);

        Task<IReadOnlyCollection<Supplier>> GetMultipleAsync();
        Task<Supplier> GetByIdAsync(int id);
        [Obsolete("Added for mistake, not needed by now")]
        Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params int[] ids);

        // TODO: return bool cause surronding exceptions might be caused by any other surrounding code
        Task UpdateAsync(int id, IEnumerable<KeyValuePair<string, object>> changes);

        Task RemoveAsync(int id);
        Task RemoveAsync(IEnumerable<int> ids);

        //IObservable<IReadOnlyCollection<Supplier>> SuppliersAddeded { get; }
        //IObservable<IReadOnlyCollection<Supplier>> SuppliersRemoved { get; }
        //IObservable<IReadOnlyCollection<Supplier>> SuppliersUpdated { get; }

        //event EventHandler<ItemEventArgs<Supplier>> SuppliersAddeded;
        //event EventHandler<ItemEventArgs<Supplier>> SuppliersRemoved;
        //event EventHandler<ItemEventArgs<Supplier>> SuppliersUpdated;

        //event EventHandler<IReadOnlyCollection<Supplier>> SuppliersAddeded;
        //event EventHandler<IReadOnlyCollection<Supplier>> SuppliersRemoved;
        //event EventHandler<IReadOnlyCollection<Supplier>> SuppliersUpdated;
    }
}