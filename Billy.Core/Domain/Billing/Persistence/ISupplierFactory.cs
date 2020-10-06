using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Billy.Domain.Billing.Models;

namespace Billy.Core.Billing.Persistence
{
    [Obsolete("new application structure does not use this")]
    public interface ISupplierFactory
    {
        Task<Supplier> CreateAsync(Func<int, IEnumerable<KeyValuePair<string, object>>> factoryMethod);
    }
}