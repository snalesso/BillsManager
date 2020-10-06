using System.Collections;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Application
{
    // TODO: consider using IQbservable
    public interface IBillingService //: ISuppliersRepository
    {
        ISuppliersService Suppliers { get; }
    }
}