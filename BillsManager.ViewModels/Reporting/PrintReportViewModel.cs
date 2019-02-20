using BillsManager.Localization;
using BillsManager.Services.Reporting;
using BillsManager.ViewModels.Commanding;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace BillsManager.ViewModels
{
    public class PrintReportViewModel/*<T>*/ : Screen
    /*where T : class*/
    {
        #region fields

        private readonly ReportPrinter<BillReportViewModel> reportPrinter;
        private readonly Func<IEnumerable<BillReportViewModel>, ReportPrinter<BillReportViewModel>> reportPrinterFactory;

        #endregion

        #region ctor

        public PrintReportViewModel(
            Func<IEnumerable<BillReportViewModel>, ReportPrinter<BillReportViewModel>> reportPrinterFactory,
            IEnumerable<BillReportViewModel> billReportList,
            string header,
            string comment)
        {
            this.reportPrinterFactory = reportPrinterFactory;

            this.reportPrinter = reportPrinterFactory.Invoke(billReportList);
            this.reportPrinter.Header = header;
            this.reportPrinter.Comment = comment;

            this.DisplayName = TranslationManager.Instance.Translate("PrintReport").ToString();

            //this.Deactivated +=
            //    (s, e) =>
            //    {
            //        if (e.WasClosed)
            //            this.reportPrinterFactory.Dispose(); // TODO: remember to dispose dbconnector
            //    };

            this.GoToFirstPage();
        }

        #endregion

        #region properties

        public string Header
        {
            get { return this.reportPrinter.Header; }
            set
            {
                if (this.reportPrinter.Header == value) return;

                this.reportPrinter.Header = value;
                this.NotifyOfPropertyChange(() => this.Header);

                if (this.CurrentPageNumber == 1)
                    this.NotifyOfPropertyChange(() => this.CurrentPage);
            }
        }

        private int currentPageNumber;
        public int CurrentPageNumber
        {
            get { return this.currentPageNumber; }
            set
            {
                if (value < 1)
                    value = 1;
                else if (value > this.reportPrinter.PageCount)
                    value = this.reportPrinter.PageCount;

                if (this.currentPageNumber == value) return;

                this.currentPageNumber = value;
                this.NotifyOfPropertyChange(() => this.CurrentPageNumber);
                this.NotifyOfPropertyChange(() => this.CurrentPage);
                this.NotifyOfPropertyChange(() => this.CanGoToPreviousPage);
                this.NotifyOfPropertyChange(() => this.CanGoToNextPage);
            }
        }

        public int PageCount
        {
            get
            {
                return this.reportPrinter.PageCount;
            }
        }

        public Visual CurrentPage
        {
            get
            {
                return this.reportPrinter.GetPage(this.CurrentPageNumber - 1).Visual;
            }
        }

        #region page navigation

        public bool CanGoToPreviousPage
        {
            get { return this.CurrentPageNumber > 1; }
        }

        public bool CanGoToNextPage
        {
            get
            {
                var can = (this.CurrentPageNumber < this.reportPrinter.PageCount);
                return can;
            }
        }

        #endregion

        #endregion

        #region methods

        #region page navigation

        private void GoToPreviousPage()
        {
            this.CurrentPageNumber--;
        }

        private void GoToNextPage()
        {
            this.CurrentPageNumber++;
        }

        private void GoToFirstPage()
        {
            this.CurrentPageNumber = 1;
        }

        private void GoToLastPage()
        {
            this.CurrentPageNumber = this.reportPrinter.PageCount;
        }

        public void Print()
        {
            new PrintDialog().PrintDocument(this.reportPrinter, "Bills Manager Report");
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand goToFirstPageCommand;
        public RelayCommand GoToFirstPageCommand
        {
            get
            {
                return this.goToFirstPageCommand ?? (this.goToFirstPageCommand = 
                    new RelayCommand(
                        () => this.GoToFirstPage(),
                        () => this.CanGoToPreviousPage));
            }
        }

        private RelayCommand goToPreviousPageCommand;
        public RelayCommand GoToPreviousPageCommand
        {
            get
            {
                return this.goToPreviousPageCommand ?? (this.goToPreviousPageCommand = 
                    new RelayCommand(
                        () => this.GoToPreviousPage(),
                        () => this.CanGoToPreviousPage));
            }
        }

        private RelayCommand goToNextPageCommand;
        public RelayCommand GoToNextPageCommand
        {
            get
            {
                return this.goToNextPageCommand ?? (this.goToNextPageCommand = 
                    new RelayCommand(
                        () => this.GoToNextPage(),
                        () => this.CanGoToNextPage));
            }
        }

        private RelayCommand goToLastPageCommand;
        public RelayCommand GoToLastPageCommand
        {
            get
            {
                return this.goToLastPageCommand ?? (this.goToLastPageCommand = 
                    new RelayCommand(
                        () => this.GoToLastPage(),
                        () => this.CanGoToNextPage));
            }
        }

        private RelayCommand printCommand;
        public RelayCommand PrintCommand
        {
            get
            {
                return this.printCommand ?? (this.printCommand = 
                    new RelayCommand(
                        () => this.Print()));
            }
        }

        #endregion
    }
}