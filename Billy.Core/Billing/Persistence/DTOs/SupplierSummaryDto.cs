using Billy.Billing.Models;
using System;

namespace Billy.Billing.Persistence
{
    public record SupplierSummaryDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Notes { get; init; }

        // calculated properties
        public double AmountsSum { get; init; }

        public static SupplierSummaryDto From(Supplier supplier, double amountsSum)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            return new()
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Notes = supplier.Notes,
                AmountsSum = amountsSum
            };
        }
    }
}