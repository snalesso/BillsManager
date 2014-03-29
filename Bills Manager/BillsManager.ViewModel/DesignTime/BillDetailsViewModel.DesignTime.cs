using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BillsManager.Models;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class BillDetailsViewModel
    {
        #region ctor

        public BillDetailsViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        public BillDetailsViewModel(Bill bill)
        {
            this.ExposedBill = bill;
        }

        #endregion

        #region methods

        void LoadDesignTimeData()
        {
            //var supp = new Supplier(
            //    0,
            //    "Faber-Castell",
            //    "Via Stromboli",
            //    "14",
            //    "Milano",
            //    "20144",
            //    "MI",
            //    "Italia",
            //    "faber-castell@faber-castell.it",
            //    "http://www.faber-castell.it",
            //    "02/43069601",
            //    "sconti 10/06 - 24/09."
            //    //,new[] { new Agent("Barbara", "Robecchi", "347-7892234", string.Empty) }
            //);

            this.ExposedBill = new Bill(
                0,
                //9,
                DateTime.Today.AddDays(-2),
                DateTime.Today.AddDays(14),
                DateTime.Today,
                DateTime.Today.AddDays(-8),
                723.61,
                66,
                /*0,
                4,*/
                "call agent for reduction @additional comments to trigger validation rule",
                "X3V-KDM");

            //this.SupplierName = "Supplier Name";
        }

        #endregion
    } 
#endif
}