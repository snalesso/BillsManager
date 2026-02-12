using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IBillingUnitOfWorkFactory
    {
        Task<IBillingUnitOfWork> CreateAsync();
    }
}