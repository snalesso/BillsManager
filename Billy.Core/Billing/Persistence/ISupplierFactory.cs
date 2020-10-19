using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    [Obsolete("new application structure does not use this")]
    public interface ISupplierFactory
    {
        Task<Supplier> CreateAsync(Func<int, IEnumerable<KeyValuePair<string, object>>> factoryMethod);
    }
}