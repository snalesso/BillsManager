using Billy.Billing.Application.DTOs;
using Billy.Billing.Models;
using Billy.UI.Wpf.Common.ViewModels;

namespace Billy.Billing.ViewModels
{
    public class SupplierEditorViewModel : ObjectEditorViewModel<SupplierDto>
    {
        #region constants & fields

        private readonly SupplierDto _supplierDto = null;

        #endregion

        #region constructors

        public SupplierEditorViewModel(
            SupplierDto supplierDto = null)
            : base(supplierDto)
        {
            this._supplierDto = supplierDto;

            this.Name = supplierDto?.Name;
            this.Notes = supplierDto?.Notes;
            this.Email = supplierDto?.Email;
            this.Phone = supplierDto?.Phone;
            this.Website = supplierDto?.Website;
            this.Fax = supplierDto?.Fax;

            this.AgentEditorViewModel = new AgentEditorViewModel(this._supplierDto?.Agent);
            this.AddressEditorViewModel = new AddressEditorViewModel(this._supplierDto?.Address);
        }

        #endregion

        #region properties

        private string _name;
        public string Name
        {
            get { return this._name; }
            set { this.Set(ref this._name, value); }
        }

        private string _email;
        public string Email
        {
            get { return this._email; }
            set { this.Set(ref this._email, value); }
        }

        private string _website;
        public string Website
        {
            get { return this._website; }
            set { this.Set(ref this._website, value); }
        }

        private string _phone;
        public string Phone
        {
            get { return this._phone; }
            set { this.Set(ref this._phone, value); }
        }

        private string _fax;
        public string Fax
        {
            get { return this._fax; }
            set { this.Set(ref this._fax, value); }
        }

        private string _notes;
        public string Notes
        {
            get { return this._notes; }
            set { this.Set(ref this._notes, value); }
        }

        public ValueObjectEditorViewModel<Agent> AgentEditorViewModel { get; }

        public ValueObjectEditorViewModel<Address> AddressEditorViewModel { get; }

        #endregion

        #region methods

        protected override void UpdateChanges()
        {
            this.UpdateChange(this, vm => vm.Name, supplier => supplier.Name);
            this.UpdateChange(this, vm => vm.Notes, supplier => supplier.Notes);
            this.UpdateChange(this, vm => vm.Email, supplier => supplier.Email);
            this.UpdateChange(this, vm => vm.Website, supplier => supplier.Website);
            this.UpdateChange(this, vm => vm.Phone, supplier => supplier.Phone);
            this.UpdateChange(this, vm => vm.Fax, supplier => supplier.Fax);

            this.UpdateChange(x => x.Address, this.AddressEditorViewModel);
            this.UpdateChange(x => x.Agent, this.AgentEditorViewModel);
        }

        #endregion
    }
}