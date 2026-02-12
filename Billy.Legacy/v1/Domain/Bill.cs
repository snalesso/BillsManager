using System;

namespace BillsManager.v1.Models
{
    public partial class Bill
    {
        #region ctor

        public Bill(uint id)
            : this(
            id, 
            0, 
            DateTime.Today, 
            DateTime.Today,
            DateTime.Today, 
            null, 
            0, 
            0, 
            0, 
            null, 
            null)
        {
        }

        public Bill(
            uint id,
            uint supplierID,
            DateTime registrationDate, 
            DateTime dueDate,
            DateTime releaseDate, 
            DateTime? paymentDate, 
            Double amount, 
            Double agio, 
            Double costs, 
            string code, 
            string notes)
        {
            this.AdditionalCosts = costs;
            this.Agio = agio;
            this.Amount = amount;
            this.Code = code;
            this.DueDate = dueDate;
            this.id = id;
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
        }

        public DateTime RegistrationDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime ReleaseDate { get; set; }

        public Double Amount { get; set; }

        public Double Agio { get; set; }

        public Double AdditionalCosts { get; set; }

        public uint SupplierID { get; set; }

        public string Notes { get; set; }

        public string Code { get; set; }

        #endregion
    }
}