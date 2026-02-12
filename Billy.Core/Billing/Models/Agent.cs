using Billy.Domain.Models;
using System.Collections.Generic;
using System;

namespace Billy.Billing.Models
{
    public class Agent : ValueObject<Agent>
    {
        public static readonly Agent NullAgent = new Agent();

        #region ctor

        protected Agent(
            string name,
            string surname,
            string phone)
        {
            this.Name = name.TrimToNull();
            this.Surname = surname.TrimToNull();
            this.Phone = phone.TrimToNull();
        }

        private Agent() : this(null, null, null) { }

        #endregion

        public static Agent Create(
            string name = null,
            string surname = null,
            string phone = null)
        {
            name = name.TrimToNull();
            surname = surname.TrimToNull();
            phone = phone.TrimToNull();

            if (name == null
                && surname == null
                && phone == null)
            {
                return null;
            }

            return new Agent(name, surname, phone);
        }

        #region properties

        public string Name { get; }
        public string Surname { get; }
        public string Phone { get; }

        #endregion

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Name;
            yield return this.Surname;
            yield return this.Phone;
        }

        #endregion
    }
}