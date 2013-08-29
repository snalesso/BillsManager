using System.ComponentModel;
using BillsManager.Model;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SupplierViewModel : Screen, IEditableObject
    {
        #region IEditableObject

        #region fields

        private Supplier exposedSupplierBackup;

        #endregion

        #region properties

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

        #endregion

        #region methods

        public void BeginEdit()
        {
            this.exposedSupplierBackup = (Supplier)this.ExposedSupplier.Clone();

            this.IsInEditMode = true;
        }

        private void RevertChanges()
        {
            this.eMail = this.exposedSupplierBackup.eMail;
            this.Name = this.exposedSupplierBackup.Name;
            this.Notes = this.exposedSupplierBackup.Notes;
            this.Phone = this.exposedSupplierBackup.Phone;
            this.Website = this.exposedSupplierBackup.Website;

            //this.Address = this.exposedSupplierBackup.Address; // TODO: review
            this.Street = this.exposedSupplierBackup.Street;
            this.Number = this.exposedSupplierBackup.Number;
            this.Zip = this.exposedSupplierBackup.Zip;
            this.City = this.exposedSupplierBackup.City;
            this.Province = this.exposedSupplierBackup.Province;
            this.Country = this.exposedSupplierBackup.Country;
        }

        public void CancelEdit()
        {
            this.RevertChanges();

            this.EndEdit();
        }

        public void EndEdit()
        {
            this.HasChanges = false;

            this.exposedSupplierBackup = null;

            this.IsInEditMode = false;
        }

        #endregion

        #endregion
    }
}
