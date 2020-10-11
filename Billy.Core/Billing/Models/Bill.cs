using Billy.Domain.Models;
using System;

namespace Billy.Domain.Billing.Models
{
    // TODO: ensure try-catch blocks await returned Tasks in all the source (written here so this comment will not get accidentally deleted)
    public class Bill : Entity<int>//, IBill
    {
        #region ctor

        public Bill(
            int id,
            uint supplierID,
            DateTime registrationDate,
            DateTime dueDate,
            DateTime releaseDate,
            DateTime? paymentDate,
            double amount,
            double agio,
            double costs,
            string code,
            string notes = null)
            : base(id)
        {
            this.SupplierID = supplierID;
            this.RegistrationDate = registrationDate;
            this.DueDate = dueDate;
            this.ReleaseDate = releaseDate;
            this.PaymentDate = paymentDate;
            this.Amount = amount;
            this.Agio = agio;
            this.AdditionalCosts = costs;
            this.Code = code;
            this.Notes = notes;
        }

        #endregion

        #region properties

        private DateTime _registrationDate;
        public DateTime RegistrationDate
        {
            get { return this._registrationDate; }
            set { this.SetAndRaiseIfChanged(ref this._registrationDate, value); }
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

        private DateTime _releaseDate;
        public DateTime ReleaseDate
        {
            get { return this._releaseDate; }
            set { this.SetAndRaiseIfChanged(ref this._releaseDate, value); }
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

        private uint _supplierId;
        public uint SupplierID
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

        #region Entity

        //protected override void EnsureIsWellFormattedId(int id)
        //{
        //    if (id <= 0)
        //        // TODO: create ad-hoc exception (e.g. InvalidIdValueException)
        //        throw new ArgumentException($"{this.GetType().FullName}.{nameof(this.Id)} cannot be set to {id}.", nameof(id));
        //}

        #endregion
    }
}