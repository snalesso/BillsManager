using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SupplierAddEditViewModel : SupplierViewModel
    {
        #region fields

        //private readonly IDialogService dialogService;
        private readonly IWindowManager windowManager;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public SupplierAddEditViewModel(
            Supplier supplier,
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            if (supplier == null)
                throw new ArgumentNullException("supplier cannot be null.");

            this.exposedSupplier = supplier;
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        #region wrapped from supplier

        [Required(ErrorMessage = "You must specify a name.")]
        public override string Name
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

        public override string eMail
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

        public override string Website
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

        public override string Phone
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
        public override string Notes
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
        public override string Street
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
        public override string Number
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
        public override string City
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
        public override ushort Zip
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
        public override string Province
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
        public override string Country
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

        #region overrides

        public new string DisplayName
        {
            get { return this.IsInEditMode ? ("Edit supplier" + (this.IsInEditMode & this.HasChanges ? " [*]" : string.Empty)) : "New supplier"; }
        }

        #endregion

        #endregion

        #region methods

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
                                
                                this.TryClose(false);
                            }
                        }
                        else
                        {
                            if (this.IsInEditMode)
                            {
                                this.CancelEdit();
                            }

                            this.TryClose(false);
                        }
                    });

                return this.cancelAddEditAndCloseCommand;
            }
        }

        #endregion
    }
}