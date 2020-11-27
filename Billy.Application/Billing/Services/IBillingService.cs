using Billy.Billing.Persistence;

namespace Billy.Billing.Services
{
    // TODO: consider using IQbservable
    public interface IBillingService : IReadBillingService, IWriteBillingService
    {
        new ISuppliersService Suppliers { get; }
        new IBillsService Bills { get; }
    }

    public interface IReadBillingService
    {
        IReadSuppliersService Suppliers { get; }
        IReadBillsService Bills { get; }
    }

    public interface IWriteBillingService
    {
        IWriteSuppliersService Suppliers { get; }
        IWriteBillsService Bills { get; }
    }
}