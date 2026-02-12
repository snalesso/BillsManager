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
    public class BillViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly BillDto _billDto;
        //private readonly Func<Bill, EditBillViewModel> _editBillTagsViewModelFactoryMethod;

        #endregion

        #region constructors

        public BillViewModel(
            BillDto billDto
            //, Func<Bill, EditBillViewModel> editBillViewModelFactoryMethod
            )
        {
            this._billDto = billDto ?? throw new ArgumentNullException(nameof(billDto));
            //this._editBillTagsViewModelFactoryMethod = editBillViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editBillViewModelFactoryMethod));
        }

        #endregion

        #region properties

        //[Obsolete("Find a way to not expose this, maybe making this VM editable.")]
        //public BillDto BillDto => this._billDto;

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
    }
}