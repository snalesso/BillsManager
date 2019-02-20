namespace BillsManager.Models
{
    public partial class Address
    {
        #region ctor

        public Address()
        {
        }

        public Address(
            string street,
            string number,
            string city,
            string zip,
            string province,
            string country)
        {
            this.Street = street;
            this.Number = number;
            this.City = city;
            this.Zip = zip;
            this.Province = province;
            this.Country = country;
        }

        #endregion

        #region properties

        private string street;
        public string Street
        {
            get { return this.street; }
            set
            {
                if (this.Street != value)
                {
                    this.street = value;
                }
            }
        }

        private string number;
        public string Number
        {
            get { return this.number; }
            set
            {
                if (this.Number != value)
                {
                    this.number = value;
                }
            }
        }

        private string city;
        public string City
        {
            get { return this.city; }
            set
            {
                if (this.City != value)
                {
                    this.city = value;
                }
            }
        }

        private string zip;
        public string Zip
        {
            get { return this.zip; }
            set
            {
                if (this.Zip != value)
                {
                    this.zip = value;
                }
            }
        }

        private string province;
        public string Province
        {
            get { return this.province; }
            set
            {
                if (this.Province != value)
                {
                    this.province = value;
                }
            }
        }

        private string country;
        public string Country
        {
            get { return this.country; }
            set
            {
                if (this.Country != value)
                {
                    this.country = value;
                }
            }
        }

        #endregion

        #region methods

        #region override

        public override string ToString()
        {
            return
                this.Street +
                " " + this.Number +
                ", " + this.Zip +
                " " + this.City +
                " (" + this.Province + ")" +
                " - " + this.Country;
            //return base.ToString();
        }

        #endregion

        #endregion
    }
}