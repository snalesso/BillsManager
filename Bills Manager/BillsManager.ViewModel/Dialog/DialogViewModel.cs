using BillsManager.Localization;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace BillsManager.ViewModels
{
    public partial class DialogViewModel : Screen
    {
        #region fields

        DispatcherTimer timer;

        #endregion

        #region ctor

        /* TODO: create static methods such as
         * .YesCheckedNo .YesTimedNo
         * or
         * YesNo(ResponseType.Timed, ResponseType.Checked) */
        public DialogViewModel(
            string caption,
            string message,
            IEnumerable<DialogResponse> responses)
        {
            this.responses = responses;

            bool hasDefault = false, hasCancel = false;

            foreach (DialogResponse resp in this.Responses)
            {
                hasDefault |= resp.IsDefault;
                hasCancel |= resp.IsCancel;
            }

            if (!hasCancel)
                this.Responses.LastOrDefault().IsCancel = true;

            if (!hasDefault)
                this.Responses.FirstOrDefault().IsDefault = true;

            foreach (DialogResponse resp in this.Responses)
            {
                if (resp.Timer > 0)
                {
                    if (this.timer == null)
                        this.timer = new DispatcherTimer() { Interval = new System.TimeSpan(0, 0, 0, 1) };

                    this.timer.Tick += this.UpdateResponsesTimers;
                    break;
                }
            }

            this.DisplayName = caption;
            this.message = message;

            if (timer != null) timer.Start(); // TODO: move in a DialogView.Shown handler or smth like that
        }

        public DialogViewModel(string caption, string message)
            : this(caption, message, new[] { new DialogResponse(ResponseType.Ok, TranslationManager.Instance.Translate("Ok").ToString()) })
        {
        }

        #endregion

        #region properties

        private readonly string caption;
        public string Caption
        {
            get { return this.caption; }
        }

        private readonly string message;
        public string Message
        {
            get { return this.message; }
        }

        private readonly IEnumerable<DialogResponse> responses;
        public IEnumerable<DialogResponse> Responses
        {
            get { return this.responses; }
        }

        private ResponseType response = ResponseType.InvalidResponse;
        public ResponseType FinalResponse
        {
            get { return this.response; }
            private set
            {
                this.response = value;
            }
        }

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
            this.FinalResponse = dialogResponse.Response;

            bool? result = null;
            if (dialogResponse.IsDefault)
                result = true;
            if (dialogResponse.IsCancel)
                result = false;

            this.TryClose(result);
        }

        public override void CanClose(Action<bool> callback)
        {
            callback(this.FinalResponse != ResponseType.InvalidResponse);
        }

        #endregion
    }
}