using System;
using Billy.Domain.Billing.Models;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class EditBillViewModel : ReactiveScreen
    {
        #region constants & fields

        //private readonly IReadLibraryService _readLibraryService;
        //private readonly IWriteLibraryService _writeLibraryService;
        private readonly Bill _bill;
        private readonly Func<Bill, EditBillViewModel> _editBillViewModelFactoryMethod;

        #endregion

        #region constructors

        public EditBillViewModel(
            //IReadLibraryService readLibraryService,
            //IWriteLibraryService writeLibraryService,
            Bill bill,
            Func<Bill, EditBillViewModel> editBillViewModelFactoryMethod)
        {
            this._bill = bill ?? throw new ArgumentNullException(nameof(bill));
            //this._readLibraryService = readLibraryService ?? throw new ArgumentNullException(nameof(readLibraryService));
            //this._writeLibraryService = writeLibraryService ?? throw new ArgumentNullException(nameof(writeLibraryService));
            this._editBillViewModelFactoryMethod = editBillViewModelFactoryMethod ?? throw new ArgumentNullException(nameof(editBillViewModelFactoryMethod));

            //this.EditBillViewModel = this._editBillViewModelFactoryMethod.Invoke(this._bill);
        }

        #endregion

        #region properties
        #endregion

        #region methods
        #endregion

        #region commands
        #endregion
    }
}