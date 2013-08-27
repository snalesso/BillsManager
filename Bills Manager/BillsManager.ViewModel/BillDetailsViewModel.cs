using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BillsManager.Model;

namespace BillsManager.ViewModel
{
    public class BillDetailsViewModel : BillViewModel
    {
        #region ctor

        //public BillDetailsViewModel(Bill bill)
        //{
        //    this.ExposedBill = bill;
        //}

        #endregion

        #region properties

        public TimeSpan RemainingTime
        {
            get
            {
                return this.DueDate.Subtract(DateTime.Today);
            }
        }

        #endregion
    }
}
