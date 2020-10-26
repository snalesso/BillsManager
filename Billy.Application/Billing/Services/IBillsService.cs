using Billy.Billing.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public interface IBillsService
    {
        // READ
        IObservable<BillDto> Added { get; }
        IObservable<IReadOnlyCollection<BillDto>> Updated { get; }
        IObservable<IReadOnlyCollection<long>> Removed { get; }

        Task<IReadOnlyCollection<BillDto>> GetAsync();

        // WRITE
        Task<BillDto> CreateAndAddAsync(IDictionary<string, object> data);
        Task UpdateAsync(long billId, IDictionary<string, object> changes);
        Task<bool> RemoveAsync(long id);
    }
}