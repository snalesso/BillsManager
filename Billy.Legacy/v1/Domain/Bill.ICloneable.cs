using System;

namespace BillsManager.v1.Models
{
    public partial class Bill : ICloneable
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