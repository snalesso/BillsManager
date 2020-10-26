using Billy.Domain.Models;

namespace Billy.Billing.Models
{
    public class Supplier : Entity<long>//, ISupplier
    {
        #region ctor

        public Supplier(
            long id,
            string name,
            string eMail = null,
            string webSite = null,
            string phone = null,
            string fax = null,
            string notes = null,
            Address address = null,
            Agent agent = null)
            : base(id)
        {
            this.Name = name;
            this.Email = eMail;
            this.Website = webSite;
            this.Phone = phone;
            this.Fax = fax;
            this.Notes = notes;

            this.Address = address;
            this.Agent = agent;
        }

        #endregion

        #region properties

        private string _name;
        // TODO: validate name change
        public string Name
        {
            get { return this._name; }
            set { this.SetAndRaiseIfChanged(ref this._name, value); }
        }

        private Agent _agent;
        public Agent Agent
        {
            get { return this._agent; }
            set { this.SetAndRaiseIfChanged(ref this._agent, value); }
        }

        private string _email;
        public string Email
        {
            get { return this._email; }
            set { this.SetAndRaiseIfChanged(ref this._email, value); }
        }

        private string _website;
        public string Website
        {
            get { return this._website; }
            set { this.SetAndRaiseIfChanged(ref this._website, value); }
        }

        private string _phone;
        public string Phone
        {
            get { return this._phone; }
            set { this.SetAndRaiseIfChanged(ref this._phone, value); }
        }

        private string _fax;
        public string Fax
        {
            get { return this._fax; }
            set { this.SetAndRaiseIfChanged(ref this._fax, value); }
        }

        private string _notes;
        public string Notes
        {
            get { return this._notes; }
            set { this.SetAndRaiseIfChanged(ref this._notes, value); }
        }

        private Address _address;
        public Address Address
        {
            get { return this._address; }
            set { this.SetAndRaiseIfChanged(ref this._address, value); }
        }

        #endregion
    }
}