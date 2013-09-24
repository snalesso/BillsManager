using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SupplierViewModel :
        Screen,
        IHandle<BillsListChangedMessage>,
        IHandle<BillAddedMessage>,
        IHandle<BillDeletedMessage>,
        IHandle<BillEditedMessage>
    {
        #region fields

        //private readonly IDialogService dialogService;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public SupplierViewModel(
            Supplier supplier,
            //IDialogService dialogService,
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            if (supplier == null)
                throw new ArgumentNullException("supplier cannot be null.");

            this.exposedSupplier = supplier;
            //this.dialogService = dialogService;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        private Supplier exposedSupplier;
        public Supplier ExposedSupplier
        {
            get { return this.exposedSupplier; }
            protected set
            {
                if (this.exposedSupplier != value)
                {
                    this.exposedSupplier = value;
                    this.NotifyOfPropertyChange(() => this.ExposedSupplier);
                }
            }
        }

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

        #region wrapped from supplier

        public uint ID
        {
            get { return this.ExposedSupplier.ID; }
        }

        [Required(ErrorMessage = "You must specify a name.")]
        public string Name
        {
            get { return this.ExposedSupplier.Name; }
            set
            {
                if (this.Name != value)
                {
                    this.ExposedSupplier.Name = value;
                    this.NotifyOfPropertyChange(() => this.Name);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        //public Address Address
        //{
        //    get
        //    {
        //        //if (this.address == null) this.address = new Address();
        //        // TODO: review
        //        return this.ExposedSupplier.Address;
        //    }
        //    set
        //    {
        //        if (this.ExposedSupplier.Address != value)
        //        {
        //            this.ExposedSupplier.Address = value;
        //            this.NotifyOfPropertyChange(() => this.Address);
        //        }
        //    }
        //}

        public string eMail
        {
            get { return this.ExposedSupplier.eMail; }
            set
            {
                if (this.eMail != value)
                {
                    this.ExposedSupplier.eMail = value;
                    this.NotifyOfPropertyChange(() => this.eMail);
                    this.HasChanges = true;
                }
            }
        }

        public string Website
        {
            get { return this.ExposedSupplier.Website; }
            set
            {
                if (this.Website != value)
                {
                    this.ExposedSupplier.Website = value;
                    this.NotifyOfPropertyChange(() => this.Website);
                    this.HasChanges = true;
                }
            }
        }

        //public ObservableCollection<Agent> Agents
        //{
        //    get
        //    {
        //        if (this.ExposedSupplier.Agents == null) this.ExposedSupplier.Agents = new ObservableCollection<Agent>();
        //        return this.ExposedSupplier.Agents;
        //    }
        //    set
        //    {
        //        if (this.ExposedSupplier.Agents != value)
        //        {
        //            this.ExposedSupplier.Agents = value;
        //            this.NotifyOfPropertyChange(() => this.Agents);
        //        }
        //    }
        //}

        public string Phone
        {
            get { return this.ExposedSupplier.Phone; }
            set
            {
                if (this.Phone != value)
                {
                    this.ExposedSupplier.Phone = value;
                    this.NotifyOfPropertyChange(() => this.Phone);
                    this.HasChanges = true;
                }
            }
        }

        [StringLength(100, ErrorMessage = "You cannot exceed 100 characters.")]
        public string Notes
        {
            get { return this.ExposedSupplier.Notes; }
            set
            {
                if (this.Notes != value)
                {
                    this.ExposedSupplier.Notes = value;
                    this.NotifyOfPropertyChange(() => this.Notes);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        #region address

        [Required(ErrorMessage = "You must specify a street.")]
        public string Street
        {
            get { return this.ExposedSupplier.Street; }
            set
            {
                if (this.Street != value)
                {
                    this.ExposedSupplier.Street = value;
                    this.NotifyOfPropertyChange(() => this.Street);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a number.")]
        public string Number
        {
            get { return this.ExposedSupplier.Number; }
            set
            {
                if (this.Number != value)
                {
                    this.ExposedSupplier.Number = value;
                    this.NotifyOfPropertyChange(() => this.Number);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a city.")]
        public string City
        {
            get { return this.ExposedSupplier.City; }
            set
            {
                if (this.City != value)
                {
                    this.ExposedSupplier.City = value;
                    this.NotifyOfPropertyChange(() => this.City);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a zip.")]
        [Range(10000, 99999, ErrorMessage = "The zip must be 5 characters long.")]
        public ushort Zip
        {
            get { return this.ExposedSupplier.Zip; }
            set
            {
                if (this.Zip != value)
                {
                    this.ExposedSupplier.Zip = value;
                    this.NotifyOfPropertyChange(() => this.Zip);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a province.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "The province abbreviation must be 2 characters long.")]
        public string Province
        {
            get { return this.ExposedSupplier.Province; }
            set
            {
                if (this.Province != value)
                {
                    this.ExposedSupplier.Province = value;
                    this.NotifyOfPropertyChange(() => this.Province);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        [Required(ErrorMessage = "You must specify a country.")]
        public string Country
        {
            get { return this.ExposedSupplier.Country; }
            set
            {
                if (this.Country != value)
                {
                    this.ExposedSupplier.Country = value;
                    this.NotifyOfPropertyChange(() => this.Country);
                    this.NotifyOfPropertyChange(() => this.IsValid);
                    this.HasChanges = true;
                }
            }
        }

        #endregion

        #endregion

        #region added

        private double obligationAmount = 0;
        public double ObligationAmount
        {
            get
            {
                return obligationAmount;
            }
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

        #endregion

        #region overrides

        public new string DisplayName
        {
            get { return this.IsInEditMode ? ("Edit supplier" + (this.IsInEditMode & this.HasChanges ? " [*]" : string.Empty)) : "New supplier"; }
        }

        #endregion

        #endregion

        #region methods

        #region overrides

        public override void CanClose(Action<bool> callback)
        {
            callback(this.IsClosing);
        }

        #endregion

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

        private RelayCommand confirmAddEditAndCloseCommand;
        public RelayCommand ConfirmAddEditAndCloseCommand
        {
            get
            {
                if (this.confirmAddEditAndCloseCommand == null) this.confirmAddEditAndCloseCommand = new RelayCommand(
                    () =>
                    {
                        if (this.IsInEditMode) this.EndEdit();

                        this.IsClosing = true;
                        this.TryClose(true);
                    },
                    () => this.IsValid);

                return this.confirmAddEditAndCloseCommand;
            }
        }

        private RelayCommand cancelAddEditAndCloseCommand;
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
                                "Are you sure you want to discard all the changes?", // TODO: language
                                new[]
                            {
                                new DialogResponse(ResponseType.Yes),
                                new DialogResponse(ResponseType.No)
                            });

                            this.windowManager.ShowDialog(question, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } });

                            if (question.Response == ResponseType.Yes)
                            {
                                if (this.IsInEditMode)
                                {
                                    this.CancelEdit();
                                }

                                this.IsClosing = true;
                                this.TryClose(false);
                            }
                        }
                        else
                        {
                            if (this.IsInEditMode)
                            {
                                this.CancelEdit();
                            }

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