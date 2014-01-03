using BillsManager.ViewModels.Commanding;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class DBAddEditViewModel : Screen
    {
        #region fields

        private readonly IEnumerable<string> takenDBNames;

        #endregion

        #region ctor

        public DBAddEditViewModel(IEnumerable<string> takenDBNames)
        {
            this.takenDBNames = takenDBNames;

            this.DisplayName = "New database"; // TODO: language
        }

        public DBAddEditViewModel(IEnumerable<string> takenDBNames, string oldName)
            : this(takenDBNames)
        {
            if (!string.IsNullOrWhiteSpace(oldName))
            {
                this.NewDBName = oldName;

                this.DisplayName = "Rename database"; // TODO: language
            }
        }

        #endregion

        #region properties

        private string newDBName;
        public string NewDBName
        {
            get { return this.newDBName; }
            set
            {
                if (this.newDBName != value)
                {
                    this.newDBName = value;
                    this.NotifyOfPropertyChange(() => this.NewDBName);
                }
            }
        }
        
        private bool addAndOpen = true;
        public bool AddAndOpen
        {
            get { return this.addAndOpen; }
            set
            {
                if (this.addAndOpen == value) return;

                this.addAndOpen = value;
                this.NotifyOfPropertyChange(() => this.AddAndOpen);
            }
        }

        //public string ForbiddenChars
        //{
        //    get
        //    {
        //        //var idc = System.IO.Path.GetInvalidPathChars();
        //        //var ifc = System.IO.Path.GetInvalidFileNameChars();

        //        //string all = string.Empty;

        //        //idc.Apply(dc =>
        //        //{
        //        //    if (!all.Contains(dc))
        //        //        all += " " + dc;
        //        //});

        //        //ifc.Apply(fc =>
        //        //{
        //        //    if (!all.Contains(fc))
        //        //        all += " " + fc;
        //        //});

        //        //return all;

        //        return @"\ "" < > | : * ? /";
        //    }
        //}

        #endregion

        #region methods

        private bool IsNewValidDBName(string value)
        {
            return
                !string.IsNullOrWhiteSpace(value) &
                !this.takenDBNames.Contains(value, StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region commands

        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                    this.cancelCommand = new RelayCommand(
                        () => this.TryClose(false));

                return this.cancelCommand;
            }
        }

        private RelayCommand confirmCommand;
        public RelayCommand ConfirmCommand
        {
            get
            {
                if (this.confirmCommand == null)
                    this.confirmCommand = new RelayCommand(
                        () => this.TryClose(true),
                        () => this.IsNewValidDBName(this.NewDBName));

                return this.confirmCommand;
            }
        }

        #endregion
    }
}