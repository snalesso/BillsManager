using Billy.Domain.Persistence;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IBillingUnitOfWork : IUnitOfWork
    {
        ISuppliersRepository Suppliers { get; }
        IBillsRepository Bills { get; }
    }
    public interface IBillingUnitOfWorkFactory
    {
        Task<IBillingUnitOfWork> CreateAsync();
    }
}