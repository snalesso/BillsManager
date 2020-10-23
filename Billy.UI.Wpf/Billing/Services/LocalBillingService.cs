using System;

namespace Billy.Billing.Services
{
    public class LocalBillingService : IBillingService
    {
        public LocalBillingService(
            ISuppliersService localSuppliersService
            , IBillsService localBillsService
            )
        {
            this.Suppliers = localSuppliersService ?? throw new ArgumentNullException(nameof(localSuppliersService));
            this.Bills = localBillsService ?? throw new ArgumentNullException(nameof(localBillsService));
        }

        public ISuppliersService Suppliers { get; }
        public IBillsService Bills { get; }
    }
}