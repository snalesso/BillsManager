using Billy.Billing.Application.DTOs;
using Billy.Billing.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public interface IReadBillsService
    {
        IObservable<BillDto> Added { get; }
        IObservable<IReadOnlyCollection<BillDto>> Updated { get; }
        IObservable<IReadOnlyCollection<long>> Removed { get; }

        Task<IReadOnlyCollection<BillDto>> GetAsync(BillCriteria billCriteria = null);
    }

    public interface IWriteBillsService
    {
        Task<BillDto> CreateAndAddAsync(IDictionary<string, object> data);
        Task UpdateAsync(long billId, IDictionary<string, object> changes);
        Task<bool> RemoveAsync(long id);
    }

    public interface IBillsService : IReadBillsService, IWriteBillsService
    {
    }
}