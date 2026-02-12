using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billy.Billing.Persistence
{
    public record SupplierCriteria
    {
        public long Id { get; init; }
        public string FullTextSearch { get; init; }
    }
}
