using System.ComponentModel;
using BillsManager.Models;

namespace BillsManager.ViewModels
{
    public partial class SupplierAddEditViewModel : IEditableObject
    {
        #region IEditableObject

        #region fields

        private Supplier _uneditedSupplier;

        #endregion

        #region properties

        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get { return this.isInEditMode; }
            private set
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
            private set
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
            this._uneditedSupplier = (Supplier)this.ExposedSupplier.Clone();

            this.IsInEditMode = true;
        }

        private void RevertChanges()
        {
            this.eMail = this._uneditedSupplier.eMail;
            this.Name = this._uneditedSupplier.Name;
            this.Notes = this._uneditedSupplier.Notes;
            this.Phone = this._uneditedSupplier.Phone;
            this.Fax = this._uneditedSupplier.Fax;
            this.Website = this._uneditedSupplier.Website;

            //this.Address = this._uneditedSupplier.Address; // TODO: review
            this.Street = this._uneditedSupplier.Street;
            this.Number = this._uneditedSupplier.Number;
            this.Zip = this._uneditedSupplier.Zip;
            this.City = this._uneditedSupplier.City;
            this.Province = this._uneditedSupplier.Province;
            this.Country = this._uneditedSupplier.Country;

            this.AgentName = this._uneditedSupplier.AgentName;
            this.AgentSurname = this._uneditedSupplier.AgentSurname;
            this.AgentPhone = this._uneditedSupplier.AgentPhone;
        }

        public void CancelEdit()
        {
            this.RevertChanges();

            this.EndEdit();
        }

        public void EndEdit()
        {
            this.HasChanges = false;

            this._uneditedSupplier = null;

            this.IsInEditMode = false;
        }

        #endregion

        #endregion
    }
}