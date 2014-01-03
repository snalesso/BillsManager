using BillsManager.Services.Reporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BillsManager.ViewModels.Reporting
{
    // TODO: review base class
    public class BillReportViewModel
    {
        #region fields

        private readonly BillDetailsViewModel billDetailsViewModel;

        #endregion

        #region ctor

        public BillReportViewModel(BillDetailsViewModel billDetailsViewModel)
        {
            this.billDetailsViewModel = billDetailsViewModel;
        }

        #endregion

        #region properties

        [DisplayName("Supplier")]
        public string SupplierName
        {
            get { return this.billDetailsViewModel.SupplierName; }
        }

        [DisplayName("Code")]
        [TextAlignment(TextAlignment.Center)]
        public string Code
        {
            get
            {
                if (this.billDetailsViewModel.Code != null)
                    return this.billDetailsViewModel.Code;
                return string.Empty;
            }
        }

        [DisplayName("Amount")]
        [TextAlignment(TextAlignment.Right)]
        public string Amount
        {
            get { return string.Format("{0:N2} €", this.billDetailsViewModel.Amount); }
        }

        [DisplayName("Payment date")]
        [TextAlignment(TextAlignment.Center)]
        public string PaymentDate
        {
            get
            {
                return
                    this.billDetailsViewModel.PaymentDate.HasValue ?
                    this.billDetailsViewModel.PaymentDate.Value.ToShortDateString() :
                    string.Empty;
            }
        }

        [DisplayName("Due date")]
        [TextAlignment(TextAlignment.Center)]
        public string DueDate
        {
            get { return this.billDetailsViewModel.DueDate.ToShortDateString(); }
        }

        [DisplayName("Release date")]
        [TextAlignment(TextAlignment.Center)]
        public string ReleaseDate
        {
            get { return this.billDetailsViewModel.ReleaseDate.ToShortDateString(); }
        }

        #endregion
    }
}