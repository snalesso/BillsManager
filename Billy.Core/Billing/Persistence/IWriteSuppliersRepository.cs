using Billy.Billing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IWriteSuppliersRepository
    {
        // TODO: handle db errors for missing/invalid values etc.
        Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        Task RemoveAsync(IEnumerable<long> ids);
        Task RemoveAsync(long id);
        // TODO: return bool cause surronding exceptions might be caused by any other surrounding code
        Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);
    }
}