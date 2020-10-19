using Billy.Domain.Models;
using System.Collections.Generic;

namespace Billy.UI.Wpf.Common.ViewModels
{
    public abstract class ValueObjectEditorViewModel<TValueObject> : ObjectEditorViewModel<TValueObject>
        where TValueObject : ValueObject<TValueObject>
    {
        public ValueObjectEditorViewModel(TValueObject originalObject = default)
            : base(originalObject)
        {
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
    }
}