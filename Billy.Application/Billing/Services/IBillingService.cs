using System.Collections;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Services
{
    // TODO: consider using IQbservable
    public interface IBillingService //: ISuppliersRepository
    {
        ISuppliersService Suppliers { get; }
    }
}