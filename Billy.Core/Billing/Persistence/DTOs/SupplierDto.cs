using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    // TODO: consider changing name to something like "XDetailsDto"/"XSummaryDto"
    public record SupplierDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public Agent Agent { get; init; }
        public string Email { get; init; }
        public string Website { get; init; }
        public string Phone { get; init; }
        public string Fax { get; init; }
        public string Notes { get; init; }
        public Address Address { get; init; }

        public static SupplierDto From(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            return new()
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Agent = supplier.Agent,
                Email = supplier.Email,
                Website = supplier.Website,
                Phone = supplier.Phone,
                Fax = supplier.Fax,
                Notes = supplier.Notes,
                Address = supplier.Address
            };
        }

        /* TODO: what happens if underlying entity changes property and for some reason UI re-scans the DTO?
         * better copy properties and not return from the source? probably yes */
        // TODO: what happens if the system wants to dispose the entity, and there's a reference to it held in here?
        //private readonly Supplier _supplier;

        //public SupplierDto(Supplier supplier)
        //{
        //    this._supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        //}

        //public long Id => this._supplier.Id;
        //public string Name => this._supplier.Name;
        //public Agent Agent => this._supplier.Agent;
        //public string Email => this._supplier.Email;
        //public string Website => this._supplier.Website;
        //public string Phone => this._supplier.Phone;
        //public string Fax => this._supplier.Fax;
        //public string Notes => this._supplier.Notes;
        //public Address Address => this._supplier.Address;
    }
}
