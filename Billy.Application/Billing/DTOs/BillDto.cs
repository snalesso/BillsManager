using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    // TODO: consider changing name to something like "XDetailsDto"/"XSummaryDto"
    public sealed class BillDto
    {
        private readonly Bill _bill;

        public BillDto(Bill bill)
        {
            this._bill = bill ?? throw new ArgumentNullException(nameof(bill));
        }

        public long Id => this._bill.Id;
        public long SupplierId => this._bill.SupplierId;
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
