using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsManager.Model
{
    public partial class Supplier : ICloneable
    {
        #region ICloneable

        #region methods

        public object Clone()
        {

            Supplier suppCopy = (Supplier)this.MemberwiseClone();

            //suppCopy.Address = (Address)this.Address.Clone();

            return suppCopy;
        }

        #endregion

        #endregion
    }
}
