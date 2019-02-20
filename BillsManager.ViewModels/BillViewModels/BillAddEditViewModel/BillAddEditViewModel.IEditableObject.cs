using BillsManager.Models;
using System.ComponentModel;

namespace BillsManager.ViewModels
{
    public partial class BillAddEditViewModel : IEditableObject
    {
        #region IEditableObject

        private Bill _uneditedBill;

        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get { return this.isInEditMode; }
            protected set
            {
                if (this.isInEditMode != value)
                {
                    this.isInEditMode = value;
                    this.NotifyOfPropertyChange(() => this.IsInEditMode);
                }
            }
        }

        private bool hasChanges = false;
        public bool HasChanges
        {
            get { return this.hasChanges; }
            protected set
            {
                if (this.hasChanges != value)
                {
                    this.hasChanges = value;
                    this.NotifyOfPropertyChange(() => this.HasChanges);
                    this.NotifyOfPropertyChange(() => this.DisplayName);
                }
            }
        }

        public void BeginEdit()
        {
            this._uneditedBill = (Bill)this.ExposedBill.Clone(); // TODO: ICloneable<T> ?

            this.IsInEditMode = true;
        }

        private void RevertChanges()
        {
            // TODO: check for full coverage
            this.AdditionalCosts = this._uneditedBill.AdditionalCosts;
            this.Agio = this._uneditedBill.Agio;
            this.Amount = this._uneditedBill.Amount;
            this.Code = this._uneditedBill.Code;
            this.DueDate = this._uneditedBill.DueDate;
            this.Notes = this._uneditedBill.Notes;
            this.PaymentDate = this._uneditedBill.PaymentDate;
            this.RegistrationDate = this._uneditedBill.RegistrationDate;
            this.ReleaseDate = this._uneditedBill.ReleaseDate;
            this.SupplierID = this._uneditedBill.SupplierID;
        }

        public void CancelEdit()
        {
            this.RevertChanges();

            this.EndEdit();
        }

        public void EndEdit()
        {
            this.HasChanges = false;

            this._uneditedBill = null;

            this.IsInEditMode = false;
        }

        #endregion
    }
}