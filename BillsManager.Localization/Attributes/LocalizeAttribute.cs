using System;

namespace BillsManager.Localization.Attributes
{
    // TODO: review modality -> string == null ? use type : use provided key -> .Translation
    [Obsolete]
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    class LocalizeAttribute : Attribute
    {
        public LocalizeAttribute(string key)
        {
            this.key = key;
        }

        private readonly string key;
        public string Key
        {
            get { return this.key; }
        }

        public string Translation
        {
            get
            {
                return TranslationManager.Instance.Translate(this.Key) as string;
            }
        }
    }
}