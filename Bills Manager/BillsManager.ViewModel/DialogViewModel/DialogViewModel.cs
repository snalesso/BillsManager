using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Caliburn.Micro;
using System.Linq;

namespace BillsManager.ViewModel
{
    public partial class DialogViewModel : Screen
    {
        #region fields

        DispatcherTimer timer;

        #endregion

        #region ctor


        public DialogViewModel(string caption, string message, IEnumerable<DialogResponse> responses)
        {
            this.Caption = caption;
            this.Message = message;

            if (responses != null)
            {
                this.Responses = responses;

                foreach (DialogResponse resp in responses)
                {
                    if (resp.Timer > 0)
                    {
                        this.timer = new DispatcherTimer() { Interval = new System.TimeSpan(0, 0, 0, 1) };
                        this.timer.Tick += this.UpdateResponsesTimers;
                        break;
                    }
                }
            }
            else
            {
                this.Responses = new[] { new DialogResponse(ResponseType.Ok, "Ok") };
            }

            this.Responses.FirstOrDefault().IsDefault = true;
            this.Responses.LastOrDefault().IsCancel = true;

            if (timer != null) timer.Start(); // TODO: move in a void connected to DialogView.Shown or smth like that
        }

        public DialogViewModel(string caption, string message)
            : this(caption, message, null)
        {
        }

        #endregion

        #region properties

        private string caption;
        public string Caption
        {
            get { return this.caption; }
            set
            {
                if (this.caption != value)
                {
                    this.caption = value;
                    this.NotifyOfPropertyChange(() => this.Caption);
                }
            }
        }

        private string message;
        public string Message
        {
            get { return this.message; }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyOfPropertyChange(() => this.Message);
                }
            }
        }

        private IEnumerable<DialogResponse> responses;
        public IEnumerable<DialogResponse> Responses
        {
            get { return this.responses; }
            set
            {
                if (this.responses != value)
                {
                    this.responses = value;
                    this.NotifyOfPropertyChange(() => this.Responses);
                }
            }
        }

        private ResponseType response;
        public ResponseType Response
        {
            get { return this.response; }
            set
            {
                if (this.response != value)
                {
                    this.response = value;
                }
            }
        }

        #region overrieds

        public new string DisplayName
        {
            get { return this.Caption; }
        }

        #endregion

        #endregion

        #region methods

        void UpdateResponsesTimers(object sender, EventArgs e)
        {
            bool leftovers = false;

            foreach (DialogResponse resp in this.Responses)
            {
                if (resp.Timer > 0)
                {
                    resp.Timer--;
                    leftovers |= (resp.Timer > 0);
                }
            }

            if (!leftovers) this.timer.Stop();
        }

        public void Respond(DialogResponse dialogResponse)
        {
            this.Response = dialogResponse.Response;

            this.TryClose(null);
        }

        #endregion
    }
}