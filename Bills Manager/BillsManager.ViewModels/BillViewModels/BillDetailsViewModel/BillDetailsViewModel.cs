using BillsManager.Localization;
using BillsManager.Localization.Attributes;
using BillsManager.Models;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.Linq;

namespace BillsManager.ViewModels
{
    [DebuggerDisplay("Code = {Code}")]
    public partial class BillDetailsViewModel :
        BillViewModel,
        IHandle<EditedMessage<Supplier>>
    {
        #region fields

        protected readonly IWindowManager _windowManager;
        protected readonly IEventAggregator _globalEventAggregator;

        #endregion

        #region ctor

        protected BillDetailsViewModel() { }

        public BillDetailsViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            Bill bill)
        {
            // TODO: review
            //if (bill == null)
            //    throw new ArgumentNullException("bill cannot be null");

            this.exposedBill = bill;

            this._windowManager = windowManager;
            this._globalEventAggregator = globalEventAggregator;

            this._globalEventAggregator.Subscribe(this);

            //this.SupplierName = this.GetSupplierName(this.SupplierID);

            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        this._globalEventAggregator.Unsubscribe(this);
                    }
                };
        }

        #endregion

        #region properties

        #region wrapped from bill

        public override DateTime RegistrationDate
        {
            get { return base.RegistrationDate; }
        }

        public override DateTime DueDate
        {
            get { return base.DueDate; }
        }

        public override DateTime ReleaseDate
        {
            get { return base.ReleaseDate; }
        }

        public override DateTime? PaymentDate
        {
            get { return base.PaymentDate; }
        }

        public override double Amount
        {
            get { return base.Amount; }
        }

        public override double Agio
        {
            get { return base.Agio; }
        }

        public override double AdditionalCosts
        {
            get { return base.AdditionalCosts; }
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

                if /*(remDays >= 15)
                    return DueAlert.None;
                else if*/ (remDays >= 7)
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
                    typeof(DueAlert)
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
            set { }
        }

        #endregion

        #endregion

        #region methods

        private string GetSupplierName()
        {
            // TODO: move supplier logic to BillsViewModel (same for supp's obligation amount)
            string supp = string.Empty;
            this._globalEventAggregator.PublishOnUIThread(new SupplierNameRequest(this.SupplierID, s => supp = s));
            return supp;
        }

        private void SwitchToEdit()
        {
            this.TryClose();
            this._globalEventAggregator.PublishOnUIThread(new EditBillOrder(this.ExposedBill));
        }

        #region message handlers

        public void Handle(EditedMessage<Supplier> message)
        {
            if (this.SupplierID == message.OldItem.ID)
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
                return this.switchToEditCommand ?? (this.switchToEditCommand =
                    new RelayCommand(
                        () => this.SwitchToEdit()));
            }
        }

        private RelayCommand closeDetailsViewCommand;
        public RelayCommand CloseDetailsViewCommand
        {
            get
            {
                return this.closeDetailsViewCommand ?? (this.closeDetailsViewCommand =
                    new RelayCommand(
                        () => this.TryClose()));
            }
        }

        #endregion
    }
}