using BillsManager.Localization.Attributes;
using BillsManager.Services.Reporting;
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

        [LocalizedDisplayName("Supplier")]
        public string SupplierName
        {
            get { return this.billDetailsViewModel.SupplierName; }
        }

        [LocalizedDisplayName("Code")]
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

        [LocalizedDisplayName("Amount")]
        [TextAlignment(TextAlignment.Right)]
        public string Amount
        {
            get { return string.Format("{0:N2} €", this.billDetailsViewModel.Amount); }
        }

        [LocalizedDisplayName("Agio")]
        [TextAlignment(TextAlignment.Right)]
        public string Agio
        {
            get { return string.Format("{0:N2} €", this.billDetailsViewModel.Agio); }
        }

        [LocalizedDisplayName("DueDate")]
        [TextAlignment(TextAlignment.Center)]
        public string DueDate
        {
            get { return this.billDetailsViewModel.DueDate.ToShortDateString(); }
        }

        [LocalizedDisplayName("ReleaseDate")]
        [TextAlignment(TextAlignment.Center)]
        public string ReleaseDate
        {
            get { return this.billDetailsViewModel.ReleaseDate.ToShortDateString(); }
        }

        #endregion
    }
}