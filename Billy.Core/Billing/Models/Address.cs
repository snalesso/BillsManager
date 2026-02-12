using Billy.Domain.Models;
using System.Collections.Generic;
using System;

namespace Billy.Billing.Models
{
    public class Address : ValueObject<Address>
    {
        //public static readonly Address Null = new Address();

        #region ctor

        // TODO: only for null object
        //private Address() { }

        protected Address(
            string country,
            string province,
            string city,
            string zip,
            string street,
            string number)
        {
            this.Country = country.TrimToNull();
            this.Province = province.TrimToNull();
            this.City = city.TrimToNull();
            this.Zip = zip.TrimToNull();
            this.Street = street.TrimToNull();
            this.Number = number.TrimToNull();
        }

        #endregion

        #region properties

        public string Country { get; }
        public string Province { get; }
        public string City { get; }
        public string Zip { get; }
        public string Street { get; }
        public string Number { get; }

        #endregion

        public static Address Create(
            string country = null,
            string province = null,
            string city = null,
            string zip = null,
            string street = null,
            string number = null)
        {
            country = country.TrimToNull();
            province = province.TrimToNull();
            city = city.TrimToNull();
            zip = zip.TrimToNull();
            street = street.TrimToNull();
            number = number.TrimToNull();

            if (country == null
                && province == null
                && city == null
                && zip == null
                && street == null
                && number == null)
            {
                return null;
            }

            return new Address(country, province, city, zip, street, number);
        }

        #region ValueObject

        protected override IEnumerable<object> GetValueIngredients()
        {
            yield return this.Country;
            yield return this.Province;
            yield return this.City;
            yield return this.Zip;
            yield return this.Street;
            yield return this.Number;
        }

        #endregion
    }
}