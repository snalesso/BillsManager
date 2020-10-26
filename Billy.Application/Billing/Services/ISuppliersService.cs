using Billy.Billing.Application.DTOs;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public interface ISuppliersService
    {
        // READ
        IObservable<SupplierDto> Added { get; }
        IObservable<IReadOnlyCollection<SupplierDto>> Updated { get; }
        IObservable<IReadOnlyCollection<long>> Removed { get; }

        IObservable<IChangeSet<SupplierDto, long>> SuppliersChanges { get; }
        IObservableCache<SupplierDto, long> Suppliers { get; }

        Task<IReadOnlyCollection<SupplierDto>> GetAllAsync();

        //IQbservable<Supplier> Suppliers { get; }
        //Task<SupplierDTO> GetSingle();

        // WRITE
        Task<SupplierDto> CreateAndAddAsync(IDictionary<string, object> data);
        Task UpdateAsync(long supplierId, IDictionary<string, object> changes);
        Task<bool> RemoveAsync(long id);
        //Task AddBillToSupplierAsync(int supplierId, IDictionary<string, object> data);
    }
}