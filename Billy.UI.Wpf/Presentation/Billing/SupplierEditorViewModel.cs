using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Billy.Core.Domain.Billing.Application.DTOs;
using Billy.Domain.Billing.Models;
using Billy.Domain.Models;
using Caliburn.Micro;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class SupplierEditorViewModel : ObjectEditorViewModel<SupplierDTO>
    {
        #region constants & fields

        private readonly SupplierDTO _supplierDto = null;

        #endregion

        #region constructors

        public SupplierEditorViewModel(
            SupplierDTO supplierDto = null)
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

    public class AgentEditorViewModel : ValueObjectEditorViewModel<Agent>
    {
        public AgentEditorViewModel(Agent agent)
            : base(agent)
        {
            // TODO: make this setup automatic with some config which handles both setup & changes calc
            this.Name = agent?.Name;
            this.Surname = agent?.Surname;
            this.Phone = agent?.Phone;
        }

        #region properties

        private string _name;
        public string Name
        {
            get => this._name;
            set => this.Set(ref this._name, value);
            //get => this.GetTracked<string>();
            //set => this.SetAndTrack(value);
        }

        private string _surname;
        public string Surname
        {
            get => this._surname;
            set => this.Set(ref this._surname, value);
            //get => this.GetTracked<string>();
            //set => this.SetAndTrack(value);
        }

        private string _phone;
        public string Phone
        {
            get => this._phone;
            set => this.Set(ref this._phone, value);
            //get => this.GetTracked<string>();
            //set => this.SetAndTrack(value);
        }

        #endregion

        protected override void UpdateChanges()
        {
            this.UpdateChange(this, vm => vm.Name, agent => agent.Name);
            this.UpdateChange(this, vm => vm.Surname, agent => agent.Surname);
            this.UpdateChange(this, vm => vm.Phone, agent => agent.Phone);
        }

        protected override Agent GetEditedValueParser(IReadOnlyDictionary<string, object> changes)
        {
            return Agent.Create(
                this.GetFinalPropertyValue(x => x.Name),
                this.GetFinalPropertyValue(x => x.Surname),
                this.GetFinalPropertyValue(x => x.Phone));
        }

        //public override Agent GetEdited()
        //{
        //    var changes = this.GetChanges();
        //    //return Agent.Create(this._)
        //    throw new NotImplementedException();
        //}

        //protected override void SetOriginalValues()
        //{
        //    this.TrackOriginalValue(this, x => x.Name, x => x.Name);
        //    this.TrackOriginalValue(this, x => x.Surname, x => x.Surname);
        //    this.TrackOriginalValue(this, x => x.Phone, x => x.Phone);
        //}
    }

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

    public abstract class ObjectEditorViewModel<TObject> : Screen
        where TObject : class
    {
        protected readonly TObject _originalObject;
        private readonly IDictionary<string, object> _changes;

        public ObjectEditorViewModel(TObject originalObject = default)
        {
            this._originalObject = originalObject;

            this._changes = new Dictionary<string, object>();
        }

        protected TProperty GetFinalPropertyValue<TProperty>(string propertyName, TProperty fallbackValue = default)
        {
            return this._changes.TryGetValue(propertyName, out var value) ? (TProperty)value : fallbackValue;
        }

        protected TProperty GetFinalPropertyValue<TProperty>(Expression<Func<TObject, TProperty>> propertyGetterExpr, TProperty fallbackValue = default)
        {
            return this._changes.TryGetValue(propertyGetterExpr.GetMemberName(), out var value) ? (TProperty)value : fallbackValue;
        }

        [Obsolete("Cache values")]
        public IReadOnlyDictionary<string, object> GetChanges()
        {
            this.UpdateChanges();

            return new ReadOnlyDictionary<string, object>(this._changes);
        }

        protected abstract void UpdateChanges();

        protected void UpdateChange<TProperty>(
            string propertyName,
            ValueObjectEditorViewModel<TProperty> editor,
            Func<TObject, TProperty> originalValueGetter)
            //where TProperty : class
            where TProperty : ValueObject<TProperty>
        {
            var originalValue =
                this._originalObject != null
                ? originalValueGetter(this._originalObject)
                : default;
            var newValue = editor.GetEditedValue();
            var isChanged = object.Equals(originalValue, newValue) == false;

            if (isChanged == true)
            {
                var changes = editor.GetChanges();

                if (this._changes.ContainsKey(propertyName))
                {
                    this._changes[propertyName] = changes;
                }
                else
                {
                    if (this._changes.TryAdd(propertyName, changes) == false)
                    {
                        // TODO: handle
                        throw new Exception("Shiet");
                    }
                }
            }
            else
            {
                // TODO: remove property from dictionary
            }
        }

        protected void UpdateChange<TProperty>(
            Expression<Func<TObject, TProperty>> changedPropertyNameExpr,
            ValueObjectEditorViewModel<TProperty> editor)
            //where TProperty : class
            where TProperty : ValueObject<TProperty>
        {
            this.UpdateChange(changedPropertyNameExpr.GetMemberName(), editor, changedPropertyNameExpr.Compile());
        }

        protected void UpdateChange<TThis, TProperty>(
            TThis target,
            Func<TThis, TProperty> newValueGetter,
            Expression<Func<TObject, TProperty>> originalValueGetterExpr)
            where TProperty : IEquatable<TProperty>
        {
            this.UpdateChange(target, newValueGetter, originalValueGetterExpr.Compile(), originalValueGetterExpr.GetMemberName());
        }

        protected void UpdateChange<TThis, TProperty>(
            TThis target,
            Func<TThis, TProperty> newValueGetter,
            Func<TObject, TProperty> originalValueGetter,
            string changedPropertyName)
            where TProperty : IEquatable<TProperty>
        {
            TProperty newValue = newValueGetter(target);
            TProperty originalValue =
                this._originalObject != null
                ? originalValueGetter(this._originalObject)
                : default;

            var valueChanged = object.Equals(newValue, originalValue) == false;

            if (valueChanged)
            {
                if (this._changes.ContainsKey(changedPropertyName))
                {
                    this._changes[changedPropertyName] = newValue; // this.GetChanges();
                }
                else
                {
                    this._changes.Add(changedPropertyName, newValue); //this.GetChanges());
                }
            }
            else
            {
                if (this._changes.ContainsKey(changedPropertyName))
                {
                    this._changes.Remove(changedPropertyName);
                }
            }
        }
    }

    public abstract class ValueObjectEditorViewModel<TValueObject> : ObjectEditorViewModel<TValueObject>
        where TValueObject : ValueObject<TValueObject>
    {
        //private readonly TValueObject _originalValueObject;
        //private readonly IDictionary<string, object> _changes;

        public ValueObjectEditorViewModel(TValueObject originalObject = default)
            : base(originalObject)
        {
            //    this._originalValueObject = originalObject;

            //    this._changes = new Dictionary<string, object>();
        }

        public TValueObject GetEditedValue()
        {
            var changes = this.GetChanges();

            if (changes == null || changes.Count <= 0)
            {
                // we can return the direct object instance because this is a value object
                return this._originalObject;
            }

            return this.GetEditedValueParser(changes);
        }
        protected abstract TValueObject GetEditedValueParser(IReadOnlyDictionary<string, object> changes);

        //public IReadOnlyDictionary<string, object> GetChanges()
        //{
        //    this.UpdateChanges();

        //    return new ReadOnlyDictionary<string, object>(this._changes);
        //}

        //protected abstract void UpdateChanges();

        //protected void UpdateChange<TThis, TProperty>(
        //    TThis target,
        //    Func<TThis, TProperty> newValueGetter,
        //    Expression<Func<TValueObject, TProperty>> originalValueGetterExpr)
        //    where TProperty : IEquatable<TProperty>
        //{
        //    this.UpdateChange(target, newValueGetter, originalValueGetterExpr.Compile(), originalValueGetterExpr.GetMemberName());
        //}

        //protected void UpdateChange<TThis, TProperty>(
        //    /*[NotNull]*/ TThis target,
        //    Func<TThis, TProperty> newValueGetter,
        //    Func<TValueObject, TProperty> originalValueGetter,
        //    string changedPropertyName)
        //{
        //    TProperty newValue = newValueGetter(target);
        //    TProperty originalValue =
        //        this._originalValueObject != null
        //        ? originalValueGetter(this._originalValueObject)
        //        : default;

        //    if (object.Equals(newValue, originalValue) == true)
        //    {
        //        if (this._changes.ContainsKey(changedPropertyName))
        //        {
        //            this._changes[changedPropertyName] = newValue;
        //        }
        //        else
        //        {
        //            this._changes.Add(changedPropertyName, newValue);
        //        }
        //    }
        //    else
        //    {
        //        if (this._changes.ContainsKey(changedPropertyName))
        //        {
        //            this._changes.Remove(changedPropertyName);
        //        }
        //    }
        //}
    }
}