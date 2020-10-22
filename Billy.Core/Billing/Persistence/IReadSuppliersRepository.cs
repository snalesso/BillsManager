using Billy.Billing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IReadSuppliersRepository
    {
        Task<Supplier> GetByIdAsync(long id);
        //[Obsolete("Added for mistake, not needed by now")]
        Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params long[] ids);
        Task<IReadOnlyCollection<Supplier>> GetMultipleAsync();
    }
}