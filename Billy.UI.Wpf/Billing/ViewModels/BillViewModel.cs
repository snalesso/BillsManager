using Billy.Billing.Models;
using Billy.Billing.Persistence;
using Billy.UI.Wpf.Common.Services;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Billy.Billing.ViewModels
{
    public class BillViewModel : ReactiveScreen, IDisposable
    {
        #region constants & fields

        private readonly IBillsRepository _billsRepository;
        private readonly IDialogService _dialogService;

        private readonly Bill _bill;
        private readonly Func<Bill, EditBillViewModel> _editBillTagsViewModelFactoryMethod;

        #endregion

        #region constructors

        public BillViewModel(
            Bill bill,
            IBillsRepository billsRepository,
            IDialogService dialogService,
            Func<Bill, EditBillViewModel> editBillViewModelFactoryMethod)
        {
            this._bill = bill ?? throw new ArgumentNullException(nameof(bill));
            this._billsRepository = billsRepository ?? throw new ArgumentNullException(nameof(billsRepository));
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this._editBillTagsViewModelFactoryMethod = editBillViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editBillViewModelFactoryMethod));

            this.EditBillTags = ReactiveCommand.Create(
                () =>
                {
                    this._dialogService.ShowDialogAsync(this._editBillTagsViewModelFactoryMethod?.Invoke(this._bill));
                });
            this.EditBillTags.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.EditBillTags.DisposeWith(this._disposables);
        }

        #endregion

        #region properties

        public long Id => this._bill.Id;

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

        public ReactiveCommand<Unit, Unit> EditBillTags { get; }

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