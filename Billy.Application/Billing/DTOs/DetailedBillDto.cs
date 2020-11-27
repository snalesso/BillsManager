using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    public sealed class DetailedBillDto
    {
        private readonly Bill _bill;

        public DetailedBillDto(Bill bill, SupplierHeaderDto supplierHeader)
        {
            this._bill = bill ?? throw new ArgumentNullException(nameof(bill));
            this.SupplierHeader = supplierHeader ?? throw new ArgumentNullException(nameof(supplierHeader));
        }

        public long Id => this._bill.Id;

        public SupplierHeaderDto SupplierHeader { get; }

        public DateTime ReleaseDate => this._bill.ReleaseDate;
        public DateTime DueDate => this._bill.DueDate;
        public DateTime? PaymentDate => this._bill.PaymentDate;
        public DateTime RegistrationDate => this._bill.RegistrationDate;
        public double Amount => this._bill.Amount;
        public double Agio => this._bill.Agio;
        public double AdditionalCosts => this._bill.AdditionalCosts;
        public string Notes => this._bill.Notes;
        public string Code => this._bill.Code;
    }
}
