using Billy.Billing.Application.DTOs;
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

        //private readonly IDialogService _dialogService;

        private readonly BillDto _billDto;
        //private readonly Func<Bill, EditBillViewModel> _editBillTagsViewModelFactoryMethod;

        #endregion

        #region constructors

        public BillViewModel(
            BillDto billDto
            //, IBillsRepository billsRepository,
            //, IDialogService dialogService,
            //, Func<Bill, EditBillViewModel> editBillViewModelFactoryMethod
            )
        {
            this._billDto = billDto ?? throw new ArgumentNullException(nameof(billDto));
            //this._billsRepository = billsRepository ?? throw new ArgumentNullException(nameof(billsRepository));
            //this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            //this._editBillTagsViewModelFactoryMethod = editBillViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editBillViewModelFactoryMethod));
        }

        #endregion

        #region properties

        [Obsolete("Find a way to not expose this, maybe making this VM editable.")]
        public BillDto BillDto => this._billDto;

        public long Id => this._billDto.Id;
        public long SupplierId => this._billDto.SupplierId;
        public DateTime ReleaseDate => this._billDto.ReleaseDate;
        public DateTime DueDate => this._billDto.DueDate;
        public DateTime? PaymentDate => this._billDto.PaymentDate;
        public DateTime RegistrationDate => this._billDto.RegistrationDate;
        public double Amount => this._billDto.Amount;
        public double Agio => this._billDto.Agio;
        public double AdditionalCosts => this._billDto.AdditionalCosts;
        public string Notes => this._billDto.Notes;
        public string Code => this._billDto.Code;

        public bool IsPaid => this.PaymentDate.HasValue;

        #endregion

        #region methods

        #endregion

        #region commands

        #endregion

        #region IDisposable

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