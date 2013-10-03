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
    public partial class BillAddEditViewModel :
        BillViewModel,
        IHandle<SuppliersListChangedMessage>
    {
        #region fields

        protected readonly IWindowManager windowManager;
        protected readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public BillAddEditViewModel(
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

            this.AvailableSuppliers = this.GetAvailableSuppliers();
        }

        #endregion

        #region properties

        #region view support

        // TODO: remove this proprerty when removed close window button
        private bool isClosing;
        private bool IsClosing
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
        
        #endregion

        #region add/edit support

        private IEnumerable<Supplier> availableSuppliers;
        public IEnumerable<Supplier> AvailableSuppliers
        {
            get { return this.availableSuppliers; }
            protected set
            {
                if (this.availableSuppliers != value)
                {
                    this.availableSuppliers = value;
                    this.NotifyOfPropertyChange(() => this.AvailableSuppliers);

                    var selSupp = this.AvailableSuppliers.Single(s => s.ID == this.SupplierID);
                    this.selectedSupplier = selSupp != null ? selSupp : this.AvailableSuppliers.FirstOrDefault();
                }
            }
        }

        private Supplier selectedSupplier;
        [Required(ErrorMessage = "You must select a supplier.")] // TODO: language
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
        
        #endregion

        #region wrapped from bill

        public override DateTime RegistrationDate
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

        [Required(ErrorMessage = "You must specify a due date.")] // TODO: language
        public override DateTime DueDate
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
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a release date.")] // TODO: language
        public override DateTime ReleaseDate
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

        [Required(ErrorMessage = "You must specify an amount.")] // TODO: language
        public override Double Amount
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

        [Required(ErrorMessage = "You must specify a supplier.")] // TODO: language
        [Range(0, uint.MaxValue, ErrorMessage = "Chosen supplier ID is out of range.")] // TODO: language
        public override uint SupplierID
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

        [StringLength(100, ErrorMessage = "You cannot exceed 100 characters.")] // TODO: language
        public override string Notes
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

        [Required(ErrorMessage = "You must specify a code.")] // TODO: language
        public override string Code
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

        private IEnumerable<Supplier> GetAvailableSuppliers()
        {
            IEnumerable<Supplier> suppliers = null;

            this.eventAggregator.Publish(new AskForAvailableSuppliersMessage(s => suppliers = s));

            return suppliers;
        }

        #region overrides

        public override void CanClose(Action<bool> callback)
        {
            callback(this.IsClosing);
        }

        #endregion

        #region message handlers

        public void Handle(SuppliersListChangedMessage message)
        {
            this.AvailableSuppliers = message.Suppliers;
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand addNewSupplierCommand;
        public RelayCommand AddNewSupplierCommand
        {
            get
            {
                if (this.addNewSupplierCommand == null)
                    this.addNewSupplierCommand = new RelayCommand(
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
                if (this.cancelAddEditAndCloseCommand == null)
                    this.cancelAddEditAndCloseCommand = new RelayCommand(
                        () =>
                        { // TODO: optimize
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

                                this.windowManager.ShowDialog(
                                    question,
                                    settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

                                if (question.Response == ResponseType.Yes)
                                {
                                    if (this.IsInEditMode)
                                        this.CancelEdit();

                                    this.IsClosing = true;
                                    this.TryClose(false);
                                }
                            }
                            else
                            {
                                if (this.IsInEditMode)
                                    this.CancelEdit();

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