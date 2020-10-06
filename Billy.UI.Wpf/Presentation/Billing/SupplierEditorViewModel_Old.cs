using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Billy.Core.Domain.Billing.Application.DTOs;
using Billy.Domain.Billing.Models;
using Caliburn.Micro;
using ReactiveUI;

namespace Billy.UI.Wpf.Presentation.Billing
{
    //public class SupplierEditorViewModel_Old : Screen
    //{
    //    #region constants & fields

    //    private readonly SupplierDTO _supplierDto = null;
    //    private readonly IDictionary<string, OldNewValues> _supplierData = new Dictionary<string, OldNewValues>();

    //    private IDictionary<string, PropertyInfo> _supplierPropertiesDictionary;
    //    private IDictionary<string, PropertyInfo> _vmSupplierPropertiesDictionary;

    //    #endregion

    //    #region constructors

    //    //internal SupplierEditorViewModel() { }

    //    public SupplierEditorViewModel_Old(
    //        SupplierDTO supplierDto = null)
    //    {
    //        this._supplierDto = supplierDto;

    //        this.AgentEditorViewModel = new AgentEditorViewModel(this._supplierDto.Agent);

    //        this.CacheOldProperties();

    //        if (this._supplierDto == null)
    //        {
    //            this.Name = "Nome di Prova " + DateTime.Now.Ticks;
    //            this.Notes = "Note a caso ... " + DateTime.Now.Ticks + new Random((int)DateTime.Now.Ticks).NextDouble();
    //        }
    //    }

    //    #endregion

    //    #region properties

    //    // TODO: make readonly
    //    private IReadOnlyDictionary<string, OldNewValues> _values;
    //    public IReadOnlyDictionary<string, OldNewValues> Values => this._values ??= this._supplierData.ToImmutableDictionary();

    //    private string _name;
    //    public string Name
    //    {
    //        get { return this._name; }
    //        set { this.SetAndTrack(ref this._name, value); }
    //    }

    //    private Agent _agent;
    //    public Agent Agent
    //    {
    //        get { return this._agent; }
    //        set { this.SetAndTrack(ref this._agent, value); }
    //    }

    //    public ValueObjectEditorViewModel<Agent> AgentEditorViewModel { get; }

    //    private string _email;
    //    public string Email
    //    {
    //        get { return this._email; }
    //        set { this.SetAndTrack(ref this._email, value); }
    //    }

    //    private string _website;
    //    public string Website
    //    {
    //        get { return this._website; }
    //        set { this.SetAndTrack(ref this._website, value); }
    //    }

    //    private string _phone;
    //    public string Phone
    //    {
    //        get { return this._phone; }
    //        set { this.SetAndTrack(ref this._phone, value); }
    //    }

    //    private string _fax;
    //    public string Fax
    //    {
    //        get { return this._fax; }
    //        set { this.SetAndTrack(ref this._fax, value); }
    //    }

    //    private string _notes;
    //    public string Notes
    //    {
    //        get { return this._notes; }
    //        set { this.SetAndTrack(ref this._notes, value); }
    //    }

    //    private Address _address;
    //    public Address Address
    //    {
    //        get { return this._address; }
    //        set { this.SetAndTrack(ref this._address, value); }
    //    }

    //    #endregion

    //    #region methods

    //    protected T SetAndTrack<T>(ref T field, T newValue, [CallerMemberName] string callerMemberName = null)
    //    {
    //        this.Set(ref field, newValue, callerMemberName);
    //        if (this._supplierData.TryGetValue(callerMemberName, out var onv))
    //        {
    //            if (onv != null)
    //            {
    //                onv.NewValue = newValue;
    //            }
    //        }
    //        else
    //        {
    //            // TODO: improve system, we should never get here
    //            // the old value was not saved but the value is already set, so ... shit!
    //        }
    //        return newValue;
    //    }

