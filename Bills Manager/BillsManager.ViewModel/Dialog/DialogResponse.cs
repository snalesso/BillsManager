using BillsManager.Localization.Attributes;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public class DialogResponse : PropertyChangedBase
    {
        #region ctor
        
        public DialogResponse(ResponseType response, string text, string confirmCheckText)
        {
            this.response = response;
            this.text = text;
            this.confirmCheckText = confirmCheckText;
        }

        public DialogResponse(ResponseType response, string text)
            : this(response, text, null)
        {
        }

        public DialogResponse(ResponseType response)
            : this(ResponseType.Ok, null, null)
        {
        }

        #endregion

        #region properties

        private readonly ResponseType response;
        public ResponseType Response
        {
            get { return response; }
        }

        private readonly string text;
        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(this.text))
                {
                    return this.text;
                }
                else
                {
                    var rtInfo = typeof(ResponseType).GetMember(response.ToString());
                    var attributes = rtInfo[0].GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true);
                    return (attributes[0] as LocalizedDisplayNameAttribute).DisplayName;
                }
            }
        }

        private readonly string confirmCheckText;
        public string ConfirmCheckText
        {
            get { return this.confirmCheckText; }
        }

        public bool UseConfirmCheck
        {
            get { return !string.IsNullOrEmpty(this.ConfirmCheckText); }
        }

        public bool IsDefault { get; set; }

        public bool IsCancel { get; set; }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                if (this.IsEnabled != value)
                {
                    this.isEnabled = value;
                    this.NotifyOfPropertyChange(() => this.IsEnabled);
                }
            }
        }

        #endregion
    }
}