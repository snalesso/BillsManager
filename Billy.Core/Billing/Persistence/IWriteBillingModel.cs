using Billy.Billing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    // TODO: decide if return Task<Result<T>> or throw exceptions
    public interface IWriteBillsRepository
    {
        // TODO: replace KeyValuePairs with RO-Dictionary or Entity
        Task<Bill> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);
        Task RemoveAsync(long id);
    }
    public interface IWriteSuppliersRepository
    {
        Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        // TODO: return bool cause surronding exceptions might be caused by any other surrounding code
        Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);
        Task RemoveAsync(long id);
    }
    public interface IWriteBillingModel
    {
        IWriteBillsRepository Bills { get; }
        IWriteSuppliersRepository Suppliers { get; }
    }
}