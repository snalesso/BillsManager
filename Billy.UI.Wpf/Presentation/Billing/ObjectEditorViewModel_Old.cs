using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Billy.Domain.Models;
using Caliburn.Micro;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public abstract class ObjectEditorViewModel_Old<TValueObject> : Screen
        where TValueObject : ValueObject<TValueObject>
    {
        private readonly TValueObject _originalObject;
        private readonly IDictionary<string, IValueTracker> _trackings;

        public ObjectEditorViewModel_Old(TValueObject originalObject = default)
        {
            this._originalObject = originalObject;

            this._trackings = new Dictionary<string, IValueTracker>();
            this.SetOriginalValues();
        }

        protected abstract void SetOriginalValues();

        public IReadOnlyDictionary<string, object> GetChanges()
        {
            var dict = this._trackings.Where(x => x.Value.IsChanged).ToDictionary(x => x.Key, x => x.Value.Current);
            return new ReadOnlyDictionary<string, object>(dict);
        }

        public abstract TValueObject GetEdited();

        protected void TrackOriginalValue<TThis, TProperty>(
            TThis vm,
            Expression<Func<TThis, TProperty>> vmPropertyName,
            Func<TValueObject, TProperty> originalValueGetter)
        {
            var propertyName = vmPropertyName.GetMemberName();
            var originalValue =
                this._originalObject != null
                ? originalValueGetter.Invoke(this._originalObject)
                : default;

            this._trackings.TryAdd(propertyName, new PropertyTracker<TProperty>(originalValue));
        }

        protected TProperty SetAndTrack<TProperty>(TProperty newValue, [CallerMemberName] string propertyName = null)
        {
            var hasKey = this._trackings.TryGetValue(propertyName, out var tracker);

            if (hasKey == false)
            {
                if (this._trackings.TryAdd(propertyName, new PropertyTracker<TProperty>(newValue)) == false)
                {
                    throw new Exception("Could not track the property. Cannot add the key.");
                }
            }
            else
            {
                var tTracker = (PropertyTracker<TProperty>)tracker;

                if (tTracker == null)
                {
                    var tType = typeof(TValueObject);
                    var ov =
                        this._originalObject != null
                        ? tType.GetProperty(propertyName).GetValue(this._originalObject)
                        : default;

                    if (tType.GetType().IsAssignableFrom(ov.GetType()) == false)
                    {
                        // TODO: handle
                        throw new InvalidCastException($"{this.GetType().FullName}.{propertyName} and {typeof(TValueObject).FullName}.{propertyName} types do not match.");
                    }

                    this._trackings[propertyName] = new PropertyTracker<TProperty>((TProperty)ov);
                }
                else
                {
                    this._trackings[propertyName].Current = newValue;
                }
            }

            this.NotifyOfPropertyChange(propertyName);

            return newValue;
        }

        protected TProperty GetTracked<TProperty>([CallerMemberName] string propertyName = null)
        {
            var hasKey = this._trackings.TryGetValue(propertyName, out var tracker);
            var tTracker = (PropertyTracker<TProperty>)tracker;

            if (hasKey == false || tTracker == null)
            {
                // TODO: handle
                throw new Exception($"Property {propertyName} is not being tracked.");
            }

            return tTracker.Current;
        }
    }
}