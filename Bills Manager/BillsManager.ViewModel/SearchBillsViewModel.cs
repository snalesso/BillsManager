using System;
using System.Collections.Generic;
using System.Linq;
using BillsManager.Model;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SearchBillsViewModel :
        Screen,
        IHandle<SuppliersListChangedMessage>,
        IHandle<SupplierEditedMessage>,
        IHandle<BillEditedMessage>,
        IHandle<ActivateBillsSupplierFilterMessage>
    {
        #region fields

        private readonly IEventAggregator eventAggregator;

        private readonly Func<BillViewModel, bool> isPaidFilter;
        private readonly Func<BillViewModel, bool> supplierNameFilter;
        private readonly Func<BillViewModel, bool> releaseDateFilter;
        private readonly Func<BillViewModel, bool> dueDateFilter;

        #endregion

        #region ctor

        public SearchBillsViewModel(
            IEnumerable<Supplier> availableSuppliers,
            IEventAggregator eventAggregator)
        {
            this.AvailableSuppliers = availableSuppliers;
            this.eventAggregator = eventAggregator;

            this.isPaidFilter = bvm => bvm.PaymentDate.HasValue == this.IsPaidFilterValue;
            this.supplierNameFilter = bvm => bvm.SupplierID == this.SelectedSupplier.ID;
            this.releaseDateFilter = bvm => bvm.ReleaseDate == this.ReleaseDateFilterValue;
            this.dueDateFilter = bvm => bvm.DueDate == this.dueDateFilterValue;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        private bool useSupplierFilter;
        public bool UseSupplierFilter
        {
            get { return this.useSupplierFilter; }
            set
            {
                if (this.useSupplierFilter != value)
                {
                    this.useSupplierFilter = value;
                    this.NotifyOfPropertyChange(() => this.UseSupplierFilter);
                    this.SelectedSupplier = (value ? this.AvailableSuppliers.FirstOrDefault() : null);
                }
            }
        }

        private IEnumerable<Supplier> availableSuppliers;
        public IEnumerable<Supplier> AvailableSuppliers
        {
            get { return this.availableSuppliers; }
            set
            {
                if (this.availableSuppliers != value)
                {
                    this.availableSuppliers = value;
                    this.NotifyOfPropertyChange(() => this.AvailableSuppliers);
                }
            }
        }

        private Supplier selectedSupplier;
        public Supplier SelectedSupplier
        {
            get { return this.selectedSupplier; }
            set
            {
                if (this.selectedSupplier != value)
                {
                    this.selectedSupplier = value;
                    this.NotifyOfPropertyChange(() => this.SelectedSupplier);
                    this.useSupplierFilter = (value != null);
                    this.NotifyOfPropertyChange(() => this.UseSupplierFilter);
                }
            }
        }

        private bool? isPaidFilterValue;
        public bool? IsPaidFilterValue
        {
            get { return this.isPaidFilterValue; }
            set
            {
                if (this.isPaidFilterValue != value)
                {
                    this.isPaidFilterValue = value;
                    this.NotifyOfPropertyChange(() => this.IsPaidFilterValue);
                    this.NotifyOfPropertyChange(() => this.IsPaidFilterState);
                }
            }
        }

        public string IsPaidFilterState
        {
            get
            {
                switch (this.IsPaidFilterValue) // TODO: language
                {
                    case true:
                        return "Paid";
                    case false:
                        return "Not paid";
                    default:
                        return "All";
                }
            }
        }

        private bool useReleaseDateFilter;
        public bool UseReleaseDateFilter
        {
            get { return this.useReleaseDateFilter; }
            set
            {
                if (this.useReleaseDateFilter != value)
                {
                    this.useReleaseDateFilter = value;
                    this.NotifyOfPropertyChange(() => this.UseReleaseDateFilter);
                    this.ReleaseDateFilterValue = (value ? DateTime.Today : (DateTime?)null);
                }
            }
        }

        private DateTime? releaseDateFilterValue = null;
        public DateTime? ReleaseDateFilterValue
        {
            get { return this.releaseDateFilterValue; }
            set
            {
                if (this.releaseDateFilterValue != value)
                {
                    this.releaseDateFilterValue = value;
                    this.NotifyOfPropertyChange(() => this.ReleaseDateFilterValue);
                    this.useReleaseDateFilter = (value != null);
                    this.NotifyOfPropertyChange(() => this.UseReleaseDateFilter);
                }
            }
        }

        private bool useDueDateFilter;
        public bool UseDueDateFilter
        {
            get { return this.useDueDateFilter; }
            set
            {
                if (this.useDueDateFilter != value)
                {
                    this.useDueDateFilter = value;
                    this.NotifyOfPropertyChange(() => this.UseDueDateFilter);
                    this.DueDateFilterValue = (value ? DateTime.Today : (DateTime?)null);
                }
            }
        }

        private DateTime? dueDateFilterValue = null;
        public DateTime? DueDateFilterValue
        {
            get { return this.dueDateFilterValue; }
            set
            {
                if (this.dueDateFilterValue != value)
                {
                    this.dueDateFilterValue = value;
                    this.NotifyOfPropertyChange(() => this.DueDateFilterValue);
                    this.useDueDateFilter = (value != null);
                    this.NotifyOfPropertyChange(() => this.UseDueDateFilter);
                }
            }
        }

        #endregion

        #region methods

        private void SendFilters()
        {
            List<Func<BillViewModel, bool>> filters = new List<Func<BillViewModel, bool>>();

            if (this.IsPaidFilterValue.HasValue) filters.Add(this.isPaidFilter);
            if (this.UseSupplierFilter) filters.Add(this.supplierNameFilter);
            if (this.UseReleaseDateFilter) filters.Add(this.releaseDateFilter);
            if (this.UseDueDateFilter) filters.Add(this.dueDateFilter);

            if (filters.Count > 0)
                this.eventAggregator.Publish(new BillsFilterMessage(filters));
            else
                this.eventAggregator.Publish(new BillsFilterMessage((Func<BillViewModel, bool>)null));
        }

        private void DeactivateAllFilters()
        {
            this.UseDueDateFilter = false;
            this.UseReleaseDateFilter = false;
            this.UseSupplierFilter = false;
            this.IsPaidFilterValue = null;
        }

        #region message handlers

        public void Handle(SuppliersListChangedMessage message)
        {
            this.AvailableSuppliers = message.Suppliers;

            if (!this.AvailableSuppliers.Contains(this.SelectedSupplier))
            {
                this.UseSupplierFilter = false;
                this.SendFilters();
            }
        }

        public void Handle(SupplierEditedMessage message)
        {
            if (message.OldSupplierVersion.Name != message.NewSupplierVersion.Name)
            {
                var supps = this.AvailableSuppliers;
                var selSupp = this.SelectedSupplier;

                this.AvailableSuppliers = null;
                this.SelectedSupplier = null;

                this.AvailableSuppliers = supps;
                this.SelectedSupplier = selSupp;
            }
        }

        public void Handle(BillEditedMessage message)
        {
            this.SendFilters(); // TODO: refresh only if needed
        }

        public void Handle(ActivateBillsSupplierFilterMessage message)
        {
            if (this.AvailableSuppliers.Any(s => s.ID == message.Supplier.ID))
            {
                this.SelectedSupplier = this.AvailableSuppliers.Single(s => s.ID == message.Supplier.ID);
                this.SendFilters();
            }
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand sendFilerCommand;
        public RelayCommand SendFilterCommand
        {
            get
            {
                if (this.sendFilerCommand == null) this.sendFilerCommand = new RelayCommand(
                    () => this.SendFilters());

                return this.sendFilerCommand;
            }
        }

        private RelayCommand exitSearchCommand;
        public RelayCommand ExitSearchCommand
        {
            get
            {
                if (this.exitSearchCommand == null) this.exitSearchCommand = new RelayCommand(
                    () =>
                    {
                        this.DeactivateAllFilters();
                        this.SendFilters();
                    });

                return this.exitSearchCommand;
            }
        }

        #endregion
    }
}