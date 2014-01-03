using BillsManager.Models;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;

namespace BillsManager.ViewModels
{
    // TODO: add cell color for remaining due time in details view
    public partial class BillDetailsViewModel :
        BillViewModel,
        IHandle<SupplierEditedMessage>
    {
        #region fields

        protected readonly IWindowManager windowManager;
        //protected readonly IEventAggregator globalEventAggregator;
        protected readonly IEventAggregator dbEventAggregator;

        #endregion

        #region ctor

        public BillDetailsViewModel(
            IWindowManager windowManager,
            //IEventAggregator globalEventAggregator,
            IEventAggregator dbEventAggregator,
            Bill bill)
        {
            if (bill == null)
                throw new ArgumentNullException("bill cannot be null");

            this.exposedBill = bill;

            this.windowManager = windowManager;
            //this.globalEventAggregator = globalEventAggregator;
            this.dbEventAggregator = dbEventAggregator;

            //this.globalEventAggregator.Subscribe(this);
            this.dbEventAggregator.Subscribe(this);

            //this.SupplierName = this.GetSupplierName(this.SupplierID);

            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        //this.globalEventAggregator.Unsubscribe(this);
                        this.dbEventAggregator.Unsubscribe(this);
                    }
                };
        }

        #endregion

        #region properties

        #region wrapped from bill

        public override DateTime DueDate
        {
            get { return this.ExposedBill.DueDate; }
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
            get { return this.ExposedBill.PaymentDate; }
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

        public string SupplierName
        {
            get
            {
                return this.GetSupplierName();
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
                // TODO: optimization benchmark (overdue + s ...)
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
                    return "Overdue " + (timeleft.TotalDays * -1).ToString() + " days ago";
                }
            }
        }

        #endregion

        #region override

        public new string DisplayName
        {
            get { return this.Code + " - " + this.SupplierName; } // TODO: fix
        }

        #endregion

        #endregion

        #region methods

        private string GetSupplierName()
        {
            // TODO: move supplier logic to BillsViewModel (same for supp's obligation amount)
            string supp = string.Empty;
            this.dbEventAggregator.Publish(new SupplierNameRequestMessage(this.SupplierID, s => supp = s));
            return supp;
        }

        private void SwitchToEdit()
        {
            this.TryClose();
            this.dbEventAggregator.Publish(new EditBillRequestMessage(this.ExposedBill));
        }

        #region message handlers

        public void Handle(SupplierEditedMessage message)
        {
            if (this.SupplierID == message.OldSupplierVersion.ID)
            {
                this.NotifyOfPropertyChange(() => this.SupplierName);
            }
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand switchToEditCommand;
        public RelayCommand SwitchToEditCommand
        {
            get
            {
                if (this.switchToEditCommand == null)
                    this.switchToEditCommand = new RelayCommand(
                        () => this.SwitchToEdit());

                return this.switchToEditCommand;
            }
        }

        private RelayCommand closeDetailsViewCommand;
        public RelayCommand CloseDetailsViewCommand
        {
            get
            {
                if (this.closeDetailsViewCommand == null)
                    this.closeDetailsViewCommand = new RelayCommand(
                    () => this.TryClose());

                return this.closeDetailsViewCommand;
            }
        }

        #endregion
    }
}