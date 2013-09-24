using System;
using System.Collections.Generic;
using System.Linq;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SearchSuppliersViewModel : Screen
    {
        #region fields

        private readonly IEventAggregator eventAggregator;

        private readonly Func<SupplierViewModel, bool> obligationStateFilter;

        #endregion

        #region ctor

        public SearchSuppliersViewModel(
            IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            this.obligationStateFilter = s => s.ObligationState == this.ObligationStateFilterValue;

            this.eventAggregator.Subscribe(this);
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
            List<Func<SupplierViewModel, bool>> filters = new List<Func<SupplierViewModel, bool>>();

            if (this.UseObligationStateFilter) filters.Add(this.obligationStateFilter);

            if (filters.Count > 0)
                this.eventAggregator.Publish(new SuppliersFilterMessage(filters));
            else
                this.eventAggregator.Publish(new SuppliersFilterMessage((Func<SupplierViewModel, bool>)null));
        }

        void DeactivateAllFilters()
        {
            this.UseObligationStateFilter = false;
        }

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
