using Billy.Billing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface ISupplierHeadersView
    {
        Task<IReadOnlyCollection<SupplierHeaderDto>> GetMultipleAsync(SupplierCriteria criteria = null);
    }
    public interface ISupplierSummaries
    {
        Task<IReadOnlyCollection<SupplierSummaryDto>> GetMultipleAsync(SupplierCriteria criteria = null);
    }
    public interface IDetailedBillsView
    {
        Task<IReadOnlyCollection<DetailedBillDto>> GetMultipleAsync(BillCriteria billCriteria = null, SupplierCriteria supplierCriteria = null);
    }
}
