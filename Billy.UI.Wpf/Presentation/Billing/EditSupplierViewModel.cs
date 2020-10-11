using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using Billy.Billing.Application;
using Billy.Billing.Application.DTOs;
using Billy.Domain.Billing.Models;
using Billy.Billing.Persistence;
using Caliburn.Micro;
using ReactiveUI;
using System.Threading.Tasks;
using System.Threading;
using Caliburn.Micro.ReactiveUI;
using Billy.Billing.Services;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class EditSupplierViewModel : ReactiveScreen
    {
        #region constants & fields

        private readonly ISuppliersService _suppliersService;
        private readonly SupplierDto _supplierDto;

        #endregion

        #region constructors

        public EditSupplierViewModel(
            ISuppliersService suppliersService,
            SupplierDto supplierDto)
        {
            this._suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
            this._supplierDto = supplierDto ?? throw new ArgumentNullException(nameof(supplierDto));

            this.SupplierEditorViewModel = new SupplierEditorViewModel(this._supplierDto);

            this.TrySaveAndClose = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    var changedValues = this.SupplierEditorViewModel.GetChanges();
                    //this.SupplierEditorViewModel.Values.Where(x => x.Value.OldValue != x.Value.NewValue).ToDictionary(x => x.Key, x => x.Value.NewValue);

                    // TODO: handle esceptions/response
                    try
                    {
                        await this._suppliersService.UpdateAsync(this._supplierDto.Id, changedValues);
                        await this.TryCloseAsync(true);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            this.TrySaveAndClose.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.TrySaveAndClose.DisposeWith(this._disposables);

            this.DiscardAndClose = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await this.TryCloseAsync(false);
                });
            this.DiscardAndClose.ThrownExceptions
                .Subscribe(ex => Debug.WriteLine(ex.Message))
                .DisposeWith(this._disposables);
            this.DiscardAndClose.DisposeWith(this._disposables);

            this.DisplayName = "Edit " + nameof(Supplier) + " [" + this._supplierDto.Id + "]";
        }

        #endregion

        #region properties

        public SupplierEditorViewModel SupplierEditorViewModel { get; }

        //private string _name;
        //public string Name
        //{
        //    get { return this._name; }
        //    set { this.SetAndTrack(ref this._name, value); }
        //}

        //private Agent _agent;
        //public Agent Agent
        //{
        //    get { return this._agent; }
        //    set { this.SetAndTrack(ref this._agent, value); }
        //}

        //private string _email;
        //public string Email
        //{
        //    get { return this._email; }
        //    set { this.SetAndTrack(ref this._email, value); }
        //}

        //private string _website;
        //public string Website
        //{
        //    get { return this._website; }
        //    set { this.SetAndTrack(ref this._website, value); }
        //}

        //private string _phone;
        //public string Phone
        //{
        //    get { return this._phone; }
        //    set { this.SetAndTrack(ref this._phone, value); }
        //}

        //private string _fax;
        //public string Fax
        //{
        //    get { return this._fax; }
        //    set { this.SetAndTrack(ref this._fax, value); }
        //}

        //private string _notes;
        //public string Notes
        //{
        //    get { return this._notes; }
        //    set { this.SetAndTrack(ref this._notes, value); }
        //}

        //private Address _address;
        //public Address Address
        //{
        //    get { return this._address; }
        //    set { this.SetAndTrack(ref this._address, value); }
        //}

        #endregion

        #region methods

        public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            // TODO: prevent losing unsaved changes
            return base.CanCloseAsync(cancellationToken);
        }

        //protected T SetAndTrack<T>(ref T field, T newValue, [CallerMemberName] string callerMemberName = null)
        //{
        //    this.Set(ref field, newValue, callerMemberName);
        //    this._supplierData[callerMemberName].NewValue = newValue;
        //    return newValue;
        //}

        //private PropertyInfo[] GetProperties<T>()
        //{
        //    return typeof(T).GetProperties(
        //        BindingFlags.Public
        //        | BindingFlags.DeclaredOnly
        //        | BindingFlags.Instance
        //        | BindingFlags.SetProperty
        //        | BindingFlags.GetProperty);
        //}

        //private void AddPropertyCache<TObject, TProperty>(
        //    IDictionary<string, OldNewValues> dict,
        //    TObject obj,
        //    Expression<Func<TObject, TProperty>> propertyAccessor)
        //{
        //    var propertyName = PropertyHelper.GetMemberName(propertyAccessor);
        //    var propertyValue = obj != null ? propertyAccessor.Compile()(obj) : default;

        //    if (dict.ContainsKey(propertyName))
        //    {
        //        if (dict.TryGetValue(propertyName, out var oldNewValues) == false)
        //        {
        //            if (oldNewValues == null)
        //            {
        //                dict[propertyName] = new OldNewValues(propertyValue);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        dict[propertyName] = new OldNewValues(propertyValue);
        //    }
        //}

        //private void CacheOldProperties()
        //{
        //    // TODO: disable UI before updating
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Address);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Agent);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Email);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Fax);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Name);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Notes);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Phone);
        //    this.AddPropertyCache(this._supplierData, this._supplierDto, x => x.Website);

        //    //using (var inpcSuppression = this.SuppressChangeNotifications())
        //    //{
        //    //    this._supplierData[nameof(this._supplierDto.Address)] = new OldNewValues(this.Address = this._supplierDto.Address);
        //    //    this._supplierData[nameof(this._supplierDto.Agent)] = new OldNewValues(this.Agent = this._supplierDto.Agent);
        //    //    this._supplierData[nameof(this._supplierDto.Email)] = new OldNewValues(this.Email = this._supplierDto.Email);
        //    //    this._supplierData[nameof(this._supplierDto.Fax)] = new OldNewValues(this.Fax = this._supplierDto.Fax);
        //    //    this._supplierData[nameof(this._supplierDto.Name)] = new OldNewValues(this.Name = this._supplierDto.Name);
        //    //    this._supplierData[nameof(this._supplierDto.Notes)] = new OldNewValues(this.Notes = this._supplierDto.Notes);
        //    //    this._supplierData[nameof(this._supplierDto.Phone)] = new OldNewValues(this.Phone = this._supplierDto.Phone);
        //    //    this._supplierData[nameof(this._supplierDto.Website)] = new OldNewValues(this.Website = this._supplierDto.Website);
        //    //}
        //    //this.RaisePropertyChanged(null);
        //}

        //private void ResetProperties()
        //{
        //    // TODO: disable UI before updating

        //    // ensure we have PropertyInfo[]
        //    // for supplier
        //    this._supplierPropertiesDictionary ??= this.GetProperties<Supplier>().ToDictionary(x => x.Name);
        //    // for VM
        //    if (this._vmSupplierPropertiesDictionary == null)
        //    {
        //        this._vmSupplierPropertiesDictionary = this.GetProperties<AddSupplierViewModel>()
        //            .Where(vmp => this._supplierPropertiesDictionary.ContainsKey(vmp.Name))
        //            .ToDictionary(x => x.Name);
        //    }

        //    // set VM properties' on supplier's values
        //    foreach (var supplierProperty in this._supplierPropertiesDictionary.Values)
        //    {
        //        if (this._vmSupplierPropertiesDictionary.TryGetValue(supplierProperty.Name, out var vmProperty) == false)
        //        {
        //            throw new KeyNotFoundException(); // TODO: we got a problem here
        //        }

        //        var supplierPropertyValue = supplierProperty.GetValue(this._supplierDto);
        //        vmProperty.SetValue(this, supplierPropertyValue);
        //    }
        //}

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> TrySaveAndClose { get; }
        public ReactiveCommand<Unit, Unit> DiscardAndClose { get; }

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
                return;

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;

            // remove in non-derived class
            //base.Dispose(isDisposing);
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}