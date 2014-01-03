using System;

namespace BillsManager.Models
{
    public partial class Bill
    {
        #region ctor

        public Bill(
            uint id)
        {
            this.id = id;
        }

        public Bill(
            uint id,
            DateTime RegistrationDate,
            DateTime DueDate,
            DateTime? PaymentDate,
            DateTime ReleaseDate,
            Double Amount,
            uint SupplierID,
            string Notes,
            string Code)
        {
            this.Amount = Amount;
            this.Code = Code;
            this.DueDate = DueDate;
            this.id = id;
            this.Notes = Notes;
            this.PaymentDate = PaymentDate;
            this.RegistrationDate = RegistrationDate;
            this.ReleaseDate = ReleaseDate;
            this.SupplierID = SupplierID;
        }

        #endregion

        #region properties

        private readonly uint id = 0;
        public uint ID
        {
            get { return this.id; }
            //private set
            //{
            //    this.id = value;
            //}
        }

        private DateTime registrationDate = DateTime.Today;
        public DateTime RegistrationDate
        {
            get { return this.registrationDate; }
            set
            {
                if (this.RegistrationDate != value)
                {
                    this.registrationDate = value;
                }
            }
        }

        private DateTime dueDate = DateTime.Today;
        public DateTime DueDate
        {
            get { return this.dueDate; }
            set
            {
                if (this.DueDate != value)
                {
                    this.dueDate = value;
                }
            }
        }

        private DateTime? paymentDate = null;
        public DateTime? PaymentDate
        {
            get { return this.paymentDate; }
            set
            {
                if (this.PaymentDate != value)
                {
                    this.paymentDate = value;
                }
            }
        }

        private DateTime releaseDate = DateTime.Today;
        public DateTime ReleaseDate
        {
            get { return this.releaseDate; }
            set
            {
                if (this.ReleaseDate != value)
                {
                    this.releaseDate = value;
                }
            }
        }

        private Double amount;
        public Double Amount
        {
            get { return this.amount; }
            set
            {
                if (this.Amount != value)
                {
                    this.amount = value;
                }
            }
        }

        private uint supplierID; // TODO: find a default value that means no supplier (0 means the first created supplier)
        public uint SupplierID
        {
            get { return this.supplierID; }
            set
            {
                if (this.SupplierID != value)
                {
                    this.supplierID = value;
                }
            }
        }

        private string notes;
        public string Notes
        {
            get { return this.notes; }
            set
            {
                if (this.Notes != value)
                {
                    this.notes = value;
                }
            }
        }

        private string code;
        public string Code
        {
            get { return this.code; }
            set
            {
                if (this.Code != value)
                {
                    this.code = value;
                }
            }
        }

        #endregion
    }
}