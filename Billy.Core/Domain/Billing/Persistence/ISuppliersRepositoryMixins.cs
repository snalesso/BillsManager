using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billy.Domain.Billing.Models;

namespace Billy.Domain.Billing.Persistence
{
    public static class ISuppliersRepositoryMixins
    {
        public static Task UpdateAsync(this ISuppliersRepository suppliersRepository, Supplier supplier, IEnumerable<KeyValuePair<string, object>> changes)
        {
            return (suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository))).UpdateAsync(supplier.Id, changes);
        }

        public static Task RemoveAsync(this ISuppliersRepository suppliersRepository, Supplier supplier)
        {
            return (suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository))).RemoveAsync(supplier.Id);
        }

        public static Task RemoveAsync(this ISuppliersRepository suppliersRepository, IEnumerable<Supplier> suppliers)
        {
            return (suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository))).RemoveAsync(suppliers.Select(x => x.Id));
        }
    }
}