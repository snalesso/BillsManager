using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Persistence
{
    public static class IBillsRepositoryExtensions
    {
        //public static Task RemoveAsync(this IBillsRepository billsRepository, Bill bill)
        //{
        //    return (billsRepository ?? throw new ArgumentNullException(nameof(billsRepository))).RemoveAsync(bill.Id);
        //}

        //public static Task RemoveAsync(this IBillsRepository billsRepository, IEnumerable<Bill> bills)
        //{
        //    return (billsRepository ?? throw new ArgumentNullException(nameof(billsRepository))).RemoveAsync(bills.Select(x => x.Id));
        //}
    }
}