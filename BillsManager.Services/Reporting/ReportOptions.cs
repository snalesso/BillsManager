using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BillsManager.Services.Reporting
{
    public class ReportOptions
    {
        public ReportOptions(
            Size pageSize,
            Thickness pageMargins,
            Brush fontColor)
        {
            this.pageSize = pageSize;
            this.pageMargins = pageMargins;
            this.fontColor = fontColor;
        }

        #region page properties

        private readonly Size pageSize;
        public Size PageSize
        {
            get { return this.pageSize; }
        }

        private readonly Thickness pageMargins;
        public Thickness PageMargins
        {
            get { return this.pageMargins; }
        }

        private readonly Brush fontColor;
        public Brush FontColor
        {
            get { return this.fontColor; }
        }

        #endregion

        #region font sizes

        private readonly double headerFontSize;
        public double HeaderFontSize
        {
            get { return this.headerFontSize; }
        }

        private readonly double columnHeaderFontSize;
        public double ColumnHeaderFontSize
        {
            get { return this.columnHeaderFontSize; }
        }

        private readonly double rowItemFontSize;
        public double RowItemFontSize
        {
            get { return this.rowItemFontSize; }
        }

        #endregion
    }
}