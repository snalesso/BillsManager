namespace Billy.Billing.Services
{
    // TODO: consider using IQbservable
    public interface IBillingService //: ISuppliersRepository
    {
        ISuppliersService Suppliers { get; }
        IBillsService Bills { get; }
    }
}