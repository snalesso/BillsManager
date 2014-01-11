using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public abstract class BillViewModel : Screen
    {
        #region properties

        protected Bill exposedBill;
        public Bill ExposedBill
        {
            get { return this.exposedBill; }
            protected set
            {
                if (this.exposedBill != value)
                {
                    this.exposedBill = value;
                    this.NotifyOfPropertyChange(() => this.ExposedBill);
                }
            }
        }

        #region wrapped from bill

        public virtual DateTime RegistrationDate
        {
            get { return this.ExposedBill.RegistrationDate; }
            set
            {
                if (this.RegistrationDate != value)
                {
                    this.ExposedBill.RegistrationDate = value;
                    this.NotifyOfPropertyChange(() => this.RegistrationDate);
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
                    this.NotifyOfPropertyChange(() => this.DueDate);
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
                    this.NotifyOfPropertyChange(() => this.PaymentDate);
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
                    this.NotifyOfPropertyChange(() => this.ReleaseDate);
                }
            }
        }

        public virtual Double Amount
        {
            get { return this.ExposedBill.Amount; }
            set
            {
                // TODO: review set logic
                //if (this.Amount != value)
                //{
                    this.ExposedBill.Amount = value;
                    this.NotifyOfPropertyChange(() => this.Amount);
                //}
            }
        }

        public virtual uint SupplierID
        {
            get { return this.ExposedBill.SupplierID; }
            set
            {
                if (this.SupplierID != value)
                {
                    this.ExposedBill.SupplierID = value;
                    this.NotifyOfPropertyChange(() => this.SupplierID);
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
                    this.NotifyOfPropertyChange(() => this.Notes);
                }
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
                    this.NotifyOfPropertyChange(() => this.Code);
                }
            }
        }

        #endregion

        #endregion
    }
}