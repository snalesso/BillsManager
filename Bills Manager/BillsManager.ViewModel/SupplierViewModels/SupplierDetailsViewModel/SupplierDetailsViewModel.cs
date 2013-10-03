using System;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SupplierDetailsViewModel :
        SupplierViewModel,
        IHandle<BillsListChangedMessage>,
        IHandle<BillAddedMessage>,
        IHandle<BillDeletedMessage>,
        IHandle<BillEditedMessage>
    {
        #region fields

        protected readonly IWindowManager windowManager;
        protected readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public SupplierDetailsViewModel(
            Supplier supplier,
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            if (supplier == null)
                throw new ArgumentNullException("supplier cannot be null");

            this.ExposedSupplier = supplier;

            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        #region wrapped from supplier

        // TODO: perform a clean override on all the other props
        public override string City
        {
            get
            {
                return base.City;
            }
            set
            {
                base.City = value;

                this.NotifyOfPropertyChange(() => this.FullAddress);
            }
        }

        public override string Country
        {
            get
            {
                return base.Country;
            }
            set
            {
                base.Country = value;

                this.NotifyOfPropertyChange(() => this.FullAddress);
            }
        }

        public override string Province
        {
            get
            {
                return base.Province;
            }
            set
            {
                base.Province = value;

                this.NotifyOfPropertyChange(() => this.FullAddress);
            }
        }

        public override ushort Zip
        {
            get
            {
                return base.Zip;
            }
            set
            {
                base.Zip = value;

                this.NotifyOfPropertyChange(() => this.FullAddress);
            }
        }

        public override string Street
        {
            get
            {
                return base.Street;
            }
            set
            {
                base.Street = value;

                this.NotifyOfPropertyChange(() => this.FullAddress);
            }
        }

        public override string Number
        {
            get
            {
                return base.Number;
            }
            set
            {
                base.Number = value;

                this.NotifyOfPropertyChange(() => this.FullAddress);
            }
        }

        #endregion

        #region added

        private double obligationAmount = double.NaN;
        public double ObligationAmount
        {
            get { return obligationAmount; }
            set
            {
                if (this.obligationAmount != value)
                {
                    this.obligationAmount = value;
                    this.NotifyOfPropertyChange(() => this.ObligationAmount);
                    this.NotifyOfPropertyChange(() => this.ObligationState);
                }
            }
        }

        public Obligation ObligationState
        {
            get
            {
                if (this.ObligationAmount < 0) return Obligation.Creditor;
                if (this.ObligationAmount > 0) return Obligation.Debtor;
                return Obligation.Null;
            }
        }

        public string FullAddress
        {
            get
            {
                return
                    this.Street +
                    " " + this.Number +
                    ", " + this.Zip +
                    " " + this.City +
                    " (" + this.Province + ")" +
                    " - " + this.Country;
            }
        }

        #endregion

        #region overrides

        public new string DisplayName
        {
            get { return this.Name; }
        }

        #endregion

        #endregion

        #region methods

        #region message handlers

        public void Handle(BillsListChangedMessage message)
        {
            double newOblAmount = 0;

            foreach (var bill in message.Bills)
            {
                if (bill.SupplierID == this.ID & !bill.PaymentDate.HasValue)
                    newOblAmount += -bill.Amount;
            }

            this.ObligationAmount = newOblAmount;
        }

        public void Handle(BillAddedMessage message)
        {
            if (this.ID == message.AddedBill.SupplierID)
                if (!message.AddedBill.PaymentDate.HasValue)
                    this.ObligationAmount += -message.AddedBill.Amount;
        }

        public void Handle(BillDeletedMessage message)
        {
            if (this.ID == message.DeletedBill.SupplierID)
                if (!message.DeletedBill.PaymentDate.HasValue)
                    this.ObligationAmount += message.DeletedBill.Amount;
        }

        public void Handle(BillEditedMessage message)
        {
            bool supplierChanged = message.NewBillVersion.SupplierID != message.OldBillVersion.SupplierID;

            if (supplierChanged)
            {
                if (this.ID == message.OldBillVersion.SupplierID)
                    if (!message.OldBillVersion.PaymentDate.HasValue)
                        this.ObligationAmount += message.OldBillVersion.Amount;

                if (this.ID == message.NewBillVersion.SupplierID)
                    if (!message.NewBillVersion.PaymentDate.HasValue)
                        this.ObligationAmount += -message.NewBillVersion.Amount;
            }
            else
            {
                if (this.ID == message.NewBillVersion.SupplierID)
                {
                    if (!message.OldBillVersion.PaymentDate.HasValue)
                        this.ObligationAmount += message.OldBillVersion.Amount;

                    if (!message.NewBillVersion.PaymentDate.HasValue)
                        this.ObligationAmount += -message.NewBillVersion.Amount;
                }
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
                        () =>
                        {
                            this.TryClose();
                            this.eventAggregator.Publish(new EditSupplierRequestMessage(this.ExposedSupplier));
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