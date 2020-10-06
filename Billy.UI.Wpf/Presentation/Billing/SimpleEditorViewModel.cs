using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace Billy.UI.Wpf.Presentation.Billing
{
    //public class PropertyTracker<TEntity, TProperty> : IValueTracker<TProperty>
    //    where TEntity : ValueObject<TEntity>
    //{
    //    private readonly MemberInfo memberInfo;
    //    private readonly Expression<Func<TEntity, TProperty>> propertyExp;
    //    private readonly PropertyInfo propertyInfo;

    //    public PropertyTracker()
    //    {
    //    }

    //    public TProperty Original { get; }
    //    public TProperty Current { get; set; }
    //}

    //public class PropertyTracker<TEntity, TIdentity, TProperty> : IValueTracker<TProperty>
    //    where TIdentity : IEquatable<TIdentity>
    //    where TEntity : Entity<TIdentity>
    //{
    //    private readonly MemberInfo memberInfo;
    //    private readonly Expression<Func<TEntity, TProperty>> propertyExp;
    //    private readonly PropertyInfo propertyInfo;

    //    public PropertyTracker()
    //    {
    //    }

    //    public TProperty Original { get; }
    //    public TProperty Current { get; set; }
    //}

    //public abstract class SimpleEditorViewModel<T> : Screen
    //    where T : class
    //{
    //    private readonly T _originalObject;

    //    public SimpleEditorViewModel(T originalObject = null)
    //    {
    //        this._originalObject = originalObject;

    //        this._trackings = new Dictionary<string, IValueTracker>();
    //    }

    //    private readonly IDictionary<string, IValueTracker> _trackings;
    //    public IReadOnlyDictionary<string, object> GetChanges()
    //    {
    //        var dict = this._trackings.ToDictionary(x => x.Key, x => x.Value.Current);
    //        return new ReadOnlyDictionary<string, object>(dict);
    //    }

    //    protected TProperty SetAndTrack<TProperty>(
    //        TProperty newValue,
    //        [CallerMemberName] string propertyName = null)
    //    {
    //        if (this._trackings.TryGetValue(propertyName, out var tracker))
    //        {
    //            var tTracker = (PropertyTracker<TProperty>)tracker;

    //            if (tTracker != null)
    //                tTracker.Current = newValue;
    //            else
    //                this._trackings[propertyName] = new PropertyTracker<TProperty>(newValue);
    //        }
    //        else
    //        {
    //            // TODO: handle
    //        }

    //        this.NotifyOfPropertyChange(propertyName);

    //        return newValue;
    //    }

    //    protected TProperty GetTracked<TProperty>(
    //        [CallerMemberName] string propertyName = null)
    //    {
    //        var hasKey = this._trackings.TryGetValue(propertyName, out var tracker);
    //        var tTracker = (PropertyTracker<TProperty>)tracker;

    //        if (hasKey == false || tTracker == null)
    //        {
    //            // TODO: handle
    //            throw new Exception($"Property {propertyName} is not being tracked.");
    //        }

    //        return tTracker.Current;
    //    }
    //}
}