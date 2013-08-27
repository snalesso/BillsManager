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
        IHandle<AvailableSuppliersMessage>,
        IHandle<SupplierNameChangedMessage>
    {
        #region fields

        private readonly IEventAggregator eventAggregator;

        private readonly Func<BillViewModel, bool> isPaidFilter;
        private readonly Func<BillViewModel, bool> supplierNameFilter;
        private readonly Func<BillViewModel, bool> releaseDateFilter;
        private readonly Func<BillViewModel, bool> dueDateFilter;
        //private readonly Func<BillViewModel, bool> amountFilter;
        //private readonly Func<BillViewModel, bool> codeFilter;

        #endregion

        #region ctor

        public SearchBillsViewModel(
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.isPaidFilter = bvm => bvm.PaymentDate.HasValue == this.IsPaidFilterValue;
            this.supplierNameFilter = bvm => bvm.Supplier.Contains(this.SelectedSupplier.Name);
            this.releaseDateFilter = bvm => bvm.ReleaseDate == this.ReleaseDateFilterValue;
            this.dueDateFilter = bvm => bvm.DueDate == this.dueDateFilterValue;
            //this.amountFilter = bvm => bvm.Amount == this.amountFilterValue;
            //this.codeFilter = bvm => bvm.Code.Contains(this.CodeFilterValue);

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

        //private bool useCodeFilter;
        //public bool UseCodeFilter
        //{
        //    get { return this.useCodeFilter; }
        //    set
        //    {
        //        if (this.useCodeFilter != value)
        //        {
        //            this.useCodeFilter = value;
        //            this.NotifyOfPropertyChange(() => this.UseCodeFilter);
        //        }
        //    }
        //}

        //private string codeFilterValue;
        //public string CodeFilterValue
        //{
        //    get { return this.codeFilterValue; }
        //    set
        //    {
        //        if (this.codeFilterValue != value)
        //        {
        //            this.codeFilterValue = value;
        //            this.NotifyOfPropertyChange(() => this.CodeFilterValue);
        //        }
        //    }
        //}

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

        //private bool useAmountFilter;
        //public bool UseAmountFilter
        //{
        //    get { return this.useAmountFilter; }
        //    set
        //    {
        //        if (this.useAmountFilter != value)
        //        {
        //            this.useAmountFilter = value;
        //            this.NotifyOfPropertyChange(() => this.UseAmountFilter);
        //        }
        //    }
        //}

        //private double amountFilterValue;
        //public double AmountFilterValue
        //{
        //    get { return this.amountFilterValue; }
        //    set
        //    {
        //        if (this.amountFilterValue != value)
        //        {
        //            this.amountFilterValue = value;
        //            this.NotifyOfPropertyChange(() => this.AmountFilterValue);
        //        }
        //    }
        //}

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

        void SendFilters()
        {
            List<Func<BillViewModel, bool>> filters = new List<Func<BillViewModel, bool>>();

            if (this.IsPaidFilterValue.HasValue) filters.Add(this.isPaidFilter);
            if (this.UseSupplierFilter) filters.Add(this.supplierNameFilter);
            if (this.UseReleaseDateFilter) filters.Add(this.releaseDateFilter);
            if (this.UseDueDateFilter) filters.Add(this.dueDateFilter);
            //if (this.UseAmountFilter) filters.Add(this.amountFilter);
            //if (this.UseCodeFilter & !string.IsNullOrEmpty(this.CodeFilterValue)) filters.Add(this.codeFilter);

            if (filters.Count > 0)
                this.eventAggregator.Publish(new BillsFilterMessage(filters));
            else
                this.eventAggregator.Publish(new BillsFilterMessage((Func<BillViewModel, bool>)null));
        }

        void DeactivateAllFilters()
        {
            this.UseDueDateFilter = false;
            this.UseReleaseDateFilter = false;
            this.UseSupplierFilter = false;
            this.IsPaidFilterValue = null;
        }

        #region message handlers

        public void Handle(AvailableSuppliersMessage message)
        {
            this.AvailableSuppliers = message.AvailableSuppliers;
        }

        public void Handle(SupplierNameChangedMessage message)
        {
            var selSupp = this.SelectedSupplier;
            this.NotifyOfPropertyChange(() => this.AvailableSuppliers); // TODO: find a better fix
            this.SelectedSupplier = selSupp;
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