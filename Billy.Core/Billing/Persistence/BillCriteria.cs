using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billy.Billing.Persistence
{
    public record BillCriteria
    {
        public IEnumerable<long> Ids { get; init; }
        public long? SupplierId { get; init; }
        public bool? IsPaid { get; init; } = false;
        public Range<DateTime> ReleaseDateRange { get; init; }
        public Range<DateTime> DueDateRange { get; init; }
        public Range<DateTime> PaymentDateRange { get; init; }
        public string FullText { get; init; }
    }
}
