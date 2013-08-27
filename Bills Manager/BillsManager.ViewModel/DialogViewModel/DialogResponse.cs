using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public class DialogResponse : PropertyChangedBase
    {
        #region ctor

        public DialogResponse(ResponseType response, string text, ushort timer)
        {
            this.Response = response;
            this.Text = text;
            this.Timer = timer;
        }

        public DialogResponse(ResponseType response, string text)
            : this(response, text, 0)
        {
        }

        public DialogResponse(ResponseType response, ushort timer)
            : this(response, Enum.GetName(typeof(ResponseType), response), timer)
        {
        }

        public DialogResponse(ResponseType response)
            : this(response, Enum.GetName(typeof(ResponseType), response), 0)
        {
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
            get { return timer; }
            set
            {
                if (this.timer != value)
                {
                    timer = value;
                    this.NotifyOfPropertyChange(() => this.Text);
                    this.NotifyOfPropertyChange(() => this.IsEnabled);
                }
            }
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

        public bool IsEnabled
        {
            get { return this.Timer == 0; }
        }

        #endregion
    }
}
