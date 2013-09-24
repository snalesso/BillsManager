using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BillViewModel :
        Screen,
        IHandle<SuppliersListChangedMessage>
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
                throw new ArgumentNullException("bill cannot be null");

            this.exposedBill = bill;

            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);

            //this.Supplier = this.GetSupplier(this.SupplierID);
        }

        #endregion

        #region properties

        // TODO: remove this proprerty when removed close window button
        private bool isClosing;
        public bool IsClosing
        {
            get { return this.isClosing; }
            set
            {
                if (this.isClosing != value)
                {
                    this.isClosing = value;
                }
            }
        }

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
                    this.SupplierID = this.SelectedSupplier.ID;
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
        [Range(0, uint.MaxValue, ErrorMessage = "Chosen supplier ID is out of range.")]
        public uint SupplierID
        {
            get { return this.ExposedBill.SupplierID; }
            set
            {
                if (this.SupplierID != value)
                {
                    this.ExposedBill.SupplierID = value;
                    this.NotifyOfPropertyChange(() => this.SupplierID);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;

                    //this.SelectedSupplier = this.GetSupplier(this.SupplierID);
                }
            }
        }

        [StringLength(100, ErrorMessage = "You cannot exceed 100 characters.")]
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

        //private Supplier GetSupplier(uint supplierID)
        //{
        //    Supplier supp = null;
        //    this.eventAggregator.Publish(new AskForSupplierMessage(supplierID, s => supp = s));

        //    return supp;
        //}

        // TODO: find a better way (should be solved creating different VMs for Item and AddEdit)
        public void SetupForAddEdit()
        {
            this.AvailableSuppliers = this.GetAvailableSuppliers();

            if (this.IsInEditMode)
            {
                this.SelectedSupplier = this.AvailableSuppliers.Single(s => s.ID == this.SupplierID);

                if (this.SelectedSupplier == null)
                    throw new ArgumentNullException("Cannot find " + this.SupplierID + " among available suppliers.");
            }
        }

        protected void CleanSuppliersProperties()
        {
            this.selectedSupplier = null;
            this.availableSuppliers = null;
        }

        public void Handle(SuppliersListChangedMessage message)
        {
            this.AvailableSuppliers = message.Suppliers;
        }

        #region overrides

        public override void CanClose(Action<bool> callback)
        {
            callback(this.IsClosing);
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand addNewSupplierCommand;
        public RelayCommand AddNewSupplierCommand
        {
            get
            {
                if (this.addNewSupplierCommand == null) this.addNewSupplierCommand = new RelayCommand(
                    () =>
                    {
                        this.eventAggregator.Publish(new AddNewSupplierRequestMessage());
                    });

                return this.addNewSupplierCommand;
            }
        }

        protected RelayCommand confirmAddEditAndCloseCommand;
        public RelayCommand ConfirmAddEditAndCloseCommand
        {
            get
            {
                if (this.confirmAddEditAndCloseCommand == null) this.confirmAddEditAndCloseCommand = new RelayCommand(
                    () =>
                    {
                        if (this.IsInEditMode)
                            this.EndEdit();

                        this.CleanSuppliersProperties();

                        this.IsClosing = true;
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
                        if (this.HasChanges)
                        {
                            var question = new DialogViewModel(
                                   "Canceling " + (this.IsInEditMode ? "edit" : "add"),
                                   "Are you sure you want to discard all the changes?",
                                   new[]
                            {
                                new DialogResponse(ResponseType.Yes),
                                new DialogResponse(ResponseType.No)
                            });

                            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

                            if (question.Response == ResponseType.Yes)
                            {
                                if (this.IsInEditMode)
                                    this.CancelEdit();

                                this.CleanSuppliersProperties();

                                this.IsClosing = true;
                                this.TryClose(false);
                            }
                        }
                        else
                        {
                            if (this.IsInEditMode)
                                this.CancelEdit();

                            this.CleanSuppliersProperties();

                            this.IsClosing = true;
                            this.TryClose(false);
                        }
                    });

                return this.cancelAddEditAndCloseCommand;
            }
        }

        #endregion
    }
}