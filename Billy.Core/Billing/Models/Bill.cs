using Billy.Domain.Models;
using System;

namespace Billy.Billing.Models
{
    // TODO: ensure try-catch blocks await returned Tasks in all the source (written here so this comment will not get accidentally deleted)
    public class Bill : Entity<long>//, IBill
    {
        #region ctors

        public Bill(
            long id,
            long supplierId,
            DateTime releaseDate,
            DateTime dueDate,
            DateTime? paymentDate,
            DateTime registrationDate,
            double amount,
            double agio,
            double additionalCosts,
            string code,
            string notes)
            : base(id)
        {
            this.SupplierId = supplierId;
            this.RegistrationDate = registrationDate;
            this.DueDate = dueDate;
            this.ReleaseDate = releaseDate;
            this.PaymentDate = paymentDate;
            this.Amount = amount;
            this.Agio = agio;
            this.AdditionalCosts = additionalCosts;
            this.Code = code;
            this.Notes = notes;
        }

        public Bill(
            long id,
            long supplierId,
            DateTime releaseDate,
            DateTime dueDate,
            DateTime? paymentDate,
            double amount,
            double agio,
            double additionalCosts,
            string code,
            string notes)
            : this(id, supplierId, releaseDate, dueDate, paymentDate, DateTime.Now, amount, agio, additionalCosts, code, notes)
        {
        }

        #endregion

        #region properties

        private DateTime _releaseDate;
        public DateTime ReleaseDate
        {
            get { return this._releaseDate; }
            set { this.SetAndRaiseIfChanged(ref this._releaseDate, value); }
        }

        private DateTime _dueDate;
        public DateTime DueDate
        {
            get { return this._dueDate; }
            set { this.SetAndRaiseIfChanged(ref this._dueDate, value); }
        }

        private DateTime? _paymentDate;
        public DateTime? PaymentDate
        {
            get { return this._paymentDate; }
            set { this.SetAndRaiseIfChanged(ref this._paymentDate, value); }
        }

        private DateTime _registrationDate;
        public DateTime RegistrationDate
        {
            get { return this._registrationDate; }
            set { this.SetAndRaiseIfChanged(ref this._registrationDate, value); }
        }

        private double _amount;
        public double Amount
        {
            get { return this._amount; }
            set { this.SetAndRaiseIfChanged(ref this._amount, value); }
        }

        private double _agio;
        public double Agio
        {
            get { return this._agio; }
            set { this.SetAndRaiseIfChanged(ref this._agio, value); }
        }

        private double _additionalCosts;
        public double AdditionalCosts
        {
            get { return this._additionalCosts; }
            set { this.SetAndRaiseIfChanged(ref this._additionalCosts, value); }
        }

        private long _supplierId;
        public long SupplierId
        {
            get { return this._supplierId; }
            set { this.SetAndRaiseIfChanged(ref this._supplierId, value); }
        }

        private string _notes;
        public string Notes
        {
            get { return this._notes; }
            set { this.SetAndRaiseIfChanged(ref this._notes, value); }
        }

        private string _code;
        public string Code
        {
            get { return this._code; }
            set { this.SetAndRaiseIfChanged(ref this._code, value); }
        }

        #endregion
    }
}