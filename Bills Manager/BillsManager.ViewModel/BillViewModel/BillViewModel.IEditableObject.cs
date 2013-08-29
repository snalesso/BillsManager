using System.Collections.Generic;
using System.ComponentModel;
using BillsManager.Model;
using BillsManager.ViewModel.Messages;
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

        private IEnumerable<Supplier> GetAvailableSuppliers()
        {
            IEnumerable<Supplier> s = null;

            this.eventAggregator.Publish(new AskForAvailableSuppliersMessage(supps => s = supps));

            return s;
        }

        public void Handle(AvailableSuppliersMessage message)
        {
            if (this.IsInEditMode)
                this.AvailableSuppliers = message.AvailableSuppliers;
        }

        public void BeginEdit()
        {
            this.exposedBillBackup = (Bill)this.ExposedBill.Clone();

            this.IsInEditMode = true;

            this.SetSuppliersProperties();
        }

        protected void RevertChanges()
        {
            this.Amount = this.exposedBillBackup.Amount;
            this.Code = this.exposedBillBackup.Code;
            this.DueDate = this.exposedBillBackup.DueDate;
            this.Notes = this.exposedBillBackup.Notes;
            this.PaymentDate = this.exposedBillBackup.PaymentDate;
            this.RegistrationDate = this.exposedBillBackup.RegistrationDate;
            this.ReleaseDate = this.exposedBillBackup.ReleaseDate;
            this.Supplier = this.exposedBillBackup.Supplier; 
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
