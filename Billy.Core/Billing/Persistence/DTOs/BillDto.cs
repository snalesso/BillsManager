using Billy.Billing.Models;
using System;

namespace Billy.Billing.Application.DTOs
{
    public record BillDto
    {
        public long Id { get; init; }
        public long SupplierId { get; init; }
        public DateTime ReleaseDate { get; init; }
        public DateTime DueDate { get; init; }
        public DateTime? PaymentDate { get; init; }
        public DateTime RegistrationDate { get; init; }
        public double Amount { get; init; }
        public double Agio { get; init; }
        public double AdditionalCosts { get; init; }
        public string Notes { get; init; }
        public string Code { get; init; }

        public static BillDto From(Bill bill)
        {
            if (bill is null)
                throw new ArgumentNullException(nameof(bill));

            return new()
            {
                Id = bill.Id,
                SupplierId = bill.SupplierId,
                ReleaseDate = bill.ReleaseDate,
                DueDate = bill.DueDate,
                PaymentDate = bill.PaymentDate,
                RegistrationDate = bill.RegistrationDate,
                Amount = bill.Amount,
                Agio = bill.Agio,
                AdditionalCosts = bill.AdditionalCosts,
                Notes = bill.Notes,
                Code = bill.Code
            };
        }

        //private readonly Bill _bill;
        //public long Id => this._bill.Id;
        //public long SupplierId => this._bill.SupplierId;
        //public DateTime ReleaseDate => this._bill.ReleaseDate;
        //public DateTime DueDate => this._bill.DueDate;
        //public DateTime? PaymentDate => this._bill.PaymentDate;
        //public DateTime RegistrationDate => this._bill.RegistrationDate;
        //public double Amount => this._bill.Amount;
        //public double Agio => this._bill.Agio;
        //public double AdditionalCosts => this._bill.AdditionalCosts;
        //public string Notes => this._bill.Notes;
        //public string Code => this._bill.Code;
    }
}
