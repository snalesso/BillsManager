using Billy.Billing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    // TODO: decide if return Task<Result<T>> or throw exceptions
    public interface IWriteBillsRepository
    {
        // TODO: handle db errors for missing/invalid values etc.
        Task<Bill> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);
        Task RemoveAsync(long id);
    }
}