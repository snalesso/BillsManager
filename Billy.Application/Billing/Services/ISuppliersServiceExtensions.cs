using Billy.Billing.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public static class ISuppliersServiceExtensions
    {
        public static Task<SupplierDto> CreateAndAddAsync(this ISuppliersService service, IReadOnlyDictionary<string, object> data)
        {
            return service.CreateAndAddAsync(new Dictionary<string, object>(data));
        }
        public static Task UpdateAsync(this ISuppliersService service, long supplierId, IReadOnlyDictionary<string, object> changes)
        {
            return service.UpdateAsync(supplierId, new Dictionary<string, object>(changes));
        }
    }
}