using System;

namespace BillsManager.Localization
{
    public class BillsManagerTranslationDictionary : SimpleTranslationDictionary, IBillsManagerTranslationDictionary
    {
        public BillsManagerTranslationDictionary(string translationDictionaryFilePath) : base(translationDictionaryFilePath) { }

        public string Ok
        {
            get { return base.GetTranslation(); }
        }

        public string Cancel
        {
            get { return base.GetTranslation(); }
        }

        public string Exit
        {
            get { return base.GetTranslation(); }
        }

        public string Connect
        {
            get { return base.GetTranslation(); }
        }

        public string Disconnect
        {
            get { return base.GetTranslation(); }
        }

        public string Open
        {
            get { return base.GetTranslation(); }
        }

        public string Close
        {
            get { return base.GetTranslation(); }
        }

        public string Supplier
        {
            get { return base.GetTranslation(); }
        }

        public string Bill
        {
            get { return base.GetTranslation(); }
        }

        public string Suppliers
        {
            get { return base.GetTranslation(); }
        }

        public string Bills
        {
            get { return base.GetTranslation(); }
        }
    }
}