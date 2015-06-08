using System;
using BillsManager.Model;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BillItemViewModel :
        BillViewModel
    {
        #region fields

        private readonly IWindowManager windowManager;
        protected readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public BillItemViewModel(
            Bill bill,
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            if (bill == null)
                throw new ArgumentNullException("bill cannot be null");

            this.exposedBill = bill;

            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        #region wrapped from bill

        public override DateTime DueDate
        {
            set
            {
                if (this.DueDate != value)
                {
                    this.ExposedBill.DueDate = value;
                    this.NotifyOfPropertyChange(() => this.DueDate);
                    this.NotifyOfPropertyChange(() => this.IsDued);
                    this.NotifyOfPropertyChange(() => this.DuesIn);
                }
            }
        }

        public override DateTime? PaymentDate
        {
            set
            {
                if (this.PaymentDate != value)
                {
                    this.ExposedBill.PaymentDate = value;
                    this.NotifyOfPropertyChange(() => this.PaymentDate);
                    this.NotifyOfPropertyChange(() => this.IsPaid);
                    this.NotifyOfPropertyChange(() => this.IsNotPaid);
                    this.NotifyOfPropertyChange(() => this.IsDued);
                    this.NotifyOfPropertyChange(() => this.DuesIn);
                }
            }
        }

        #endregion

        #region added

        private Supplier supplier;
        public Supplier Supplier
        {
            get { return this.supplier; }
            set
            {
                if (this.supplier != value)
                {
                    this.supplier = value;
                    this.NotifyOfPropertyChange(() => this.Supplier);
                }
            }
        }

        public bool IsPaid
        {
            get { return this.PaymentDate.HasValue; }
            set
            {
                if (this.IsPaid != value)
                {
                    if (value) this.PaymentDate = DateTime.Today;
                    else this.PaymentDate = null;
                    // changing only PaymentDate will Refresh IsPaid and similars
                }
            }
        }

        public bool IsNotPaid
        {
            get { return !this.IsPaid; }
        }

        public DateTime DisplayDateEndForPaymentDate
        {
            get { return DateTime.Today; }
        }

        public bool IsDued
        {
            get { return this.DueDate < DateTime.Today; }
        }

        public string DuesIn
        {
            get
            {
                var timeleft = this.DueDate.Subtract(DateTime.Today);

                if (timeleft.TotalDays >= 0)
                {
                    if (timeleft.TotalDays == 0) return "Overdues today"; // TODO: language
                    if (timeleft.TotalDays == 1) return "Overdues tomorrow";
                    return timeleft.TotalDays.ToString() + " days left";
                }
                else
                {
                    if (timeleft.TotalDays == -1) return "Overdue yesterday";
                    return "Overdue " + (timeleft.TotalDays * -1).ToString() + " days ago"; // TODO: language
                }
            }
        }

        #endregion

        #endregion
    }
}