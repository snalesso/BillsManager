using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class DialogViewModel : Screen, IDialogViewModelFactory
    {
        #region ctor

        private DialogViewModel(
            DialogType dialogType,
            string title,
            string message)
        {
            this.responses = new[] { new DialogResponse(ResponseType.Ok) };

            this.dialogType = dialogType;
            this.DisplayName = title;
            this.message = message;
        }

        #endregion

        #region properties

        private readonly DialogType dialogType;
        public DialogType DialogType
        {
            get { return this.dialogType; }
        }

        //private readonly string caption;
        //public string Caption
        //{
        //    get { return this.caption; }
        //}

        private readonly string message;
        public string Message
        {
            get { return this.message; }
        }

        private IEnumerable<DialogResponse> responses;
        public IEnumerable<DialogResponse> Responses
        {
            get { return this.responses; }
            private set
            {
                if ((value != null & value.Count() > 0) &
                    (this.responses != value))
                {
                    this.responses = value;

                    foreach (var resp in this.responses)
                    {
                        resp.IsDefault = false;
                        resp.IsCancel = false;
                    }
                    this.Responses.First().IsDefault = true;
                    this.Responses.Last().IsCancel = true;
                }
            }
        }

        private ResponseType finalResponse = ResponseType.Unset;
        public ResponseType FinalResponse
        {
            get { return this.finalResponse; }
            protected set
            {
                this.finalResponse = value;
            }
        }

        #endregion

        #region methods

        #region creation

        public static IDialogViewModelFactory Show(DialogType dialogType, string title, string text)
        {
            return new DialogViewModel(dialogType, title, text);
        }

        #region IDialogViewModelFactory Members

        DialogViewModel IDialogViewModelFactory.Ok(string okText)
        {
            this.Responses = new DialogResponse[] 
            { 
                new DialogResponse(ResponseType.Ok, okText)
            };

            return this;
        }

        DialogViewModel IDialogViewModelFactory.OkCancel(string okText, string cancelText)
        {
            this.Responses = new DialogResponse[] 
            { 
                new DialogResponse(ResponseType.Ok, okText),
                new DialogResponse(ResponseType.Cancel, cancelText)
            };

            return this;
        }

        DialogViewModel IDialogViewModelFactory.YesNo(string yesText, string noText)
        {
            this.Responses = new DialogResponse[] 
            { 
                new DialogResponse(ResponseType.Yes, yesText),
                new DialogResponse(ResponseType.No, noText)
            };

            return this;
        }

        DialogViewModel IDialogViewModelFactory.YesNoCancel(string yesText, string noText, string cancelText)
        {
            this.Responses = new DialogResponse[] 
            { 
                new DialogResponse(ResponseType.Yes, yesText),
                new DialogResponse(ResponseType.No, noText),
                new DialogResponse(ResponseType.Cancel, cancelText)
            };

            return this;
        }

        DialogViewModel IDialogViewModelFactory.RetryCancel(string retryText, string cancelText)
        {
            this.Responses = new DialogResponse[] 
            { 
                new DialogResponse(ResponseType.Retry, retryText),
                new DialogResponse(ResponseType.Cancel, cancelText)
            };

            return this;
        }

        #endregion

        #endregion

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
            callback(this.FinalResponse != ResponseType.Unset);
        }

        #endregion
    }
}