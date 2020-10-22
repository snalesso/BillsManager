using Billy.Billing.Application.DTOs;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Billy.UI.Wpf.Billing.ViewModels
{
    public class SuppliersFiltersViewModel : ReactiveScreen, IDisposable
    {
        private readonly StringComparison _stringComparison = StringComparison.InvariantCultureIgnoreCase;

        public SuppliersFiltersViewModel()
        {
            this.SetDefaultFilters = ReactiveCommand.Create(this.SetDefaultFiltersImpl).DisposeWith(this._disposables);
            this.SetDefaultFilters.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex)).DisposeWith(this._disposables);

            this.WhenShouldReevaluate = this
                .WhenAnyValue(X => X.FullTextSearchString)
                .Select(_ => Unit.Default);
            this.WhenFilterChanged = this.WhenShouldReevaluate.Select(_ => this.Filter);
        }

        private string _fullTextSearchString = null;
        public string FullTextSearchString
        {
            get => this._fullTextSearchString;
            set => this.RaiseAndSetIfChanged(ref this._fullTextSearchString, value);
        }

        public Predicate<SupplierDto> Filter => this.IsSupplierAccepted;
        public IObservable<Unit> WhenShouldReevaluate { get; }
        public IObservable<Predicate<SupplierDto>> WhenFilterChanged { get; }

        private bool IsSupplierAccepted(SupplierDto supplierDto)
        {
            if (supplierDto == null)
                throw new ArgumentNullException(nameof(supplierDto));

            if (this.FullTextSearchString != null
                && !supplierDto.Name.Contains(this.FullTextSearchString, this._stringComparison)
                && !supplierDto.Notes.Contains(this.FullTextSearchString, this._stringComparison))
                return false;

            return true;
        }

        private void SetDefaultFiltersImpl()
        {
            this.FullTextSearchString = null;
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
