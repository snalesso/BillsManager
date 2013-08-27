using System.ComponentModel;
using BillsManager.Model;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class BillViewModel : Screen, IEditableObject
    {
        #region IEditableObject

        private Bill exposedBillBackup;

        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get { return this.isInEditMode; }
            set
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
            this.IsInEditMode = true;

            this.exposedBillBackup = (Bill)this.ExposedBill.Clone();
        }

        public void CancelEdit()
        {
            this.Amount = this.exposedBillBackup.Amount;
            this.Code = this.exposedBillBackup.Code;
            this.DueDate = this.exposedBillBackup.DueDate;
            this.Notes = this.exposedBillBackup.Notes;
            this.PaymentDate = this.exposedBillBackup.PaymentDate;
            this.RegistrationDate = this.exposedBillBackup.RegistrationDate;
            this.ReleaseDate = this.exposedBillBackup.ReleaseDate;
            this.Supplier = this.exposedBillBackup.Supplier;

            this.HasChanges = false;
            this.IsInEditMode = false;
            this.exposedBillBackup = null;
        }

        public void EndEdit()
        {
            this.IsInEditMode = false;
            this.HasChanges = false;
            this.exposedBillBackup = null;
        }

        #endregion
    }
}
