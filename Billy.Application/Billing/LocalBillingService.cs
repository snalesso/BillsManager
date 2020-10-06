using System;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Application
{
    public class LocalBillingService : IBillingService
    {
        public LocalBillingService(
            ISuppliersService localSuppliersService
            )
        {
            this.Suppliers = localSuppliersService ?? throw new ArgumentNullException(nameof(localSuppliersService));
        }

        public ISuppliersService Suppliers { get; }
    }
}