using System;

namespace BillsManager.Models
{
    // URGENT: add aggio/spese
    public partial class Bill
    {
        #region ctor

        public Bill(uint id)
        {
            this.id = id;
        }

        public Bill(
            uint id,
            /*uint tagID,*/
            DateTime registrationDate,
            DateTime dueDate,
            DateTime? paymentDate,
            DateTime releaseDate,
            Double amount,
            /*Double gain,
            Double expense,*/
            uint supplierID,
            string notes,
            string code)
            : this(id)
        {
            /*this.TagID = tagID;*/
            this.Amount = amount;
            this.Code = code;
            this.DueDate = dueDate;
            /*this.Expense = expense;
            this.Gain = gain;*/
            this.Notes = notes;
            this.PaymentDate = paymentDate;
            this.RegistrationDate = registrationDate;
            this.ReleaseDate = releaseDate;
            this.SupplierID = supplierID;
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

        /*private uint tagID;
        public uint TagID // IDEA: nullable?
        {
            get { return this.tagID; }
            set
            {
                if (this.tagID == value) return;

                this.tagID = value;
            }
        }*/

        private DateTime registrationDate = DateTime.Today;
        public DateTime RegistrationDate
        {
            get { return this.registrationDate; }
            set { this.registrationDate = value; }
        }

        private DateTime dueDate = DateTime.Today;
        public DateTime DueDate
        {
            get { return this.dueDate; }
            set { this.dueDate = value; }
        }

        private DateTime? paymentDate = null;
        public DateTime? PaymentDate
        {
            get { return this.paymentDate; }
            set { this.paymentDate = value; }
        }

        private DateTime releaseDate = DateTime.Today;
        public DateTime ReleaseDate
        {
            get { return this.releaseDate; }
            set { this.releaseDate = value; }
        }

        private Double amount;
        public Double Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }

        /*private Double gain;
        public Double Gain
        {
            get { return this.gain; }
            set
            {
                this.gain = value;
            }
        }

        private Double expense;
        public Double Expense // URGENT: review the term
        {
            get { return this.expense; }
            set { this.expense = value; }
        }*/

        private uint supplierID; // TODO: find a default value that means no supplier (0 means the first created supplier)
        public uint SupplierID
        {
            get { return this.supplierID; }
            set { this.supplierID = value; }
        }

        private string notes;
        public string Notes
        {
            get { return this.notes; }
            set { this.notes = value; }
        }

        private string code;
        public string Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        #endregion
    }
}