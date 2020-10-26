using Billy.Billing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IReadBillsRepository
    {
        Task<Bill> GetByIdAsync(long id);
        Task<IReadOnlyCollection<Bill>> GetMultipleAsync();
    }
}