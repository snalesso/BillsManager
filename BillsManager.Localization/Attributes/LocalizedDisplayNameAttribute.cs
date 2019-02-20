using System;
using System.ComponentModel;

namespace BillsManager.Localization.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string resourceName;
        public LocalizedDisplayNameAttribute(string resourceName)
            : base()
        {
            this.resourceName = resourceName;
        }

        public override string DisplayName
        {
            get
            {
                return TranslationManager.Instance.Translate(this.resourceName) as string;
            }
        }
    }
}