    //    private PropertyInfo[] GetProperties<T>()
    //    {
    //        return typeof(T).GetProperties(
    //            BindingFlags.Public
    //            | BindingFlags.DeclaredOnly
    //            | BindingFlags.Instance
    //            | BindingFlags.SetProperty
    //            | BindingFlags.GetProperty);
    //    }

    //    private void AddPropertyCache<TSource, TProperty>(
    //        IDictionary<string, OldNewValues> dict,
    //        TSource obj,
    //        Expression<Func<TSource, TProperty>> propertyAccessor,
    //        Action<TProperty> setProperty)
    //    {
    //        var propertyName = PropertyHelper.GetMemberName(propertyAccessor);
    //        var propertyValue = obj != null ? propertyAccessor.Compile()(obj) : default;

    //        if (dict.ContainsKey(propertyName))
    //        {
    //            if (dict.TryGetValue(propertyName, out var oldNewValues) == false)
    //            {
    //                if (oldNewValues == null)
    //                {
    //                    dict[propertyName] = new OldNewValues(propertyValue);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            dict[propertyName] = new OldNewValues(propertyValue);
    //        }

    //        setProperty.Invoke(propertyValue);
    //    }

    //    private void CacheOldProperties()
    //    {
    //        // TODO: disable UI before updating
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Address, x => this.Address = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Agent, x => this.Agent = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Email, x => this.Email = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Fax, x => this.Fax = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Name, x => this.Name = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Notes, x => this.Notes = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Phone, x => this.Phone = x);
    //        this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Website, x => this.Website = x);

    //        //using (var inpcSuppression = this.SuppressChangeNotifications())
    //        //{
    //        //    this._supplierData[nameof(this._supplierDto.Address)] = new OldNewValues(this.Address = this._supplierDto.Address);
    //        //    this._supplierData[nameof(this._supplierDto.Agent)] = new OldNewValues(this.Agent = this._supplierDto.Agent);
    //        //    this._supplierData[nameof(this._supplierDto.Email)] = new OldNewValues(this.Email = this._supplierDto.Email);
    //        //    this._supplierData[nameof(this._supplierDto.Fax)] = new OldNewValues(this.Fax = this._supplierDto.Fax);
    //        //    this._supplierData[nameof(this._supplierDto.Name)] = new OldNewValues(this.Name = this._supplierDto.Name);
    //        //    this._supplierData[nameof(this._supplierDto.Notes)] = new OldNewValues(this.Notes = this._supplierDto.Notes);
    //        //    this._supplierData[nameof(this._supplierDto.Phone)] = new OldNewValues(this.Phone = this._supplierDto.Phone);
    //        //    this._supplierData[nameof(this._supplierDto.Website)] = new OldNewValues(this.Website = this._supplierDto.Website);
    //        //}
    //        //this.RaisePropertyChanged(null);
    //    }

    //    private void ResetProperties()
    //    {
    //        // TODO: disable UI before updating

    //        // ensure we have PropertyInfo[]
    //        // for supplier
    //        this._supplierPropertiesDictionary ??= this.GetProperties<Supplier>().ToDictionary(x => x.Name);
    //        // for VM
    //        if (this._vmSupplierPropertiesDictionary == null)
    //        {
    //            this._vmSupplierPropertiesDictionary = this.GetProperties<AddSupplierViewModel>()
    //                .Where(vmp => this._supplierPropertiesDictionary.ContainsKey(vmp.Name))
    //                .ToDictionary(x => x.Name);
    //        }

    //        // set VM properties' on supplier's values
    //        foreach (var supplierProperty in this._supplierPropertiesDictionary.Values)
    //        {
    //            if (this._vmSupplierPropertiesDictionary.TryGetValue(supplierProperty.Name, out var vmProperty) == false)
    //            {
    //                throw new KeyNotFoundException(); // TODO: we got a problem here
    //            }

    //            var supplierPropertyValue = supplierProperty.GetValue(this._supplierDto);
    //            vmProperty.SetValue(this, supplierPropertyValue);
    //        }
    //    }

    //    #endregion
    //}
}