using Billy.Billing.Models;
using System;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface IBillFactory
    {
        Task<Bill> CreateAsync(Func<uint, Bill> factoryMethod);
    }
}