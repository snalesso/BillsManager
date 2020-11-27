using Billy.Billing.Application.DTOs;
using Billy.Domain.CQRS.Query;
using System.Collections.Generic;

namespace Billy.Billing.Persistence.CQRS.Read
{
    public record SupplierSummariesQuery : IQuery<IReadOnlyCollection<SupplierSummaryDto>> { }
    public record SupplierHeadersQuery : IQuery<IReadOnlyCollection<SupplierHeaderDto>> { }
    public record DetailedBillsQuery : IQuery<IReadOnlyCollection<DetailedBillDto>> { }
}
