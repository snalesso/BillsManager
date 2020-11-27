using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    public sealed class DetailedSupplierDto
    {
        /* TODO: what happens if underlying entity changes property and for some reason UI re-scans the DTO?
         * better copy properties and not return from the source? probably yes */
        // TODO: what happens if the system wants to dispose the entity, and there's a reference to it held in here?
        private readonly Supplier _supplier;

        public DetailedSupplierDto(Supplier supplier, double amountsSum)
        {
            this._supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));

            this.AmountsSum = amountsSum;
        }

        public long Id => this._supplier.Id;
        public string Name => this._supplier.Name;
        public Agent Agent => this._supplier.Agent;
        public string Email => this._supplier.Email;
        public string Website => this._supplier.Website;
        public string Phone => this._supplier.Phone;
        public string Fax => this._supplier.Fax;
        public string Notes => this._supplier.Notes;
        public Address Address => this._supplier.Address;

        public double AmountsSum { get; }
    }
}
