using Billy.Billing.Models;
using Billy.Billing.Services;
using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.Billing.ViewModels
{
    public class AddSupplierViewModel :
        //EditSupplierViewModel
        Screen // TODO: make Conductor.AllActive, where items are child-entities/ValueObject editors
        , IDisposable
    {
        #region constants & fields

        //private readonly IReadLibraryService _readLibraryService;
        //private readonly IWriteLibraryService _writeLibraryService;
        //private readonly Func<IBillingUnitOfWork> _billingUowFactoryMethod;
        private readonly ISuppliersService _suppliersService;

        #endregion

        #region constructors

        public AddSupplierViewModel(
            ISuppliersService suppliersService)
        {
            this._suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));

            this.SupplierEditorViewModel = new SupplierEditorViewModel();

            this.TrySaveAndClose = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var changedValues = this.SupplierEditorViewModel.GetChanges();
                    //this.SupplierEditorViewModel.Values.Where(x => x.Value.OldValue != x.Value.NewValue).ToDictionary(x => x.Key, x => x.Value.NewValue);

                    // TODO: handle esceptions/response
                    var addedSupplier = await this._suppliersService.CreateAndAddAsync(changedValues).ConfigureAwait(false);
                    if (addedSupplier != null)
                    {
                        await this.TryCloseAsync(true).ConfigureAwait(false);
                    }
                });
            this.TrySaveAndClose.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.TrySaveAndClose.DisposeWith(this._disposables);

            this.DiscardAndClose = ReactiveCommand.CreateFromTask(
                async () => await this.TryCloseAsync(false).ConfigureAwait(false));
            this.DiscardAndClose.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.DiscardAndClose.DisposeWith(this._disposables);

            this.DisplayName = "Add " + nameof(Supplier);
        }

        #endregion

        #region properties

        public SupplierEditorViewModel SupplierEditorViewModel { get; }

        //private string _name;
        //public string Name
        //{
        //    get { return this._name; }
        //    set { this.RaiseAndSetIfChanged(ref this._name, value); }
        //}

        //private Agent _agent;
        //public Agent Agent
        //{
        //    get { return this._agent; }
        //    set { this.RaiseAndSetIfChanged(ref this._agent, value); }
        //}

        //private string _email;
        //public string Email
        //{
        //    get { return this._email; }
        //    set { this.RaiseAndSetIfChanged(ref this._email, value); }
        //}

        //private string _website;
        //public string Website
        //{
        //    get { return this._website; }
        //    set { this.RaiseAndSetIfChanged(ref this._website, value); }
        //}

        //private string _phone;
        //public string Phone
        //{
        //    get { return this._phone; }
        //    set { this.RaiseAndSetIfChanged(ref this._phone, value); }
        //}

        //private string _fax;
        //public string Fax
        //{
        //    get { return this._fax; }
        //    set { this.RaiseAndSetIfChanged(ref this._fax, value); }
        //}

        //private string _notes;
        //public string Notes
        //{
        //    get { return this._notes; }
        //    set { this.RaiseAndSetIfChanged(ref this._notes, value); }
        //}

        //private Address _address;
        //public Address Address
        //{
        //    get { return this._address; }
        //    set { this.RaiseAndSetIfChanged(ref this._address, value); }
        //}

        #endregion

        #region methods

        public override Task TryCloseAsync(bool? dialogResult = null)
        {
            return base.TryCloseAsync(dialogResult);
        }

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            return base.CanCloseAsync(cancellationToken);
        }

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> TrySaveAndClose { get; }
        public ReactiveCommand<Unit, Unit> DiscardAndClose { get; }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        //protected override void Dispose(bool isDisposing)
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