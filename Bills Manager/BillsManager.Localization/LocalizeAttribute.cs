using System;

namespace BillsManager.Localization
{
    // TODO: review modality -> string == null ? use type : use provided key -> .Translation
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class LocalizeAttribute : Attribute
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
    }
}