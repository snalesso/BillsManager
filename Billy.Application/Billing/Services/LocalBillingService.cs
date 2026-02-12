using System;

namespace Billy.Billing.Services
{
    public class LocalBillingService : IBillingService
    {
        public LocalBillingService(
            ISuppliersService suppliersService
            , IBillsService billsService
            )
        {
            this.Suppliers = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
            this.Bills = billsService ?? throw new ArgumentNullException(nameof(billsService));
        }

        public ISuppliersService Suppliers { get; }
        IReadSuppliersService IReadBillingService.Suppliers => this.Suppliers;
        IWriteSuppliersService IWriteBillingService.Suppliers => this.Suppliers;

        public IBillsService Bills { get; }
        IReadBillsService IReadBillingService.Bills => this.Bills;
        IWriteBillsService IWriteBillingService.Bills => this.Bills;
    }
}