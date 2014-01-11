using BillsManager.Models;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class BillAddEditViewModel :
        BillViewModel,
        IHandle<SuppliersListChangedMessage>
    {
        #region fields

        protected readonly IWindowManager windowManager;
        protected readonly IEventAggregator dbEventAggregator;

        #endregion

        #region ctor

        public BillAddEditViewModel(
            IWindowManager windowManager,
            IEventAggregator dbEventAggregator,
            Bill bill)
        {
            if (bill == null)
                throw new ArgumentNullException("bill cannot be null");

            this.exposedBill = bill;

            this.windowManager = windowManager;
            this.dbEventAggregator = dbEventAggregator;

            this.dbEventAggregator.Subscribe(this);

            this.AvailableSuppliers = this.GetAvailableSuppliers(); /* TODO: make injected? dependencies should be provided
                                                                     * but the handler is needed anyway */
            this.HasChanges = false; // TODO: check if mandatory

            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        this.dbEventAggregator.Unsubscribe(this);
                    }
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

                var selSupp = this.AvailableSuppliers.SingleOrDefault(s => s.ID == this.SupplierID);
                this.selectedSupplier = selSupp != null ? selSupp : this.AvailableSuppliers.FirstOrDefault();
            }
        }

        private Supplier selectedSupplier; // TODO: remove and make the get to point to the bill?
        [Required(ErrorMessage = "You must select a supplier.")] // TODO: language
        public Supplier SelectedSupplier
        {
            get
            {
                return this.selectedSupplier;
            }
            set
            {
                if (this.selectedSupplier != value)
                {
                    this.selectedSupplier = value;
                    this.NotifyOfPropertyChange(() => this.SelectedSupplier);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    if (value != null)
                        this.SupplierID = this.SelectedSupplier.ID;
                }
            }
        }

        #endregion

        #region wrapped from bill

        public override DateTime RegistrationDate
        {
            get { return base.RegistrationDate; }
            set
            {
                if (this.RegistrationDate != value)
                {
                    base.RegistrationDate = value;
                    this.NotifyOfPropertyChange(() => this.RegistrationDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a due date.")] // TODO: language
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

        [Required(ErrorMessage = "You must specify a release date.")] // TODO: language
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

        [Required(ErrorMessage = "You must specify an amount.")] // TODO: language
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

        [Required(ErrorMessage = "You must specify a supplier.")] // TODO: language
        [Range(0, uint.MaxValue, ErrorMessage = "Chosen supplier ID is out of range.")] // TODO: language
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

        [StringLength(200, ErrorMessage = "You cannot exceed 200 characters.")] // TODO: language
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
            get { return this.IsInEditMode ? ("Edit bill" + (this.IsInEditMode & this.HasChanges ? " [*]" : string.Empty)) : "New bill"; } // TODO: language
        }

        #endregion

        #endregion

        #region methods

        private IEnumerable<Supplier> GetAvailableSuppliers()
        {
            IEnumerable<Supplier> suppliers = null;

            this.dbEventAggregator.Publish(new AvailableSuppliersRequestMessage(s => suppliers = s));

            return suppliers;
        }

        private void AddNewSupplier()
        {
            this.dbEventAggregator.Publish(new AddNewSupplierOrder());
        }

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
                       "Cancel " + (this.IsInEditMode ? "edit" : "add"), // TODO: language
                       "Are you sure you want to discard all the changes?",
                       new[]
                       {
                           new DialogResponse(ResponseType.Yes),
                           new DialogResponse(ResponseType.No)
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

        public void Handle(SuppliersListChangedMessage message)
        {
            this.AvailableSuppliers = message.Suppliers;
        }

        #endregion

        #endregion

        #region commands

        // URGENT: update datepicker style
        private RelayCommand addNewSupplierCommand;
        public RelayCommand AddNewSupplierCommand
        {
            get
            {
                if (this.addNewSupplierCommand == null)
                    this.addNewSupplierCommand = new RelayCommand(
                        () => this.AddNewSupplier());

                return this.addNewSupplierCommand;
            }
        }

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