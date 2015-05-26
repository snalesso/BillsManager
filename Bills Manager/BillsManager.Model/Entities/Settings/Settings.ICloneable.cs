using System;
using System.Globalization;

namespace BillsManager.Models
{
    public partial class Settings : ICloneable
    {
        #region ICloneable Members

        public object Clone()
        {
            return 
                new Settings(
                    (CultureInfo)this.Language.Clone(), 
                    this.StartupDBLoad, 
                    this.FeedbackToEmailAddress);
        }

        #endregion
    }
}