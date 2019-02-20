using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BillsManager.Localization.Attributes
{
    //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class LocalizedRequiredAttribute : RequiredAttribute
    //public class LocalizedRequiredAttribute : ValidationAttribute
    {
        public LocalizedRequiredAttribute()
        {
            //this.UpdateErrorMessage(); -> sends validationRulesTracker to null, haven't checked why

            TranslationManager.Instance.LanguageChanged +=
                (s, e) =>
                {
                    this.UpdateErrorMessage();
                };
        }

        //public bool AllowEmptyStrings { get; set; }

        private string errorMessageKey;
        public string ErrorMessageKey
        {
            get
            {
                return this.errorMessageKey;
            }
            set
            {
                this.errorMessageKey = value;
                this.UpdateErrorMessage();
            }
        }

        private void UpdateErrorMessage()
        {
            this.ErrorMessage = TranslationManager.Instance.Translate(this.ErrorMessageKey) as string;
        }

        //public override bool IsValid(object value)
        //{
        //    if (value == null)
        //        return false;

        //    var stringValue = value as string;
        //    if (stringValue != null && !AllowEmptyStrings)
        //    {
        //        return stringValue.Trim().Length != 0;
        //    }

        //    return true;
        //}
    }
}