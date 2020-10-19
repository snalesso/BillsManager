﻿using Billy.Billing.Application.DTOs;
using Billy.Communication;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Services
{
    public interface ISuppliersService : IService
    {
        // READ
        IObservable<SupplierDto> Added { get; }
        IObservable<IReadOnlyCollection<SupplierDto>> Updated { get; }
        IObservable<IReadOnlyCollection<int>> Removed { get; }

        IObservable<IChangeSet<SupplierDto, int>> SuppliersChanges { get; }

        Task<IReadOnlyCollection<SupplierDto>> GetAllAsync();

        //IQbservable<Supplier> Suppliers { get; }
        //Task<SupplierDTO> GetSingle();

        // WRITE
        Task<SupplierDto> CreateAndAddAsync(IDictionary<string, object> data);
        Task UpdateAsync(int supplierIdd, IDictionary<string, object> changes);
        Task<bool> RemoveAsync(int id);
        //Task AddBillToSupplierAsync(int supplierId, IDictionary<string, object> data);
    }
}