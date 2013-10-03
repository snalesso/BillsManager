using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public class DialogResponse : PropertyChangedBase
    {
        #region ctor

        // TODO: review ctors gerarchy

        public DialogResponse(ResponseType response, string text, string confirmCheckText)
            : this(response, text)
        {
            this.ConfirmCheckText = confirmCheckText;
        }

        public DialogResponse(ResponseType response, string text, ushort timer)
            : this(response, text)
        {
            this.Timer = timer;
        }

        public DialogResponse(ResponseType response, string text)
            : this(response)
        {
            this.Text = text;
        }

        public DialogResponse(ResponseType response, ushort timer)
            : this(response)
        {
            this.Timer = timer;
        }

        public DialogResponse(ResponseType response)
        {
            this.Response = response;
            this.Text = response.ToString();
        }

        #endregion

        #region properties

        private ResponseType response;
        public ResponseType Response
        {
            get { return response; }
            protected set
            {
                if (this.response != value)
                {
                    response = value;
                    this.NotifyOfPropertyChange(() => this.Response);
                }
            }
        }

        private string text;
        public string Text
        {
            get
            {
                if (this.Timer == 0) return this.text;

                return this.text + " (" + this.Timer + ")";
            }
            protected set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.NotifyOfPropertyChange(() => this.Text);
                }
            }
        }

        private ushort timer;
        public ushort Timer
        {
            get { return this.timer; }
            set
            {
                if (this.timer != value)
                {
                    this.timer = value;
                    this.NotifyOfPropertyChange(() => this.Text);
                    this.IsEnabled = (this.Timer <= 0);
                }
            }
        }
                
        private string confirmCheckText;
        public string ConfirmCheckText
        {
            get { return this.confirmCheckText; }
            set
            {
                if (this.confirmCheckText != value)
                {
                    this.confirmCheckText = value;
                    this.NotifyOfPropertyChange(() => this.ConfirmCheckText);
                    this.NotifyOfPropertyChange(() => this.UseConfirmCheck);
                    this.IsEnabled = !this.UseConfirmCheck;
                }
            }
        }
        
        public bool UseConfirmCheck
        {
            get { return !string.IsNullOrEmpty(this.ConfirmCheckText); }
        }

        private bool isDefault;
        public bool IsDefault
        {
            get { return this.isDefault; }
            set
            {
                if (this.isDefault != value)
                {
                    this.isDefault = value;
                    this.NotifyOfPropertyChange(() => this.IsDefault);
                }
            }
        }

        private bool isCancel;
        public bool IsCancel
        {
            get { return this.isCancel; }
            set
            {
                if (this.isCancel != value)
                {
                    this.isCancel = value;
                    this.NotifyOfPropertyChange(() => this.IsCancel);
                }
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                if ( this.IsEnabled != value)
                {
                    this.isEnabled = value;
                    this.NotifyOfPropertyChange(() => this.IsEnabled);
                }
            }
        }

        #endregion
    }
}