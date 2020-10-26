namespace Billy.Billing.Services
{
    // TODO: consider using IQbservable
    public interface IBillingService
    {
        ISuppliersService Suppliers { get; }
        IBillsService Bills { get; }
    }
}