using Billy.Billing.Application.DTOs;
using Billy.Billing.Persistence;
using Billy.Billing.Services;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Billy.Billing.ViewModels
{
    public class BillsFiltersViewModel : ReactiveScreen, IDisposable
    {
        private readonly ISuppliersService _suppliersService;

        public BillsFiltersViewModel(
            ISuppliersService suppliersService)
        {
            this._suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));

            this.SetDefaultFilters = ReactiveCommand.Create(this.SetDefaultFiltersValues).DisposeWith(this._disposables);
            this.SetDefaultFilters.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex)).DisposeWith(this._disposables);

            this._criteria_BehaviorSubject = new BehaviorSubject<BillCriteria>(new()).DisposeWith(this._disposables);
            this.WhenCriteriaChanged = this._criteria_BehaviorSubject
                .DistinctUntilChanged()
                //.Do(x => Debug.WriteLine("Criteria changed"))
                ;

            this._isPaid_OAPH =
                //this.WhenAnyValue(x => x.Criteria)
                this._criteria_BehaviorSubject
                .Select(x => x.IsPaid)
                .ObserveOn(RxApp.MainThreadScheduler)
                //.Do(x => Debug.WriteLine("Is paid changed"))
                .ToProperty(this, nameof(this.IsPaid), initialValue: this.Criteria.IsPaid)
                .DisposeWith(this._disposables);
        }

        //private readonly ObservableAsPropertyHelper<BillCriteria> _criteria_OAPH;
        //public BillCriteria Criteria => this._criteria_OAPH.Value;

        //[Reactive] public string FullTextSearchString { get; set; }
        //[Reactive] public bool? IsPaid { get; set; }

        private readonly BehaviorSubject<BillCriteria> _criteria_BehaviorSubject;
        public BillCriteria Criteria
        {
            get => this._criteria_BehaviorSubject.Value;
            private set
            {
                if (this._criteria_BehaviorSubject.Value == value)
                    return;

                this._criteria_BehaviorSubject.OnNext(value);
                this.RaisePropertyChanged(nameof(this.Criteria));
            }
        }

        private ObservableAsPropertyHelper<bool?> _isPaid_OAPH;
        public bool? IsPaid
        {
            get => this._isPaid_OAPH.Value;
            set
            {
                if (this.IsPaid == value)
                    return;

                this.Criteria = this.Criteria with { IsPaid = value };
            }
        }

        //private BillCriteria _criteria = new();
        //public BillCriteria Criteria
        //{
        //    get => this._criteria;
        //    private set
        //    {
        //        this.RaisePropertyChanging();
        //        this._criteria = value;
        //        this.RaisePropertyChanged();
        //    }
        //}

        //public string FullTextSearchString
        //{
        //    get => this.Criteria.FullText;
        //    set
        //    {
        //        this.RaisePropertyChanging();
        //        this.Criteria = this.Criteria with { FullText = value };
        //        this.RaisePropertyChanged();
        //    }
        //}

        //public bool? IsPaid
        //{
        //    get => this.Criteria.IsPaid;
        //    set
        //    {
        //        this.RaisePropertyChanging();
        //        this.Criteria = this.Criteria with { IsPaid = value };
        //        this.RaisePropertyChanged();
        //    }
        //}

        public IObservable<BillCriteria> WhenCriteriaChanged { get; }
        //public IObservable<Unit> WhenShouldReevaluateCriteria { get; }

        [Reactive] public ReadOnlyObservableCollection<SupplierHeaderDto> Suppliers { get; private set; }
        [Reactive] public SupplierDto SelectedSupplier { get; set; }

        private void SetDefaultFiltersValues()
        {
            //this.Criteria = new();
        }

        public ReactiveCommand<Unit, Unit> SetDefaultFilters { get; }

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}