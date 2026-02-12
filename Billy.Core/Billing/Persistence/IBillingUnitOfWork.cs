using Billy.Domain.Persistence;

namespace Billy.Billing.Persistence
{
    public interface IBillingUnitOfWork : IUnitOfWork
    {
        ISuppliersRepository Suppliers { get; }
        IBillsRepository Bills { get; }

        ISupplierSummaries SupplierSummaries { get; }
        ISupplierHeadersView SupplierHeaders { get; }
        IDetailedBillsView DetailedBills { get; }

        //IReadBillingModel Read { get; }
        //IWriteBillingModel Write { get; }
    }
}