using System;

namespace BillsManager.Models
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