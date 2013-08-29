using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BillViewModel : Screen
    {
        #region fields

        //protected readonly IDialogService dialogService;
        private readonly IWindowManager windowManager;
        protected readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public BillViewModel(
            Bill bill,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator)
        {
            if (bill == null)
                throw new ArgumentNullException("The bill cannot be null");

            this.ExposedBill = bill;

            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        protected Bill exposedBill;
        public Bill ExposedBill
        {
            get { return this.exposedBill; }
            protected set
            {
                if (this.exposedBill != value)
                {
                    this.exposedBill = value;
                    this.NotifyOfPropertyChange(() => this.ExposedBill);
                }
            }
        }

        protected IEnumerable<Supplier> availableSuppliers;
        public IEnumerable<Supplier> AvailableSuppliers
        {
            get { return this.availableSuppliers; }
            protected set
            {
                if (this.availableSuppliers != value)
                {
                    this.availableSuppliers = value;
                    this.NotifyOfPropertyChange(() => this.AvailableSuppliers);
                    this.SelectedSupplier = null;
                }
            }
        }

        protected Supplier selectedSupplier;
        [Required(ErrorMessage = "You must select a supplier.")]
        public Supplier SelectedSupplier
        {
            get { return this.selectedSupplier; }
            set
            {
                if (this.selectedSupplier != value)
                {
                    this.selectedSupplier = value;
                    this.NotifyOfPropertyChange(() => this.SelectedSupplier);
                    this.Supplier = this.SelectedSupplier.Name;
                }
            }
        }

        #region wrapped from bill

        public DateTime RegistrationDate
        {
            get { return this.ExposedBill.RegistrationDate; }
            set
            {
                if (this.RegistrationDate != value)
                {
                    this.ExposedBill.RegistrationDate = value;
                    this.NotifyOfPropertyChange(() => this.RegistrationDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a due date.")]
        public DateTime DueDate
        {
            get { return this.ExposedBill.DueDate; }
            set
            {
                if (this.DueDate != value)
                {
                    this.ExposedBill.DueDate = value;
                    this.NotifyOfPropertyChange(() => this.DueDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                    this.NotifyOfPropertyChange(() => this.IsDued);
                    this.NotifyOfPropertyChange(() => this.DuesIn);
                }
            }
        }

        public DateTime? PaymentDate
        {
            get { return this.ExposedBill.PaymentDate; }
            set
            {
                if (this.PaymentDate != value)
                {
                    this.ExposedBill.PaymentDate = value;
                    this.NotifyOfPropertyChange(() => this.PaymentDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                    this.NotifyOfPropertyChange(() => this.IsPaid);
                    this.NotifyOfPropertyChange(() => this.IsNotPaid);
                    this.NotifyOfPropertyChange(() => this.IsDued);
                    this.NotifyOfPropertyChange(() => this.DuesIn);
                }
            }
        }

        [Required(ErrorMessage = "You must specify a release date.")]
        public DateTime ReleaseDate
        {
            get { return this.ExposedBill.ReleaseDate; }
            set
            {
                if (this.ReleaseDate != value)
                {
                    this.ExposedBill.ReleaseDate = value;
                    this.NotifyOfPropertyChange(() => this.ReleaseDate);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify an amount.")]
        public Double Amount
        {
            get { return this.ExposedBill.Amount; }
            set
            {
                if (this.Amount != value)
                {
                    this.ExposedBill.Amount = value;
                    this.NotifyOfPropertyChange(() => this.Amount);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a supplier.")]
        public string Supplier
        {
            get { return this.ExposedBill.Supplier; }
            set
            {
                if (this.Supplier != value)
                {
                    this.ExposedBill.Supplier = value;
                    this.NotifyOfPropertyChange(() => this.Supplier);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [StringLength(40, ErrorMessage = "You cannot exceed 40 characters.")]
        public string Notes
        {
            get { return this.ExposedBill.Notes; }
            set
            {
                if (this.Notes != value)
                {
                    this.ExposedBill.Notes = value;
                    this.NotifyOfPropertyChange(() => this.Notes);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a code.")]
        public string Code
        {
            get { return this.ExposedBill.Code; }
            set
            {
                if (this.Code != value)
                {
                    this.ExposedBill.Code = value;
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
                    // changing only PaymentData will Refresh IsPaid and similars
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
                    if (timeleft.TotalDays == 0) return "Overdues today";
                    if (timeleft.TotalDays == 1) return "Overdues tomorrow"; // Toyota? :D
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

        #region overrides

        public new string DisplayName
        {
            get { return this.IsInEditMode ? ("Edit bill" + (this.IsInEditMode & this.HasChanges ? " [*]" : string.Empty)) : "New bill"; }
        }

        #endregion

        #endregion

        #region methods

        // TODO: find a better way (should be solved creating different VMs for Item and AddEdit)
        public void SetupForAddEdit()
        {
            this.AvailableSuppliers = this.GetAvailableSuppliers();

            if (this.IsInEditMode)
            {
                this.SelectedSupplier = this.AvailableSuppliers.Single(s => s.Name == this.Supplier);

                if (this.SelectedSupplier == null)
                    throw new ArgumentNullException("Cannot find " + this.Supplier + " among available suppliers.");
            }
        }

        protected void CleanAfterAddEdit()
        {
            this.selectedSupplier = null;
            this.availableSuppliers = null;
        }

        #region overrides

        public override void CanClose(Action<bool> callback)
        {
            // TDOD: make it for both close window button or noone
            if (this.IsInEditMode)
            {
                var question = new DialogViewModel(
                    "Closing",
                    "Are you sure you want to discard all the changes?",
                    new[]
                    {
                        new DialogResponse(ResponseType.Yes, 3),
                        new DialogResponse(ResponseType.No)
                    });

                this.windowManager.ShowDialog(question);

                //if (this.dialogService.ShowYesNoQuestion("Closing", "Are you sure you want to discard all the changes?"))
                if (question.Response == ResponseType.Yes)
                {
                    this.CancelEdit();
                    this.availableSuppliers = null;
                    this.selectedSupplier = null;
                    callback(true);
                }
                else callback(false);
            }
            else callback(true);
        }

        #endregion

        #endregion

        #region commands

        protected RelayCommand confirmAddEditAndCloseCommand;
        public RelayCommand ConfirmAddEditAndCloseCommand
        {
            get
            {
                if (this.confirmAddEditAndCloseCommand == null) this.confirmAddEditAndCloseCommand = new RelayCommand(
                    () =>
                    {
                        if (this.IsInEditMode)
                        {
                            this.EndEdit();
                        }

                        this.CleanAfterAddEdit();

                        this.TryClose(true);
                    },
                    () => this.IsValid);

                return this.confirmAddEditAndCloseCommand;
            }
        }

        protected RelayCommand cancelAddEditAndCloseCommand;
        public RelayCommand CancelAddEditAndCloseCommand
        {
            get
            {
                if (this.cancelAddEditAndCloseCommand == null) this.cancelAddEditAndCloseCommand = new RelayCommand(
                    () =>
                    {
                        if (this.IsInEditMode)
                        {
                            this.CancelEdit();
                        }

                        this.CleanAfterAddEdit();

                        this.TryClose(false);
                    });

                return this.cancelAddEditAndCloseCommand;
            }
        }

        #endregion
    }
}