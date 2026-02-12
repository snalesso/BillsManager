using Billy.Billing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.Read
{
    public class SQLiteDapperReadWriteBillingModel : IReadBillingModel, IWriteBillingModel
    {
        public ISuppliersRepository Suppliers => throw new NotImplementedException();
        public IBillsRepository Bills => throw new NotImplementedException();

        #region write

        IWriteBillsRepository IWriteBillingModel.Bills => this.Bills;
        IWriteSuppliersRepository IWriteBillingModel.Suppliers => this.Suppliers;

        #endregion

        #region read

        IReadBillsRepository IReadBillingModel.Bills => this.Bills;
        IReadSuppliersRepository IReadBillingModel.Suppliers => this.Suppliers;

        public Task<DetailedBillDto> GetDetailedBillsAsync(BillCriteria criteria = null)
        {
            throw new NotImplementedException();
        }

        public Task<SupplierHeaderDto> GetSupplierHeadersAsync(SupplierCriteria criteria = null)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<SupplierSummaryDto>> GetSupplierSummariesAsync(SupplierCriteria criteria = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
