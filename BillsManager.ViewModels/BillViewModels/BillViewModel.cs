using BillsManager.Models;
using Caliburn.Micro;
using System;

namespace BillsManager.ViewModels
{
    public abstract class BillViewModel : Screen
    {
        #region properties

        // TODO: make readonly -> add ctor
        protected Bill exposedBill;
        public Bill ExposedBill
        {
            get { return this.exposedBill; }
            protected set
            {
                if (this.exposedBill != value)
                {
                    this.exposedBill = value;
                    this.NotifyOfPropertyChange();
                    this.Refresh(); /* TODO: this double refreshes this property, and properties that don't need refresh too
                                     * also, it refreshes in the ctor, when the object is created
                                     * and every time the value changes: eg. on edit (?) */
                }
            }
        }

        #region wrapped from bill

        public uint ID
        {
            get { return this.ExposedBill.ID; }
        }

        public virtual uint SupplierID
        {
            get { return this.ExposedBill.SupplierID; }
            set
            {
                if (this.SupplierID != value)
                {
                    this.ExposedBill.SupplierID = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual DateTime RegistrationDate
        {
            get { return this.ExposedBill.RegistrationDate; }
            set
            {
                if (this.RegistrationDate != value)
                {
                    this.ExposedBill.RegistrationDate = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual DateTime DueDate
        {
            get { return this.ExposedBill.DueDate; }
            set
            {
                if (this.DueDate != value)
                {
                    this.ExposedBill.DueDate = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual DateTime ReleaseDate
        {
            get { return this.ExposedBill.ReleaseDate; }
            set
            {
                if (this.ReleaseDate != value)
                {
                    this.ExposedBill.ReleaseDate = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual DateTime? PaymentDate
        {
            get { return this.ExposedBill.PaymentDate; }
            set
            {
                if (this.PaymentDate != value)
                {
                    this.ExposedBill.PaymentDate = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual Double Amount
        {
            get { return this.ExposedBill.Amount; }
            set
            {
                if (this.Amount != value)
                {
                    this.ExposedBill.Amount = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual Double Agio
        {
            get { return this.ExposedBill.Agio; }
            set
            {
                if (this.ExposedBill.Agio == value) return;

                this.ExposedBill.Agio = value;
                this.NotifyOfPropertyChange();
            }
        }

        public virtual Double AdditionalCosts
        {
            get { return this.ExposedBill.AdditionalCosts; }
            set
            {
                if (this.ExposedBill.AdditionalCosts == value) return;

                this.ExposedBill.AdditionalCosts = value;
                this.NotifyOfPropertyChange();
            }
        }

        public virtual string Code
        {
            get { return this.ExposedBill.Code; }
            set
            {
                if (this.Code != value)
                {
                    this.ExposedBill.Code = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        public virtual string Notes
        {
            get { return this.ExposedBill.Notes; }
            set
            {
                if (this.Notes != value)
                {
                    this.ExposedBill.Notes = value;
                    this.NotifyOfPropertyChange();
                }
            }
        }

        #endregion

        #endregion
    }
}