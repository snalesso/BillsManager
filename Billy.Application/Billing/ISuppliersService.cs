using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Billy.Application.Servicing;
using Billy.Core.Domain.Billing.Application.DTOs;
using Billy.Domain.Billing.Models;
using DynamicData;

namespace Billy.Billing.Application
{
    public interface ISuppliersService : IService
    {
        // READ
        IObservable<SupplierDTO> Added { get; }
        IObservable<IReadOnlyCollection<SupplierDTO>> Updated { get; }
        IObservable<IReadOnlyCollection<int>> Removed { get; }

        IObservable<IChangeSet<SupplierDTO, int>> SuppliersChanges { get; }

        Task<IReadOnlyCollection<SupplierDTO>> GetAllAsync();

        //IQbservable<Supplier> Suppliers { get; }
        //Task<SupplierDTO> GetSingle();

        // WRITE
        Task<SupplierDTO> CreateAndAddAsync(IDictionary<string, object> data);
        Task UpdateAsync(int supplierIdd, IDictionary<string, object> changes);
        Task<bool> RemoveAsync(int id);
        //Task AddBillToSupplierAsync(int supplierId, IDictionary<string, object> data);
    }
}