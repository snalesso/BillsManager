namespace BillsManager.Models
{
    // TODO: other properties: P. IVA (VATIN)? http://en.wikipedia.org/wiki/VAT_identification_number
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
            string zip,
            string province,
            string country,
            string eMail,
            string webSite,
            string phone,
            string fax,
            string notes,
            string agentName,
            string agentSurname,
            string agentPhone)
        {
            this.id = id;

            this.Name = name;
            this.eMail = eMail;
            this.Website = webSite;
            this.Phone = phone;
            this.Fax = fax;
            this.Notes = notes;

            this.AgentName = agentName;
            this.AgentSurname = agentSurname;
            this.AgentPhone = agentPhone;

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
        
        private string agentName;
        public string AgentName
        {
            get { return this.agentName; }
            set
            {
                if (this.agentName != value)
                {
                    this.agentName = value;
                }
            }
        }
        
        private string agentSurname;
        public string AgentSurname
        {
            get { return this.agentSurname; }
            set
            {
                if (this.agentSurname != value)
                {
                    this.agentSurname = value;
                }
            }
        }
                
        private string agentPhone;
        public string AgentPhone
        {
            get { return this.agentPhone; }
            set
            {
                if (this.agentPhone != value) // TODO: cleanup worthless if's
                {
                    this.agentPhone = value;
                }
            }
        }

        private string email;
        public string eMail
        {
            get
            {
                return this.email;
            }
            set
            {
                if (this.eMail == value) return;

                this.email = value.ToLower();
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
                if (this.Website == value) return;

                this.webSite = value.ToLower();
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
        
        private string fax;
        public string Fax
        {
            get { return this.fax; }
            set
            {
                if (this.fax == value) return;

                this.fax = value;
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

        private string zip;
        public string Zip
        {
            get { return this.zip; }
            set
            {
                if (this.Zip == value) return;

                this.zip = value;
            }
        }

        private string province;
        public string Province
        {
            get { return this.province; }
            set
            {
                if (this.Province == value) return;

                this.province = value.ToUpper();
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