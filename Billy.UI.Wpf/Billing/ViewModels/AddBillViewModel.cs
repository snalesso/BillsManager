using Billy.Billing.Application.DTOs;
using Billy.Billing.Services;
using Caliburn.Micro.ReactiveUI.Validation;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Billing.ViewModels
{
    public class AddBillViewModel : ReactiveValidatableScreen, INotifyDataErrorInfo, IValidatableViewModel, IDisposable
    {
        #region constants & fields

        private readonly IBillsService _billsService;
        private readonly ISuppliersService _suppliersService;
        private readonly BillDto _billDto;

        #endregion

        #region constructors

        public AddBillViewModel(
            IBillsService billsService,
            ISuppliersService suppliersService,
            BillDto bill)
        {
            this._billsService = billsService ?? throw new ArgumentNullException(nameof(billsService));
            this._suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
            this._billDto = bill ?? throw new ArgumentNullException(nameof(bill));

            this._suppliersSourceCache = new SourceCache<SupplierDto, long>(x => x.Id).DisposeWith(this._disposables);
            this._suppliersSubscription_Serial = new SerialDisposable().DisposeWith(this._disposables);

            this.LoadSuppliers = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var suppliers = await this._suppliersService.GetAllAsync().ConfigureAwait(false);
                    this._suppliersSourceCache.Edit(updater =>
                    {
                        updater.Clear();
                        updater.AddOrUpdate(suppliers);
                    });
                })
                .DisposeWith(this._disposables);
            this.LoadSuppliers.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(ex => Debug.WriteLine(ex))
                .DisposeWith(this._disposables);

            this.ValidationContext.DisposeWith(this._disposables);

            this.ValidationRule(
                vm => vm.SupplierId,
                propVal => propVal.HasValue,
                propVal => $"Invalid {nameof(AddBillViewModel.Code)} format.")
                .DisposeWith(this._disposables);
            this.ValidationRule(
                vm => vm.Code,
                propVal => !string.IsNullOrWhiteSpace(propVal),
                propVal => $"Invalid {nameof(AddBillViewModel.Code)} format.")
                .DisposeWith(this._disposables);
            this.ValidationRule(
                vm => vm.ReleaseDate,
                propVal => !propVal.HasValue || propVal.Value <= DateTime.Now,
                propVal => $"{nameof(AddBillViewModel.ReleaseDate)} must be in the past.")
                .DisposeWith(this._disposables);
            this.ValidationRule(
                vm => vm.PaymentDate,
                propVal => !propVal.HasValue || propVal.Value <= DateTime.Now,
                propVal => $"{nameof(AddBillViewModel.PaymentDate)} must be in the past.")
                .DisposeWith(this._disposables);
            this.ValidationRule(
                 this
                    .WhenAnyValue(x => x.ReleaseDate, x => x.DueDate)
                    .Select(dates =>
                    {
                        var (releaseDate, dueDate) = dates;

                        return !releaseDate.HasValue
                        || !dueDate.HasValue
                        || releaseDate <= dueDate;
                    }),
                 $"{nameof(AddBillViewModel.DueDate)} cannot come before {nameof(AddBillViewModel.ReleaseDate)}.")
                .DisposeWith(this._disposables);
            this.ValidationRule(
                 this
                    .WhenAnyValue(x => x.ReleaseDate, x => x.PaymentDate)
                    .Select(dates =>
                    {
                        var (releaseDate, paymentDate) = dates;

                        return !releaseDate.HasValue
                        || !paymentDate.HasValue
                        || releaseDate <= paymentDate;
                    }),
                 $"{nameof(AddBillViewModel.PaymentDate)} cannot come before {nameof(AddBillViewModel.ReleaseDate)}.")
                .DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        [Reactive] public long? SupplierId { get; set; }
        [Reactive] public string Code { get; set; }
        [Reactive] public DateTime? ReleaseDate { get; set; }
        [Reactive] public DateTime? DueDate { get; set; }
        [Reactive] public DateTime? PaymentDate { get; set; }
        //[Reactive] public DateTime? RegistrationDate { get; set; }
        [Reactive] public double Amount { get; set; }
        [Reactive] public double Agio { get; set; }
        [Reactive] public double AdditionalCosts { get; set; }
        [Reactive] public string Notes { get; set; }

        //private ReadOnlyObservableCollection<SupplierDto> _supplliers;
        [Reactive]
        public ReadOnlyObservableCollection<SupplierDto> Supplliers
        {
            get /*=> this._supplliers*/;
            private set /*=> this.SetAndNotify(ref this._supplliers, value)*/;
        }

        #endregion

        #region methods

        private readonly IObservable<IReadOnlyCollection<SupplierDto>> _suppliersChanges;
        private IDisposable _suppliersSubscription;
        private SerialDisposable _suppliersSubscription_Serial;
        private ISourceCache<SupplierDto, long> _suppliersSourceCache;

        //protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        //{
        //    if (this.Supplliers != null)
        //    {
        //        var supplierDtos = await this._suppliersService.GetAllAsync().ConfigureAwait(false);

        //        this.Supplliers = new ReadOnlyObservableCollection<SupplierDto>(
        //            new ObservableCollection<SupplierDto>(supplierDtos));
        //    }

        //    await base.OnActivateAsync(cancellationToken).ConfigureAwait(false);
        //}

        //protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        //{
        //    this._suppliersSubscription_Serial.Disposable = null;

        //    this._suppliersSubscription?.Dispose();
        //    this._suppliersSubscription = null;

        //    return base.OnDeactivateAsync(close, cancellationToken);
        //}

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> LoadSuppliers { get; }

        #endregion

        //#region validation

        //#region IValidatableViewModel

        //public ValidationContext ValidationContext { get; }

        //#endregion

        //#region INotifyDataErrorInfo

        //public bool HasErrors => throw new NotImplementedException();

        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        //public IEnumerable GetErrors(string propertyName)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        //#endregion

        #region IDisposable

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            //base.Dispose(isDisposing);
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}