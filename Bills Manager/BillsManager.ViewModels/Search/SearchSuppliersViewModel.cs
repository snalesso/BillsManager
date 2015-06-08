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
    public partial class SearchSuppliersViewModel : Screen,
        IHandle<AddedMessage<Bill>>,
        IHandle<EditedMessage<Bill>>,
        IHandle<DeletedMessage<Bill>>
    {
        #region fields

        private readonly IEventAggregator globalEventAggregator;

        private Filter<SupplierDetailsViewModel> obligationStateFilter;

        #endregion

        #region ctor

        public SearchSuppliersViewModel(
            IEventAggregator globalEventAggregator)
        {
            // SERVICES
            this.globalEventAggregator = globalEventAggregator;

            // SUBSCRIPTIONS
            this.globalEventAggregator.Subscribe(this);

            // HANDLERS
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                    {
                        this.globalEventAggregator.Unsubscribe(this);
                    }
                };

            // FILTERS
            this.ConfigureFilters();
            //this.obligationStateFilter = s => s.ObligationState == this.ObligationStateFilterValue;
        }

        private void ConfigureFilters()
        {
            this.obligationStateFilter =
                new Filter<SupplierDetailsViewModel>(
                    sdvm => sdvm.ObligationState == this.ObligationStateFilterValue,
                    () =>
                    {
                        switch (this.ObligationStateFilterValue)
                        {
                            case Obligation.Unbound:
                                return TranslationManager.Instance.Translate("Unbound_toSuppliers").ToString();
                            case Obligation.Creditor:
                                return TranslationManager.Instance.Translate("Creditor_toSuppliers").ToString();
                            default:
                                return TranslationManager.Instance.Translate("Debtor_toSuppliers").ToString();
                        }
                    });
        }

        #endregion

        #region properties

        private bool useObligationStateFilter;
        public bool UseObligationStateFilter
        {
            get { return this.useObligationStateFilter; }
            set
            {
                if (this.useObligationStateFilter != value)
                {
                    this.useObligationStateFilter = value;
                    this.NotifyOfPropertyChange(() => this.UseObligationStateFilter);
                    this.obligationStateFilterValue = value ? this.ObligationStates.FirstOrDefault() : (Obligation?)null;
                    this.NotifyOfPropertyChange(() => this.ObligationStateFilterValue);
                }
            }
        }

        private Obligation? obligationStateFilterValue;
        public Obligation? ObligationStateFilterValue
        {
            get { return this.obligationStateFilterValue; }
            set
            {
                if (this.obligationStateFilterValue != value)
                {
                    this.obligationStateFilterValue = value;
                    this.NotifyOfPropertyChange(() => this.ObligationStateFilterValue);
                    if (!this.UseObligationStateFilter & value != null)
                    {
                        this.useObligationStateFilter = true;
                        this.NotifyOfPropertyChange(() => this.UseObligationStateFilter);
                    }
                }
            }
        }

        public IEnumerable<Obligation> ObligationStates
        {
            get { return Enum.GetValues(typeof(Obligation)).Cast<Obligation>(); }
        }

        #endregion

        #region methods

        void SendFilters()
        {
            List<Filter<SupplierDetailsViewModel>> filters = new List<Filter<SupplierDetailsViewModel>>();

            if (this.UseObligationStateFilter) filters.Add(this.obligationStateFilter);

            if (filters.Count > 0)
                this.globalEventAggregator.PublishOnUIThread(new FilterMessage<SupplierDetailsViewModel>(filters));
            else
                this.globalEventAggregator.PublishOnUIThread(new FilterMessage<SupplierDetailsViewModel>(null));
        }

        void DeactivateAllFilters()
        {
            this.UseObligationStateFilter = false;
        }

        #region message handlers

        public void Handle(AddedMessage<Bill> message)
        {
            if (!this.UseObligationStateFilter) return;

            this.SendFilters();
        }

        public void Handle(EditedMessage<Bill> message)
        {
            if (!this.UseObligationStateFilter) return;

            if ((message.NewItem.Amount != message.OldItem.Amount) |
                (message.NewItem.PaymentDate.HasValue != message.OldItem.PaymentDate.HasValue))
                this.SendFilters();
        }

        public void Handle(DeletedMessage<Bill> message)
        {
            if (!this.UseObligationStateFilter) return;

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
                return this.sendFilerCommand ?? (this.sendFilerCommand =
                    new RelayCommand(
                        () => this.SendFilters()));
            }
        }

        private RelayCommand exitSearchCommand;
        public RelayCommand ExitSearchCommand
        {
            get
            {
                return this.exitSearchCommand ?? (this.exitSearchCommand =
                    new RelayCommand(
                        () =>
                        {
                            this.DeactivateAllFilters();
                            this.SendFilters();
                        }));
            }
        }

        #endregion
    }
}