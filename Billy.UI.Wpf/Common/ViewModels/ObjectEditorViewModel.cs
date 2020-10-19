using Billy.Domain.Models;
using Caliburn.Micro.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Billy.UI.Wpf.Common.ViewModels
{
    public abstract class ObjectEditorViewModel<TObject> : ReactiveScreen
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

            if (isChanged)
            {
                var changes = editor.GetChanges();

                if (this._changes.ContainsKey(propertyName))
                {
                    this._changes[propertyName] = changes;
                }
                else
                {
                    this._changes.Add(propertyName, changes);
                }
            }
            else
            {
                if (this._changes.ContainsKey(propertyName))
                {
                    this._changes.Remove(propertyName);
                }
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

            var isChanged = object.Equals(newValue, originalValue) == false;

            if (isChanged)
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
}