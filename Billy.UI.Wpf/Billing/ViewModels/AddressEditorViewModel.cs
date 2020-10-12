using System.Collections.Generic;
using Billy.Domain.Billing.Models;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class AddressEditorViewModel : ValueObjectEditorViewModel<Address>
    {
        public AddressEditorViewModel(Address address)
            : base(address)
        {
            this.Country = address?.Country;
            this.Province = address?.Province;
            this.City = address?.City;
            this.Zip = address?.Zip;
            this.Street = address?.Street;
            this.Number = address?.Number;
        }

        #region properties

        private string _country;
        public string Country
        {
            get => this._country;
            set => this.Set(ref this._country, value);
        }

        private string _province;
        public string Province
        {
            get => this._province;
            set => this.Set(ref this._province, value);
        }

        private string _city;
        public string City
        {
            get => this._city;
            set => this.Set(ref this._city, value);
        }

        private string _ip;
        public string Zip
        {
            get => this._ip;
            set => this.Set(ref this._ip, value);
        }

        private string _street;
        public string Street
        {
            get => this._street;
            set => this.Set(ref this._street, value);
        }

        private string _number;
        public string Number
        {
            get => this._number;
            set => this.Set(ref this._number, value);
        }

        #endregion

        protected override void UpdateChanges()
        {
            this.UpdateChange(this, vm => vm.Country, address => address.Country);
            this.UpdateChange(this, vm => vm.Province, address => address.Province);
            this.UpdateChange(this, vm => vm.City, address => address.City);
            this.UpdateChange(this, vm => vm.Zip, address => address.Zip);
            this.UpdateChange(this, vm => vm.Street, address => address.Street);
            this.UpdateChange(this, vm => vm.Number, address => address.Number);
        }

        protected override Address GetEditedValueParser(IReadOnlyDictionary<string, object> changes)
        {
            return Address.Create(
                this.GetFinalPropertyValue(x => x.Country),
                this.GetFinalPropertyValue(x => x.Province),
                this.GetFinalPropertyValue(x => x.City),
                this.GetFinalPropertyValue(x => x.Zip),
                this.GetFinalPropertyValue(x => x.Street),
                this.GetFinalPropertyValue(x => x.Number));
            //changes.TryGetValue(nameof(Address.Country), out var country) ? (string)country : default,
            //changes.TryGetValue(nameof(Address.Province), out var province) ? (string)province : default,
            //changes.TryGetValue(nameof(Address.City), out var city) ? (string)city : default,
            //changes.TryGetValue(nameof(Address.Zip), out var zip) ? (string)zip : default,
            //changes.TryGetValue(nameof(Address.Street), out var street) ? (string)street : default,
            //changes.TryGetValue(nameof(Address.Number), out var number) ? (string)number : default);
        }

        //public override Address GetEdited()
        //{
        //    var changes = this.GetChanges();
        //    //return Address.Create(this._)
        //    throw new NotImplementedException();
        //}

        //protected override void SetOriginalValues()
        //{
        //    this.TrackOriginalValue(this, x => x.Name, x => x.Name);
        //    this.TrackOriginalValue(this, x => x.Surname, x => x.Surname);
        //    this.TrackOriginalValue(this, x => x.Phone, x => x.Phone);
        //}
    }
}