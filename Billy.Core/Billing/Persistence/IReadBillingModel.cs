using Billy.Billing.Application.DTOs;
using Billy.Billing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IReadBillsRepository
    {
        Task<Bill> GetByIdAsync(long id);
        Task<Bill> GetSingleAsync(BillCriteria billCriteria);
        Task<IReadOnlyCollection<Bill>> GetMultipleAsync(BillCriteria criteria = null);
    }
    public interface IReadSuppliersRepository
    {
        Task<Supplier> GetByIdAsync(long id);
        Task<Supplier> GetSingleAsync(SupplierCriteria criteria = null);
        Task<IReadOnlyCollection<Supplier>> GetMultipleAsync(SupplierCriteria criteria = null);
    }
    public interface IReadBillingModel
    {
        IReadBillsRepository Bills { get; }
        IReadSuppliersRepository Suppliers { get; }

        Task<SupplierHeaderDto> GetSupplierHeadersAsync(SupplierCriteria criteria = null);
        Task<DetailedBillDto> GetDetailedBillsAsync(BillCriteria criteria = null);

        Task<IReadOnlyCollection<SupplierSummaryDto>> GetSupplierSummariesAsync(SupplierCriteria criteria = null);
    }
}