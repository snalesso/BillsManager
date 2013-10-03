using System;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BillDetailsViewModel :
        BillViewModel
    {
        #region fields

        protected readonly IWindowManager windowManager;
        protected readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public BillDetailsViewModel(
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

            this.Supplier = this.GetSupplier(this.SupplierID);
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
                    return "Overdue " + (timeleft.TotalDays * -1).ToString() + " days ago"; // TODO: language
                }
            }
        }

        #endregion

        #region override

        public new string DisplayName
        {
            get { return this.Code + " - " + this.Supplier.Name; }
        }

        #endregion

        #endregion

        #region methods

        private Supplier GetSupplier(uint supplierID)
        {
            // TODO: move supplier logic to BillsViewModel (same for supp's obligation amount)
            Supplier supp = null;
            this.eventAggregator.Publish(new AskForSupplierMessage(supplierID, s => supp = s));
            return supp;
        }

        #endregion

        #region commands

        private RelayCommand switchToEditCommand;
        public RelayCommand SwitchToEditCommand
        {
            get
            {
                if (this.switchToEditCommand == null)
                    this.switchToEditCommand = new RelayCommand(
                        () =>
                        {
                            this.TryClose();
                            this.eventAggregator.Publish(new EditBillRequestMessage(this.ExposedBill));
                        });

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
                    () =>
                    {
                        this.TryClose();
                    });

                return this.closeDetailsViewCommand;
            }
        }

        #endregion
    }
}