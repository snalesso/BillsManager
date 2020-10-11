using System;
using System.Threading.Tasks;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Persistence
{
    public interface IBillFactory
    {
        Task<Bill> CreateAsync(Func<uint, Bill> factoryMethod);
    }
}