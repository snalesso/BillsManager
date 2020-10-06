using System.Collections.Generic;
using System.Threading.Tasks;
using Billy.Core.Domain.Billing.Application.DTOs;

namespace Billy.Billing.Application
{
    public static class ISuppliersServiceExtensions
    {
        public static Task<SupplierDTO> CreateAndAddAsync(this ISuppliersService service, IReadOnlyDictionary<string, object> data)
        {
            return service.CreateAndAddAsync(new Dictionary<string, object>(data));
        }
        public static Task UpdateAsync(this ISuppliersService service, int supplierId, IReadOnlyDictionary<string, object> changes)
        {
            return service.UpdateAsync(supplierId, new Dictionary<string, object>(changes));
        }
    }
}