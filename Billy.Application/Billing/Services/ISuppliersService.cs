using Billy.Billing.Application.DTOs;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public interface IReadSuppliersService
    {
        IObservable<SupplierDto> Added { get; }
        IObservable<IReadOnlyCollection<SupplierDto>> Updated { get; }
        IObservable<IReadOnlyCollection<long>> Removed { get; }

        Task<IReadOnlyCollection<SupplierDto>> GetAllAsync();

        IObservable<IChangeSet<SupplierDto, long>> Changes { get; }
        IObservableCache<SupplierDto, long> Cache { get; }
    }

    public interface IWriteSuppliersService
    {
        Task<SupplierDto> CreateAndAddAsync(IDictionary<string, object> data);
        Task UpdateAsync(long supplierId, IDictionary<string, object> changes);
        Task<bool> RemoveAsync(long id);
    }

    public interface ISuppliersService : IReadSuppliersService, IWriteSuppliersService
    {
    }
}