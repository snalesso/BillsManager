using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    public record SupplierHeaderDto
    {
        public long Id { get; init; }
        public string Name { get; init; }

        public static SupplierHeaderDto From(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            return new()
            {
                Id = supplier.Id,
                Name = supplier.Name
            };
        }
    }
}
