using BillsManager.Localization;
using BillsManager.Models;
using BillsManager.ViewModels.Commanding;
using BillsManager.ViewModels.Messages;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.ViewModels
{
    // TODO: create a filter class with a description for the report subtitle
    public partial class SearchBillsViewModel :
        Screen,
        IHandle<SuppliersListChangedMessage>,
        IHandle<SupplierAddedMessage>,
        IHandle<SupplierEditedMessage>,
        IHandle<SupplierDeletedMessage>,
        IHandle<BillEditedMessage>,
        IHandle<ShowSuppliersBillsOrder>,
        IHandle<SelectedSupplierChagedMessage>
    {
        #region fields

        private IEventAggregator globalEventAggregator;

        private Filter<BillDetailsViewModel> supplierNameFilter;

        private Filter<BillDetailsViewModel> isPaidFilter;

        private Filter<BillDetailsViewModel> releaseDateFilter;

        private Filter<BillDetailsViewModel> dueDateFilter;


        #endregion

        #region ctor

        // TODO: should available suppliers be injected?
        public SearchBillsViewModel(
            IEventAggregator globalEventAggregator)
        {
            // SERVICES
            this.globalEventAggregator = globalEventAggregator;

            // SUBSCRIPTIONS
            this.globalEventAggregator.Subscribe(this);

            // FILTERS
            this.ConfigureFilters();
            //this.isPaidFilter = bvm => bvm.PaymentDate.HasValue == this.IsPaidFilterValue;
            //this.supplierNameFilter = bvm => bvm.SupplierID == this.SelectedSupplier.ID;
            //this.releaseDateFilter = bvm => bvm.ReleaseDate == this.ReleaseDateFilterValue;
            //this.dueDateFilter = bvm => bvm.DueDate == this.dueDateFilterValue;

            // handlers
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
                };
        }

        private void ConfigureFilters()
        {
            this.supplierNameFilter =
                new Filter<BillDetailsViewModel>(
                    bdvm => bdvm.SupplierID == this.SelectedSupplier.ID,
                    () =>
                        TranslationManager.Instance.Translate("By").ToString().ToLower(TranslationManager.Instance.CurrentLanguage) +
                        " " + this.SelectedSupplier.Name);

            this.isPaidFilter =
                new Filter<BillDetailsViewModel>(
                    bdvm => bdvm.IsPaid == this.IsPaidFilterValue,
                    () =>
                        this.IsPaidFilterValue.HasValue ?
                        (this.IsPaidFilterValue == true ? TranslationManager.Instance.Translate("Paid_toBills").ToString() :
                        TranslationManager.Instance.Translate("NotPaid_toBills").ToString()) :
                        TranslationManager.Instance.Translate("All_toBills").ToString());

            this.dueDateFilter =
                new Filter<BillDetailsViewModel>(
                    bdvm => bdvm.DueDate == this.DueDateFilterValue,
                    () => TranslationManager.Instance.Translate("DuedOn_toBills").ToString() + " " + this.DueDateFilterValue.Value.ToShortDateString());

            this.releaseDateFilter =
                new Filter<BillDetailsViewModel>(
                    bdvm => bdvm.ReleaseDate == this.ReleaseDateFilterValue,
                    () => TranslationManager.Instance.Translate("ReleasedOn_toBills").ToString() + " " + this.ReleaseDateFilterValue.Value.ToShortDateString());
        }

        #endregion

        #region properties

        private bool useSupplierFilter;
        public bool UseSupplierFilter
        {
            get { return this.useSupplierFilter; }
            set
            {
                if (this.AvailableSuppliers == null) value = false;

                if (this.useSupplierFilter == value) return;


                this.useSupplierFilter = value;
                this.NotifyOfPropertyChange(() => this.UseSupplierFilter);
                this.SelectedSupplier = (value ? this.AvailableSuppliers.FirstOrDefault() : null);
            }
        }

        private IEnumerable<Supplier> availableSuppliers;
        public IEnumerable<Supplier> AvailableSuppliers
        {
            get
            {
                if (this.availableSuppliers == null)
                    this.globalEventAggregator.Publish(
                        new AvailableSuppliersRequest(suppliers => this.availableSuppliers = suppliers));

                return this.availableSuppliers;
            }
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
                if (this.selectedSupplier == value) return;

                this.selectedSupplier = value;
                this.NotifyOfPropertyChange(() => this.SelectedSupplier);
                this.useSupplierFilter = (value != null);
                this.NotifyOfPropertyChange(() => this.UseSupplierFilter);
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
                switch (this.IsPaidFilterValue)
                {
                    case true:
                        return TranslationManager.Instance.Translate("Paid_toBills").ToString();
                    case false:
                        return TranslationManager.Instance.Translate("NotPaid_toBills").ToString();
                    default:
                        return TranslationManager.Instance.Translate("All_toBills").ToString();
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
            List<Filter<BillDetailsViewModel>> filters = new List<Filter<BillDetailsViewModel>>();

            if (this.UseSupplierFilter) filters.Add(this.supplierNameFilter);
            if (this.IsPaidFilterValue.HasValue) filters.Add(this.isPaidFilter);
            if (this.UseReleaseDateFilter) filters.Add(this.releaseDateFilter);
            if (this.UseDueDateFilter) filters.Add(this.dueDateFilter);

            if (filters.Count > 0)
                this.globalEventAggregator.Publish(new BillsFilterMessage(filters));
            else
                this.globalEventAggregator.Publish(new BillsFilterMessage(null));
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

            if (!this.UseSupplierFilter) return;

            if (!this.AvailableSuppliers.Contains(this.SelectedSupplier))
            {
                this.UseSupplierFilter = false;
                this.SendFilters();
            }
        }

        public void Handle(SupplierAddedMessage message)
        {
            this.AvailableSuppliers = this.AvailableSuppliers.Concat(new[] { message.AddedSupplier });
        }

        public void Handle(SupplierEditedMessage message)
        {
            if (message.OldSupplierVersion.Name != message.NewSupplierVersion.Name)
            {
                var supps = this.AvailableSuppliers;
                var selSupp = this.SelectedSupplier;

                this.AvailableSuppliers = null; // TODO: avoid a wasted refresh
                this.SelectedSupplier = null;

                this.AvailableSuppliers = supps;
                this.SelectedSupplier = selSupp;
            }
        }

        public void Handle(SupplierDeletedMessage message)
        {
            this.AvailableSuppliers = this.AvailableSuppliers.Where(supplier => supplier != message.DeletedSupplier);
            this.SelectedSupplier =
                (this.SelectedSupplier == message.DeletedSupplier ?
                this.AvailableSuppliers.FirstOrDefault() :
                this.SelectedSupplier);
        }


        public void Handle(BillEditedMessage message)
        {
            bool refreshNeeded = false;

            if (this.UseSupplierFilter)
                refreshNeeded |= (message.OldBillVersion.SupplierID != message.NewBillVersion.SupplierID);

            if (this.UseReleaseDateFilter)
                refreshNeeded |= (message.OldBillVersion.ReleaseDate != message.NewBillVersion.ReleaseDate);

            if (this.UseDueDateFilter)
                refreshNeeded |= (message.OldBillVersion.DueDate != message.NewBillVersion.DueDate);

            if (this.IsPaidFilterValue != null)
                refreshNeeded |= (message.OldBillVersion.PaymentDate.HasValue != message.NewBillVersion.PaymentDate.HasValue);

            if (refreshNeeded)
                this.SendFilters();
        }


        public void Handle(ShowSuppliersBillsOrder message)
        {
            if (message.Supplier == this.SelectedSupplier) return;

            if (!this.AvailableSuppliers.Contains(message.Supplier)) return;

            this.SelectedSupplier = message.Supplier;
            this.SendFilters();
        }

        public void Handle(SelectedSupplierChagedMessage message)
        {
            if (!this.UseSupplierFilter) return;

            if (!this.AvailableSuppliers.Contains(message.SelectedSupplier)) return;

            this.SelectedSupplier = message.SelectedSupplier;
            this.SendFilters();
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