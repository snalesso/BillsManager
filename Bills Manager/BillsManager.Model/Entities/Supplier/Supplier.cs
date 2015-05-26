namespace BillsManager.Models
{
    public partial class Supplier
    {
        #region ctor

        public Supplier(uint id)
            : this(
            id,
            null,
            null,
            null,
            null,
            null,
            null,
            "Italia",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null)
        {
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

        public string Name { get; set; }

        public string AgentName { get; set; }

        public string AgentSurname { get; set; }

        public string AgentPhone { get; set; }

        public string eMail { get; set; }

        public string Website { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Notes { get; set; }

        #region address

        public string Street { get; set; }

        public string Number { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        #endregion

        #endregion
    }
}