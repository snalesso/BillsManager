using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel;

namespace BillsManager.ViewModel
{
    public partial class BillViewModel : Screen, IDataErrorInfo
    {
        #region IDataErrorInfo

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
