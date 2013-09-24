using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class DialogViewModel : Screen
    {
        #region fields

        DispatcherTimer timer;

        #endregion

        #region ctor

        // TODO: review ctors gerarchy (make it create a default button in both cases)
        public DialogViewModel(string caption, string message, IEnumerable<DialogResponse> responses)
            : this(caption, message)
        {
            if (responses != null)
                this.Responses = responses;
            else
                this.Responses = new[] { new DialogResponse(ResponseType.Ok) };
            

            bool hasDefault = false, hasCancel = false;

            foreach (DialogResponse resp in this.Responses)
            {
                hasDefault |= resp.IsDefault;
                hasCancel |= resp.IsCancel;
            }

            if (!hasDefault)
                this.Responses.FirstOrDefault().IsDefault = true;

            if (!hasCancel)
                this.Responses.LastOrDefault().IsCancel = true;


            foreach (DialogResponse resp in this.Responses)
            {
                if (resp.Timer > 0)
                {
                    this.timer = new DispatcherTimer() { Interval = new System.TimeSpan(0, 0, 0, 1) };
                    this.timer.Tick += this.UpdateResponsesTimers;
                    break;
                }
            }

            if (timer != null) timer.Start(); // TODO: move in a void connected to DialogView.Shown or smth like that
        }

        public DialogViewModel(string caption, string message)
        {
            this.Caption = caption;
            this.Message = message;

            this.Responses = new[] { new DialogResponse(ResponseType.Ok) };
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

        private ResponseType response = ResponseType.Null;
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

        public override void CanClose(Action<bool> callback)
        {
            callback(this.Response != ResponseType.Null);
        }

        #endregion
    }
}