using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BillsManager.Model
{
    // TODO: other properties: P. IVA (VATIN)? FAX (FAX)?
    public partial class Supplier
    {
        #region ctor

        public Supplier(uint id)
        {
            this.id = id;
        }

        public Supplier(
            uint id,
            string name,
            string street,
            string number,
            string city,
            ushort zip,
            string province,
            string country,
            string eMail,
            string webSite,
            string phone,
            string notes)
        {
            this.id = id;

            this.Name = name;
            this.eMail = eMail;
            this.Website = webSite;
            this.Phone = phone;
            this.Notes = notes;

            // Address
            this.Street = street;
            this.Number = number;
            this.City = city;
            this.Zip = zip;
            this.Province = province;
            this.Country = country;
        }

        #endregion

        #region properties

        private readonly uint id;
        public uint ID
        {
            get { return this.id; }
        }

        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.Name != value)
                {
                    this.name = value;
                }
            }
        }

        //private Address address; // TODO: AgentViewModel con validazione dell'indirizzo
        //public Address Address
        //{
        //    get
        //    {
        //        if (this.address == null) this.address = new Address(); // TODO: review
        //        return this.address;
        //    }
        //    set
        //    {
        //        if (this.Address != value)
        //        {
        //            this.address = value;
        //        }
        //    }
        //}

        private string email;
        public string eMail
        {
            get
            {
                return this.email;
            }
            set
            {
                if (this.eMail != value)
                {
                    this.email = value;
                }
            }
        }

        private string webSite;
        public string Website
        {
            get
            {
                return this.webSite;
            }
            set
            {
                if (this.Website != value)
                {
                    this.webSite = value;
                }
            }
        }

        private string phone;
        public string Phone
        {
            get
            {
                return this.phone;
            }
            set
            {
                if (this.phone != value)
                {
                    this.phone = value;
                }
            }
        }

        private string notes;
        public string Notes
        {
            get
            {
                return this.notes;
            }
            set
            {
                if (this.Notes != value)
                {
                    this.notes = value;
                }
            }
        }

        #region address

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

        private ushort zip;
        public ushort Zip
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

        #endregion
    }
}