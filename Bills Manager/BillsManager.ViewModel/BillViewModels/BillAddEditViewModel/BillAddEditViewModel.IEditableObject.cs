using BillsManager.Models;
using System.ComponentModel;

namespace BillsManager.ViewModels
{
    public partial class BillAddEditViewModel : IEditableObject
    {
        #region IEditableObject

        private Bill exposedBillBackup;

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
            this.exposedBillBackup = (Bill)this.ExposedBill.Clone(); // TODO: ICloneable<T> ?

            this.IsInEditMode = true;
        }

        private void RevertChanges()
        {
            // TODO: check for full coverage
            this.AdditionalCosts = this.exposedBillBackup.AdditionalCosts;
            this.Agio = this.exposedBillBackup.Agio;
            this.Amount = this.exposedBillBackup.Amount;
            this.Code = this.exposedBillBackup.Code;
            this.DueDate = this.exposedBillBackup.DueDate;
            this.Notes = this.exposedBillBackup.Notes;
            this.PaymentDate = this.exposedBillBackup.PaymentDate;
            this.RegistrationDate = this.exposedBillBackup.RegistrationDate;
            this.ReleaseDate = this.exposedBillBackup.ReleaseDate;
            this.SupplierID = this.exposedBillBackup.SupplierID;
        }

        public void CancelEdit()
        {
            this.RevertChanges();

            this.EndEdit();
        }

        public void EndEdit()
        {
            this.HasChanges = false;

            this.exposedBillBackup = null;

            this.IsInEditMode = false;
        }

        #endregion
    }
}