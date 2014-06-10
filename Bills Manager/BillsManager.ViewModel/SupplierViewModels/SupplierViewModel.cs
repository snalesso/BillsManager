﻿using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public abstract class SupplierViewModel : Screen
    {
        #region properties

        protected Supplier exposedSupplier;
        public Supplier ExposedSupplier
        {
            get { return this.exposedSupplier; }
            protected set
            {
                if (this.exposedSupplier != value)
                {
                    this.exposedSupplier = value;
                    this.NotifyOfPropertyChange(() => this.ExposedSupplier);
                }
            }
        }

        #region wrapped from supplier

        public uint ID
        {
            get { return this.ExposedSupplier.ID; }
        }

        public virtual string Name
        {
            get { return this.ExposedSupplier.Name; }
            set
            {
                if (this.Name != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Name = value;
                    this.NotifyOfPropertyChange(() => this.Name);
                }
            }
        }

        public virtual string eMail
        {
            get { return this.ExposedSupplier.eMail; }
            set
            {
                if (this.eMail != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.eMail = value;
                    this.NotifyOfPropertyChange(() => this.eMail);
                }
            }
        }

        public virtual string Website
        {
            get { return this.ExposedSupplier.Website; }
            set
            {
                if (this.Website != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Website = value;
                    this.NotifyOfPropertyChange(() => this.Website);
                }
            }
        }

        /*public ObservableCollection<Agent> Agents
        {
            get
            {
                if (this.ExposedSupplier.Agents == null) this.ExposedSupplier.Agents = new ObservableCollection<Agent>();
                return this.ExposedSupplier.Agents;
            }
            set
            {
                if (this.ExposedSupplier.Agents != value)
                {
                    this.ExposedSupplier.Agents = value;
                    this.NotifyOfPropertyChange(() => this.Agents);
                }
            }
        }*/

        public virtual string Phone
        {
            get { return this.ExposedSupplier.Phone; }
            set
            {
                if (this.Phone != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Phone = value;
                    this.NotifyOfPropertyChange(() => this.Phone);
                }
            }
        }

        public virtual string Fax
        {
            get { return this.ExposedSupplier.Fax; }
            set
            {
                if (this.Fax == value) return;

                if (value == string.Empty)
                    value = null;
                this.ExposedSupplier.Fax = value;
                this.NotifyOfPropertyChange(() => this.Fax);
            }
        }

        public virtual string Notes
        {
            get { return this.ExposedSupplier.Notes; }
            set
            {
                if (this.Notes != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Notes = value;
                    this.NotifyOfPropertyChange(() => this.Notes);
                }
            }
        }

        public virtual string AgentName
        {
            get { return this.ExposedSupplier.AgentName; }
            set
            {
                if (this.AgentName != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.AgentName = value;
                    this.NotifyOfPropertyChange(() => this.AgentName);
                }
            }
        }

        public virtual string AgentSurname
        {
            get { return this.ExposedSupplier.AgentSurname; }
            set
            {
                if (this.AgentSurname != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.AgentSurname = value;
                    this.NotifyOfPropertyChange(() => this.AgentSurname);
                }
            }
        }

        public virtual string AgentPhone
        {
            get { return this.ExposedSupplier.AgentPhone; }
            set
            {
                if (this.AgentPhone != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.AgentPhone = value;
                    this.NotifyOfPropertyChange(() => this.AgentPhone);
                }
            }
        }

        #region address

        /*public Address Address
        {
            get
            {
                //if (this.address == null) this.address = new Address();
                // TODO: review
                return this.ExposedSupplier.Address;
            }
            set
            {
                if (this.ExposedSupplier.Address != value)
                {
                    this.ExposedSupplier.Address = value;
                    this.NotifyOfPropertyChange(() => this.Address);
                }
            }
        }*/

        public virtual string Street
        {
            get { return this.ExposedSupplier.Street; }
            set
            {
                if (this.Street != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Street = value;
                    this.NotifyOfPropertyChange(() => this.Street);
                }
            }
        }

        public virtual string Number
        {
            get { return this.ExposedSupplier.Number; }
            set
            {
                if (this.Number != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Number = value;
                    this.NotifyOfPropertyChange(() => this.Number);
                }
            }
        }

        public virtual string City
        {
            get { return this.ExposedSupplier.City; }
            set
            {
                if (this.City != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.City = value;
                    this.NotifyOfPropertyChange(() => this.City);
                }
            }
        }

        public virtual string Zip
        {
            get { return this.ExposedSupplier.Zip; }
            set
            {
                if (this.Zip == value) return;

                if (value == string.Empty)
                    value = null;

                this.ExposedSupplier.Zip = value;
                this.NotifyOfPropertyChange(() => this.Zip);
            }
        }

        public virtual string Province
        {
            get { return this.ExposedSupplier.Province; }
            set
            {
                if (this.Province == value) return;

                if (value == string.Empty)
                    value = null;

                this.ExposedSupplier.Province = value;
                this.NotifyOfPropertyChange(() => this.Province);
            }
        }

        public virtual string Country
        {
            get { return this.ExposedSupplier.Country; }
            set
            {
                if (this.Country != value)
                {
                    if (value == string.Empty)
                        value = null;
                    this.ExposedSupplier.Country = value;
                    this.NotifyOfPropertyChange(() => this.Country);
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}