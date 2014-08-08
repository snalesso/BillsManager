using BillsManager.Localization;
using BillsManager.Localization.Attributes;
using BillsManager.Models;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Linq;

namespace BillsManager.ViewModels
{
    // IDEA: make properties read only?
    public partial class BillDetailsViewModel :
        BillViewModel,
        IHandle<SupplierEditedMessage>
    {
        #region fields

        protected readonly IWindowManager windowManager;
        protected readonly IEventAggregator globalEventAggregator;

        #endregion

        #region ctor

        public BillDetailsViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            Bill bill)
        {
            if (bill == null)
                throw new ArgumentNullException("bill cannot be null");

            this.exposedBill = bill;

            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;

            this.globalEventAggregator.Subscribe(this);

            //this.SupplierName = this.GetSupplierName(this.SupplierID);

            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        this.globalEventAggregator.Unsubscribe(this);
                    }
                };
        }

        #endregion

        #region properties

        #region wrapped from bill

        public override DateTime DueDate
        {
            get { return base.DueDate; }
        }

        public override DateTime? PaymentDate
        {
            get { return base.PaymentDate; }
        }

        public override double Amount
        {
            get { return base.Amount; }
        }

        public override string Code
        {
            get { return base.Code; }
        }

        public override string Notes
        {
            get { return base.Notes; }
        }

        #endregion

        #region added

        public string SupplierName
        {
            get { return this.GetSupplierName(); }
        }

        public bool IsPaid
        {
            get { return this.PaymentDate.HasValue; }
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
                    if (timeleft.TotalDays == 0) return TranslationManager.Instance.Translate("OverduesToday_toBill").ToString();
                    if (timeleft.TotalDays == 1) return TranslationManager.Instance.Translate("OverduesTomorrow_toBill").ToString();
                    return string.Format(TranslationManager.Instance.Translate("OverduesInXDays_toBill_format").ToString(), timeleft.TotalDays);
                }
                else
                {
                    if (timeleft.TotalDays == -1)
                        return TranslationManager.Instance.Translate("OverdueYesterday_toBill").ToString();
                    return string.Format(TranslationManager.Instance.Translate("OverdueXDaysAgo_toBill_format").ToString(), timeleft.TotalDays * -1);
                }
            }
        }

        public DueAlert DueAlert
        {
            get
            {
                if (this.IsPaid)
                    return DueAlert.None;

                var remDays = (this.DueDate - DateTime.Today).TotalDays;

                if (remDays >= 15)
                    return DueAlert.None;
                else if (remDays >= 7)
                    return DueAlert.Low;
                else if (remDays >= 3)
                    return DueAlert.Medium;
                else if (remDays >= 0)
                    return DueAlert.High;
                else
                    return DueAlert.Critical;
            }
        }

        public string DueLevelString
        {
            get
            {
                return
                    /*TranslationManager.Instance.Translate(*/
                    typeof(ViewModels.DueAlert)
                    .GetMember(this.DueAlert.ToString())[0]
                    .GetAttributes<LocalizedDisplayNameAttribute>(true).FirstOrDefault().DisplayName
                    /*).ToString()*/;
            }
        }

        #endregion

        #region override

        public new string DisplayName
        {
            get { return this.Code + " - " + this.SupplierName; }
        }

        #endregion

        #endregion

        #region methods

        private string GetSupplierName()
        {
            // TODO: move supplier logic to BillsViewModel (same for supp's obligation amount)
            string supp = string.Empty;
            this.globalEventAggregator.Publish(new SupplierNameRequest(this.SupplierID, s => supp = s));
            return supp;
        }

        private void SwitchToEdit()
        {
            this.TryClose();
            this.globalEventAggregator.Publish(new EditBillOrder(this.ExposedBill));
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