using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public static class IWriteSuppliersRepositoryExtensions
    {
        public static Task UpdateAsync(this IWriteSuppliersRepository suppliersRepository, Supplier supplier, IEnumerable<KeyValuePair<string, object>> changes)
        {
            return (suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository))).UpdateAsync(supplier.Id, changes);
        }

        public static Task RemoveAsync(this IWriteSuppliersRepository suppliersRepository, Supplier supplier)
        {
            return (suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository))).RemoveAsync(supplier.Id);
        }

        public static Task RemoveAsync(this IWriteSuppliersRepository suppliersRepository, IEnumerable<Supplier> suppliers)
        {
            return (suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository))).RemoveAsync(suppliers.Select(x => x.Id));
        }
    }
}