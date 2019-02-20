using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BillsManager.Localization.Attributes
{
    public class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        public LocalizedStringLengthAttribute(int maximumLength) :
            base(maximumLength)
        {
            TranslationManager.Instance.LanguageChanged +=
                (s, e) =>
                {
                    this.UpdateErrorMessageFormat();
                };
        }

        private string errorMessageFormatKey;
        public string ErrorMessageFormatKey
        {
            get
            {
                return this.errorMessageFormatKey;
            }
            set
            {
                this.errorMessageFormatKey = value;
                this.UpdateErrorMessageFormat();
            }
        }

        private void UpdateErrorMessageFormat()
        {
            this.ErrorMessage = string.Format(TranslationManager.Instance.Translate(this.ErrorMessageFormatKey) as string, this.MinimumLength, this.MaximumLength);
        }
    }
}