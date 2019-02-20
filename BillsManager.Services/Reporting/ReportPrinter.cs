using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BillsManager.Services.Reporting
{
    // IDEA: use excel template for sums and other calculations
    public class ReportPrinter<T> : DocumentPaginator
        where T : class
    {
        #region fields

        private readonly PrintDialog printDialog;
        private readonly IEnumerable<T> dataSource; // TODO: convert to ICollection
        private readonly IEnumerable<PropertyInfo> properiesToShow;
        private readonly DateTime now = DateTime.Now;

        private const short SECTION_SEPARATOR_HEIGHT = 11;
        private const short ROW_SEPARATOR_HEIGHT = 7;
        private const short COLUMN_SEPARATOR_WIDTH = 7;
        private const short HEADERS_SEPARATOR_LINE_HEIGHT = 1;

        #endregion

        #region ctor

        public ReportPrinter(
            IEnumerable<T> dataSource)
        {
            this.dataSource = dataSource;

            this.properiesToShow = this.GetProperties();

            this.printDialog = new PrintDialog();

            this.pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            //this.pageSize = new Size(827, 1169);

            this.headerFontSize = 20;
            this.HeaderRenderHeight = this.GetTextRenderHeight(22);

            this.commentFontSize = 17;
            this.CommentRenderHeight = this.GetTextRenderHeight(18);

            this.columnHeaderFontSize = 12;
            this.ColumnHeaderRenderHeight = this.GetTextRenderHeight(12);

            this.rowItemFontSize = 12;
            this.RowItemRenderHeight = this.GetTextRenderHeight(12);

            this.footerFontSize = 12;
            this.FooterRenderHeight = this.GetTextRenderHeight(12);

            this.UpdateCachedValues();
        }

        #endregion

        #region properties

        private Thickness pageMargins = new Thickness(40, 25, 40, 30);
        public Thickness PageMargins
        {
            get { return this.pageMargins; }
            set
            {
                if (this.pageMargins == value) return;
                this.pageMargins = value;
            }
        }

        private int rowsInFirstPage;
        public int RowsInFirstPage
        {
            get { return this.rowsInFirstPage; }
            private set { this.rowsInFirstPage = value; }
        }

        private int rowsInSecondaryPages;
        public int RowsInSecondaryPages
        {
            get { return this.rowsInSecondaryPages; }
            set { this.rowsInSecondaryPages = value; }
        }

        #region header

        private string header;
        public string Header
        {
            get { return this.header; }
            set
            {
                if (this.header == value) return;

                this.header = value;
            }
        }

        private double headerFontSize;
        public double HeaderFontSize
        {
            get { return this.headerFontSize; }
            set
            {
                if (this.headerFontSize == value) return;

                this.headerFontSize = value;

                this.HeaderRenderHeight = this.GetTextRenderHeight(value);
                this.UpdateCachedValues();
            }
        }

        private double headerRenderHeight;
        public double HeaderRenderHeight
        {
            get { return this.headerRenderHeight; }
            private set { this.headerRenderHeight = value; }
        }

        #endregion

        #region comment

        private string comment;
        public string Comment
        {
            get { return this.comment; }
            set
            {
                if (this.comment == value) return;

                this.comment = value;
            }
        }

        private double commentFontSize;
        public double CommentFontSize
        {
            get { return this.commentFontSize; }
            set
            {
                if (this.commentFontSize == value) return;

                this.commentFontSize = value;

                this.CommentRenderHeight = this.GetTextRenderHeight(value);
                this.UpdateCachedValues();
            }
        }

        private double coomentRenderHeight;
        public double CommentRenderHeight
        {
            get { return this.coomentRenderHeight; }
            private set { this.coomentRenderHeight = value; }
        }

        #endregion

        #region column header

        private double columnHeaderFontSize;
        public double ColumnHeaderFontSize
        {
            get { return this.columnHeaderFontSize; }
            set
            {
                if (this.columnHeaderFontSize == value) return;

                this.columnHeaderFontSize = value;

                this.ColumnHeaderRenderHeight = this.GetTextRenderHeight(value);
                this.UpdateCachedValues();
            }
        }

        private double columnheaderRenderHeight;
        public double ColumnHeaderRenderHeight
        {
            get { return this.columnheaderRenderHeight; }
            private set { this.columnheaderRenderHeight = value; }
        }

        #endregion

        #region row item

        private double rowItemFontSize;
        public double RowItemFontSize
        {
            get { return this.rowItemFontSize; }
            set
            {
                if (this.rowItemFontSize == value) return;

                this.rowItemFontSize = value;

                this.RowItemRenderHeight = this.GetTextRenderHeight(value);
                this.UpdateCachedValues();
            }
        }

        private double rowItemRenderHeight;
        public double RowItemRenderHeight
        {
            get { return this.rowItemRenderHeight; }
            set { this.rowItemRenderHeight = value; }
        }

        #endregion

        #region footer

        private double footerFontSize;
        public double FooterFontSize
        {
            get { return this.footerFontSize; }
            set
            {
                if (this.footerFontSize == value) return;

                this.footerFontSize = value;

                this.FooterRenderHeight = this.GetTextRenderHeight(value);
                this.UpdateCachedValues();
            }
        }

        private double footerRenderHeight;
        public double FooterRenderHeight
        {
            get { return this.footerRenderHeight; }
            private set { this.footerRenderHeight = value; }
        }

        #endregion

        #region document paginator

        private int pageCount;
        public override int PageCount
        {
            get { return this.pageCount; }
        }

        private Size pageSize;
        public override Size PageSize
        {
            get { return this.pageSize; }
            set
            {
                if (this.pageSize == value) return;
                this.pageSize = value;

                this.UpdateCachedValues();
            }
        }

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

        #endregion

        #endregion

        #region methods

        public override DocumentPage GetPage(int pageNumber)
        {
            var rootGrid = this.GetRoot();

            var isFirstPage = pageNumber == 0;
            var headers = this.GetColumnHeaders();

            if (isFirstPage)
            {
                // header
                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.HeaderRenderHeight, GridUnitType.Pixel) });
                var header = this.GetHeader();
                Grid.SetRow(header, 0);
                Grid.SetColumnSpan(header, headers.Count * 2);
                rootGrid.Children.Add(header);

                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SECTION_SEPARATOR_HEIGHT, GridUnitType.Pixel) });

                // comment
                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.CommentRenderHeight, GridUnitType.Pixel) });
                var comment = this.GetComment();
                Grid.SetRow(comment, rootGrid.RowDefinitions.Count - 1);
                Grid.SetColumnSpan(comment, headers.Count * 2);
                rootGrid.Children.Add(comment);

                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SECTION_SEPARATOR_HEIGHT, GridUnitType.Pixel) });
            }

            // column headers
            //rootGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.ColumnHeaderRenderHeight, GridUnitType.Pixel) });
            foreach (var header in headers)
            {
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                Grid.SetRow(header, rootGrid.RowDefinitions.Count - 1);
                Grid.SetColumn(header, rootGrid.ColumnDefinitions.Count - 1);
                rootGrid.Children.Add(header);
                rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(COLUMN_SEPARATOR_WIDTH, GridUnitType.Pixel) });
            }
            // remove last column
            rootGrid.ColumnDefinitions.RemoveAt(rootGrid.ColumnDefinitions.Count - 1);

            // column headers separator line
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SECTION_SEPARATOR_HEIGHT, GridUnitType.Pixel) });
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(HEADERS_SEPARATOR_LINE_HEIGHT, GridUnitType.Pixel) });
            var columnHeadersSeparator = this.GetColumnHeadersSepatator();
            Grid.SetRow(columnHeadersSeparator, rootGrid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(columnHeadersSeparator, rootGrid.ColumnDefinitions.Count);
            rootGrid.Children.Add(columnHeadersSeparator);
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SECTION_SEPARATOR_HEIGHT, GridUnitType.Pixel) });

            // items (rows)
            var startIndex = (isFirstPage ? 0 : this.RowsInFirstPage + this.RowsInSecondaryPages * (pageNumber - 1));
            var rowsPrintableInThisPage = isFirstPage ? this.RowsInFirstPage : this.RowsInSecondaryPages;
            //var printCount = (startIndex + rowsPrintableInThisPage <= dataSource.Count ? rowsPrintableInThisPage : dataSource.Count - startIndex);
            //var itemsToDisplay = this.dataSource.ToList().GetRange(startIndex, printCount);
            var endIndex = startIndex + rowsPrintableInThisPage <= this.dataSource.Count() ? startIndex + rowsPrintableInThisPage - 1 : this.dataSource.Count() - 1;

            for (int itemIndex = startIndex; itemIndex <= endIndex; itemIndex++)
            {
                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.RowItemRenderHeight, GridUnitType.Pixel) });

                for (var i = 0; i < this.properiesToShow.Count(); i++)
                {
                    var rowItem = this.GetCellItem(this.dataSource.ElementAt(itemIndex), this.properiesToShow.ElementAt(i));
                    Grid.SetRow(rowItem, rootGrid.RowDefinitions.Count - 1);
                    Grid.SetColumn(rowItem, i * 2);
                    rootGrid.Children.Add(rowItem);
                }

                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(ROW_SEPARATOR_HEIGHT, GridUnitType.Pixel) });
            }

            // a row to fill the empty space between the last row item and the footer
            rootGrid.RowDefinitions.ElementAt(rootGrid.RowDefinitions.Count - 1).Height = new GridLength(1, GridUnitType.Star);

            // footer
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SECTION_SEPARATOR_HEIGHT, GridUnitType.Pixel) });

            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(this.FooterRenderHeight, GridUnitType.Pixel) });
            var footer = this.GetFooter(pageNumber);
            Grid.SetRow(footer, rootGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(footer, 0);
            Grid.SetColumnSpan(footer, rootGrid.ColumnDefinitions.Count);
            rootGrid.Children.Add(footer);

            // prepare rendering
            rootGrid.Measure(this.PageSize);
            rootGrid.Arrange(new Rect(new Point(0, 0), rootGrid.DesiredSize));
            rootGrid.UpdateLayout();

            double actualHeadersTotalWidth = 0;
            foreach (var column in rootGrid.ColumnDefinitions)
            {
                if (rootGrid.ColumnDefinitions.IndexOf(column) % 2 == 0)
                    actualHeadersTotalWidth += column.ActualWidth;
            }

            foreach (var column in rootGrid.ColumnDefinitions)
            {
                if (rootGrid.ColumnDefinitions.IndexOf(column) % 2 == 0)
                    column.Width =
                        new GridLength(column.ActualWidth / actualHeadersTotalWidth, GridUnitType.Star);
            }

            // prepare rendering
            rootGrid.Measure(this.PageSize);
            rootGrid.Arrange(new Rect(new Point(0, 0), rootGrid.DesiredSize));
            rootGrid.UpdateLayout();

            var dp = new DocumentPage(rootGrid);

            return dp;
        }

        #region region creators

        private Grid GetRoot()
        {
            var root = new Grid()
            {
                //Background = Brushes.LightCoral,
                Margin = this.PageMargins,
                Height = this.PageSize.Height - this.PageMargins.Top - this.PageMargins.Bottom, // TODO: cache available space
                Width = this.PageSize.Width - this.PageMargins.Left - this.PageMargins.Right,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            TextOptions.SetTextFormattingMode(root, TextFormattingMode.Ideal);
            TextOptions.SetTextHintingMode(root, TextHintingMode.Fixed);
            TextOptions.SetTextRenderingMode(root, TextRenderingMode.Auto);

            return root;
        }

        private TextBlock GetHeader()
        {
            return new TextBlock()
            {
                Text = this.Header,
                FontSize = this.HeaderFontSize,
                FontWeight = FontWeights.Medium,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private TextBlock GetComment()
        {
            return new TextBlock()
            {
                Text = this.Comment,
                FontWeight = FontWeights.Normal,
                FontSize = this.CommentFontSize,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private ICollection<TextBlock> GetColumnHeaders()
        {
            var columnHeaders = new List<TextBlock>();

            foreach (var pi in this.properiesToShow)
            {
                var text = (Attribute.GetCustomAttribute(pi, typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName;
                var alignmAtt = Attribute.GetCustomAttribute(pi, typeof(TextAlignmentAttribute)) as TextAlignmentAttribute;
                var alignment = alignmAtt != null ? alignmAtt.Alignment : TextAlignment.Left;

                columnHeaders.Add(
                    new TextBlock()
                    {
                        Text = text,
                        TextAlignment = alignment,
                        FontSize = this.ColumnHeaderFontSize,
                        FontWeight = FontWeights.Medium,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    });
            }

            return columnHeaders;
        }

        private Rectangle GetColumnHeadersSepatator()
        {
            return new Rectangle()
            {
                Fill = Brushes.Black,
                Height = HEADERS_SEPARATOR_LINE_HEIGHT,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        private TextBlock GetCellItem(T rowItem, PropertyInfo property)
        {
            var text = property.GetValue(rowItem).ToString();
            var alignmAtt = Attribute.GetCustomAttribute(property, typeof(TextAlignmentAttribute)) as TextAlignmentAttribute;
            var alignment = alignmAtt != null ? alignmAtt.Alignment : TextAlignment.Left;

            return new TextBlock()
            {
                Text = text,
                TextAlignment = alignment,
                Foreground = Brushes.Black,
                FontSize = this.RowItemFontSize,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }

        private Grid GetFooter(int pageIndex)
        {
            var footer = new Grid();

            // time info
            footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            var timeInfo = new TextBlock()
            {
                Foreground = Brushes.Black,
                Text = this.now.ToShortDateString() + " - " + this.now.ToShortTimeString(),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(timeInfo, footer.ColumnDefinitions.Count - 1);
            footer.Children.Add(timeInfo);

            // empty space
            footer.ColumnDefinitions.Add(new ColumnDefinition());

            // page info
            footer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            var pageInfo = new TextBlock()
            {
                Foreground = Brushes.Black,
                Text = (pageIndex + 1) + @"/" + this.PageCount,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(pageInfo, footer.ColumnDefinitions.Count - 1);
            footer.Children.Add(pageInfo);

            return footer;
        }

        #endregion

        #region support

        private ICollection<PropertyInfo> GetProperties()
        {
            return typeof(T)
                .GetProperties()
                .Where(pi => Attribute.IsDefined(pi, typeof(DisplayNameAttribute)))
                .ToList();
        }

        private double GetTextRenderHeight(double fontSize)
        {
            var txb = new TextBlock() { Text = "|", FontSize = fontSize };
            txb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //txt.Arrange(new Rect(new Point(0, 0), this.PageSize));
            return txb.DesiredSize.Height;
        }

        private void UpdateCachedValues()
        {
            this.UpdateRowsInFirstPage();
            this.UpdateRowsInSecondaryPages();
            this.UpdatePageCount();
        }

        private int RowsInPage(bool isFirstPage)
        {
            var spaceForRows = this.PageSize.Height - this.PageMargins.Top - this.PageMargins.Bottom;

            if (isFirstPage)
            {
                // header + separator space
                spaceForRows -= this.HeaderRenderHeight;
                spaceForRows -= SECTION_SEPARATOR_HEIGHT;
                // comment + separator space
                spaceForRows -= this.CommentRenderHeight;
                spaceForRows -= SECTION_SEPARATOR_HEIGHT;
            }

            // column headers + separator space
            spaceForRows -= this.ColumnHeaderRenderHeight;
            spaceForRows -= SECTION_SEPARATOR_HEIGHT;
            spaceForRows -= HEADERS_SEPARATOR_LINE_HEIGHT;
            spaceForRows -= SECTION_SEPARATOR_HEIGHT;

            // rows here

            // separator + footer space
            spaceForRows -= SECTION_SEPARATOR_HEIGHT;
            spaceForRows -= this.FooterRenderHeight;

            // space for rows calculated

            var rowWithSeparatorHeight = this.RowItemRenderHeight + ROW_SEPARATOR_HEIGHT;

            var remainingSpace = spaceForRows % rowWithSeparatorHeight;

            var rowsCount = Math.Truncate(spaceForRows / rowWithSeparatorHeight);

            if (remainingSpace >= this.RowItemRenderHeight)
                rowsCount++;

            return (int)Math.Floor(rowsCount);
        }

        private void UpdateRowsInFirstPage()
        {
            this.rowsInFirstPage = this.RowsInPage(true);
        }

        private void UpdateRowsInSecondaryPages()
        {
            this.rowsInSecondaryPages = this.RowsInPage(false);
        }

        private void UpdatePageCount()
        {
            double dataCount = this.dataSource.Count();
            double rowsInOtherPages = dataCount - this.rowsInFirstPage;
            double otherPagesCount = rowsInOtherPages / this.RowsInSecondaryPages;

            this.pageCount = 1 + (int)Math.Ceiling(otherPagesCount);
        }

        #endregion

        #endregion
    }
}