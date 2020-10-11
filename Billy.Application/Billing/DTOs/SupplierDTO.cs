using System;
using System.Collections.Generic;
using System.Text;
using Billy.Domain.Billing.Models;

namespace Billy.Billing.Application.DTOs
{
    public sealed class SupplierDto
    {
        /* TODO: what happens if underlying entity changes property and for some reason UI re-scans the DTO?
         * better copy properties and not return from the source? probably yes */
        private readonly Supplier _supplier;

        public SupplierDto(Supplier supplier)
        {
            this._supplier = supplier;
        }

        public int Id => this._supplier.Id;
        public string Name => this._supplier.Name;
        public Agent Agent => this._supplier.Agent;
        public string Email => this._supplier.Email;
        public string Website => this._supplier.Website;
        public string Phone => this._supplier.Phone;
        public string Fax => this._supplier.Fax;
        public string Notes => this._supplier.Notes;
        public Address Address => this._supplier.Address;
    }
}
