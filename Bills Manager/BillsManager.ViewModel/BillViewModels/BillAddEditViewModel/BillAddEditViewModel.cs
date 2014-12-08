using BillsManager.Localization;
using BillsManager.Localization.Attributes;
using BillsManager.Models;
using BillsManager.ViewModels.Commanding;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class BillAddEditViewModel :
        BillViewModel/*,
        IHandle<SuppliersListChangedMessage>,
        IHandle<SupplierAddedMessage>*/
    {
        #region fields

        protected readonly IWindowManager windowManager;
        protected readonly IEventAggregator globalEventAggregator;

        #endregion

        #region ctor

        public BillAddEditViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            IEnumerable<Supplier> availableSuppliers,
            Bill bill)
        {
            if (bill == null)
                throw new ArgumentNullException("bill cannot be null"); // TODO: is check needed? any other point to do it?

            this.exposedBill = bill;

            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;

            this.globalEventAggregator.Subscribe(this);

            this.AvailableSuppliers = availableSuppliers; // IDEA: inject SuppliersViewModel? overkill?
            //this.HasChanges = false; // TODO: check if mandatory
            this.SelectedSupplier = this.AvailableSuppliers.SingleOrDefault(s => s.ID == this.SupplierID);

            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
                };
        }

        #endregion

        #region properties

        #region add/edit support

        private IEnumerable<Supplier> availableSuppliers;
        public IEnumerable<Supplier> AvailableSuppliers
        {
            get { return this.availableSuppliers; }
            protected set
            {
                if (this.availableSuppliers == value) return;

                this.availableSuppliers = value;
                this.NotifyOfPropertyChange(() => this.AvailableSuppliers);
            }
        }

        private Supplier selectedSupplier; // TODO: remove and make the get to point to the bill?
        [LocalizedRequired(ErrorMessageKey = "SupplierRequired")]
        public Supplier SelectedSupplier
        {
            get { return this.selectedSupplier; }
            set
            {
                if (this.selectedSupplier == value) return;

                this.selectedSupplier = value;
                this.NotifyOfPropertyChange(() => this.SelectedSupplier);
                this.NotifyOfPropertyChange(() => this.IsValid);
                if (value != null)
                    this.SupplierID = this.SelectedSupplier.ID;
            }
        }

        public DateTime DisplayDateEndForReleaseDate
        {
            get { return DateTime.Today; }
        }

        #endregion

        #region wrapped from bill

        //[LocalizedRequired(ErrorMessageKey = "You must specify a supplier.")] // TODO: language
        //[Range(0, uint.MaxValue, ErrorMessage = "Chosen supplier ID is out of range.")] // TODO: language
        public override uint SupplierID
        {
            get { return base.SupplierID; }
            set
            {
                if (this.SupplierID != value)
                {
                    base.SupplierID = value;
                    this.NotifyOfPropertyChange(() => this.SupplierID);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;

                    //this.SelectedSupplier = this.GetSupplier(this.SupplierID);
                }
            }
        }

        /* TODO: registration date should be read only
         *       this means no validation and no override here */
        public override DateTime RegistrationDate
        {
            get { return base.RegistrationDate; }
            set
            {
                if (this.RegistrationDate == value) return;

                base.RegistrationDate = value;
                this.NotifyOfPropertyChange(() => this.RegistrationDate);
                this.NotifyOfPropertyChange(() => this.IsValid);
                this.HasChanges = true;
            }
        }

        [LocalizedRequired(ErrorMessageKey = "DateRequired")]
        public override DateTime DueDate
        {
            get { return base.DueDate; }
            set
            {
                if (this.DueDate != value)
                {
                    base.DueDate = value;
                    this.NotifyOfPropertyChange(() => this.DueDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [LocalizedRequired(ErrorMessageKey = "ReleaseDateRequired")]
        public override DateTime ReleaseDate
        {
            get { return base.ReleaseDate; }
            set
            {
                if (this.ReleaseDate != value)
                {
                    base.ReleaseDate = value;
                    this.NotifyOfPropertyChange(() => this.ReleaseDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        public override DateTime? PaymentDate
        {
            get { return base.PaymentDate; }
            set
            {
                if (this.PaymentDate != value)
                {
                    base.PaymentDate = value;
                    this.NotifyOfPropertyChange(() => this.PaymentDate);
                    this.NotifyOfPropertyChange(() => this.IsPaid);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [LocalizedRequired(ErrorMessageKey = "AmountRequired")]
        public override Double Amount
        {
            get { return base.Amount; }
            set
            {
                //if (this.Amount != value)
                //{
                base.Amount = value;
                this.NotifyOfPropertyChange(() => this.Amount);
                this.NotifyOfPropertyChange(() => this.IsValid);
                this.HasChanges = true;
                //}
            }
        }

        public override double Agio
        {
            get { return base.Agio; }
            set
            {
                base.Agio = value;
                this.NotifyOfPropertyChange(() => this.Agio);
                this.HasChanges = true;
            }
        }

        public override double AdditionalCosts
        {
            get { return base.AdditionalCosts; }
            set
            {
                base.AdditionalCosts = value;
                this.NotifyOfPropertyChange(() => this.AdditionalCosts);
                this.HasChanges = true;
            }
        }

        //[Required(ErrorMessage = "You must specify a code.")] // TODO: language
        public override string Code
        {
            get { return base.Code; }
            set
            {
                if (this.Code != value)
                {
                    base.Code = value;
                    this.NotifyOfPropertyChange(() => this.Code);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [LocalizedStringLength(200, ErrorMessageFormatKey = "CannotExceedChars_format")]
        public override string Notes
        {
            get { return base.Notes; }
            set
            {
                if (this.Notes != value)
                {
                    base.Notes = value;
                    this.NotifyOfPropertyChange(() => this.Notes);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        #endregion

        #region added

        public bool IsPaid
        {
            get { return this.PaymentDate.HasValue; }
            set
            {
                if (this.IsPaid != value)
                {
                    if (value) this.PaymentDate = DateTime.Today;
                    else this.PaymentDate = null;
                }
            }
        }

        #endregion

        #region overrides

        public new string DisplayName
        {
            get
            {
                return this.IsInEditMode
                    ? (TranslationManager.Instance.Translate("EditBill").ToString() + (this.IsInEditMode & this.HasChanges ? " [*]" : string.Empty))
                    : TranslationManager.Instance.Translate("NewBill").ToString();
            }
        }

        #endregion

        #endregion

        #region methods

        //private IEnumerable<Supplier> GetAvailableSuppliers()
        //{
        //    IEnumerable<Supplier> suppliers = null;

        //    this.globalEventAggregator.Publish(new AvailableSuppliersRequest(s => suppliers = s));

        //    return suppliers;
        //}

        private void ConfirmAddEditAndClose()
        {
            if (this.IsInEditMode)
                this.EndEdit();

            this.TryClose(true);
        }

        private void CancelAddEditAndClose()
        {
            // TODO: optimize
            if (this.HasChanges)
            {
                var question = new DialogViewModel(
                    this.IsInEditMode ?
                    TranslationManager.Instance.Translate("CancelEdit").ToString() :
                    TranslationManager.Instance.Translate("CancelAdd").ToString(),
                    TranslationManager.Instance.Translate("DiscardChangesQuestion").ToString(),
                    new[]
                    {
                        new DialogResponse(ResponseType.Yes, TranslationManager.Instance.Translate("Yes").ToString()),
                        new DialogResponse(ResponseType.No, TranslationManager.Instance.Translate("No").ToString())
                    });

                this.windowManager.ShowDialog(question);

                if (question.FinalResponse == ResponseType.Yes)
                {
                    if (this.IsInEditMode)
                        this.CancelEdit();

                    this.TryClose(false);
                }
            }
            else
            {
                if (this.IsInEditMode)
                    this.CancelEdit();

                this.TryClose(false);
            }
        }

        #region message handlers

        //public void Handle(SuppliersListChangedMessage message)
        //{
        //    this.AvailableSuppliers = message.Suppliers;
        //}

        //public void Handle(SupplierAddedMessage message)
        //{
        //    // available suppliers references to the same instance of suppliersvm
        //    this.NotifyOfPropertyChange(() => this.AvailableSuppliers);
        //}

        #endregion

        #endregion

        #region commands

        protected RelayCommand confirmAddEditAndCloseCommand;
        public RelayCommand ConfirmAddEditAndCloseCommand
        {
            get
            {
                if (this.confirmAddEditAndCloseCommand == null)
                    this.confirmAddEditAndCloseCommand = new RelayCommand(
                    () => this.ConfirmAddEditAndClose(),
                    () => this.IsValid);

                return this.confirmAddEditAndCloseCommand;
            }
        }

        protected RelayCommand cancelAddEditAndCloseCommand;
        public RelayCommand CancelAddEditAndCloseCommand
        {
            get
            {
                if (this.cancelAddEditAndCloseCommand == null)
                    this.cancelAddEditAndCloseCommand = new RelayCommand(
                        () => this.CancelAddEditAndClose());

                return this.cancelAddEditAndCloseCommand;
            }
        }

        #endregion
    }
}