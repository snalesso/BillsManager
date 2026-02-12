using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    public sealed class SupplierHeaderDto
    {
        private readonly Supplier _supplier;

        public SupplierHeaderDto(Supplier supplier)
        {
            this._supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        }

        public long Id => this._supplier.Id;
        public string Name => this._supplier.Name;
    }
}
