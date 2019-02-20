using System;

namespace BillsManager.Models
{
    public partial class Address : ICloneable
    {
        #region ICloneable

        #region methods

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #endregion
    }
}
