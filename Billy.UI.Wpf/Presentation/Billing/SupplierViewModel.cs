using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Billy.Billing.Application.DTOs;
using Billy.Domain.Billing.Models;
using Billy.Billing.Persistence;
using Billy.UI.Wpf.Services;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class SupplierViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        //private readonly IDialogService _dialogService;

        private readonly SupplierDto _supplierDto;
        //private readonly Func<Supplier, EditSupplierViewModel> _editSupplierTagsViewModelFactoryMethod;

        #endregion

        #region constructors

        public SupplierViewModel(
            SupplierDto supplierDto
            //, ISuppliersRepository suppliersRepository,
            //, IDialogService dialogService,
            //, Func<Supplier, EditSupplierViewModel> editSupplierViewModelFactoryMethod
            )
        {
            this._supplierDto = supplierDto ?? throw new ArgumentNullException(nameof(supplierDto));
            //this._suppliersRepository = suppliersRepository ?? throw new ArgumentNullException(nameof(suppliersRepository));
            //this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            //this._editSupplierTagsViewModelFactoryMethod = editSupplierViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editSupplierViewModelFactoryMethod));
                    }

        #endregion

        #region properties

        [Obsolete("Find a way to not expose this, maybe making this VM editable.")]
        public SupplierDto SupplierDto => this._supplierDto;

        public int Id => this._supplierDto.Id;
        public string Name => this._supplierDto.Name;
        public string Notes => this._supplierDto.Notes;
        public string Fax => this._supplierDto.Fax;
        public string Email => this._supplierDto.Email;
        public string Website => this._supplierDto.Website;
        public string Phone => this._supplierDto.Phone;
        public Address Address => this._supplierDto.Address;

        public string AddressString =>
            this._supplierDto.Address != null
            ? StringExtensions.Join(
                ", ",
                StringExtensions.Join(" ", this._supplierDto.Address.Street, this._supplierDto.Address.Number),
                this._supplierDto.Address.Zip,
                StringExtensions.Join(" ", this._supplierDto.Address.City, this._supplierDto.Address.Province.SurroundedBy("(", ")")),
                this._supplierDto.Address.Country)
            : null;

        public string AgentString =>
            this._supplierDto.Agent != null
            ? StringExtensions.Join(" ", this._supplierDto.Agent.Name, this._supplierDto.Agent.Surname, this._supplierDto.Agent.Phone.SurroundedBy("(", ")"))
            : null;

        #endregion

        #region methods

        //public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        //{
        //    return base.CanCloseAsync(cancellationToken);
        //}

        //public override void CanClose(Action<bool> callback)
        //{
        //    base.CanClose(callback);
        //}

        #endregion

        #region commands

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // free managed resources here

                //this._isLoaded_OAPH = null;
                //this._isLoved_OAPH = null;

                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
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