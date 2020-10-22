using Billy.Domain.Models;
using System;
using System.Collections.Generic;

namespace Billy.Billing.Models
{
    // TODO: make entity? identity is defined by type+value, other properties are decorations
    public class Contact : ValueObject<Contact>
    {
        #region ctor

        public Contact(
            ContactType type,
            string value,
            string name,
            string notes = null)
        {
            this.Type = type;
            this.Value = value?.TrimToNull() ?? throw new ArgumentNullException(nameof(value));
            this.Name = name?.TrimToNull() ?? throw new ArgumentNullException(nameof(name));
            this.Notes = notes?.TrimToNull();
        }

        #endregion

        #region properties

        public ContactType Type { get; }
        public string Value { get; }
        public string Name { get; }
        public string Notes { get; }

        #endregion

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Type;
            yield return this.Value;
            yield return this.Name;
            yield return this.Notes;
        }

        #endregion
    }
}