using System;

namespace BillsManager.Model
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