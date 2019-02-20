using BillsManager.Localization;
using BillsManager.Services.Feedback;
using BillsManager.ViewModels.Commanding;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public class SendFeedbackViewModel : Screen
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IFeedbackService feedbackSender;

        #endregion

        #region ctor

        public SendFeedbackViewModel(
            IWindowManager windowManager,
            IFeedbackService feedbackSender)
        {
            this.windowManager = windowManager;
            this.feedbackSender = feedbackSender;

            this.DisplayName = TranslationManager.Instance.Translate("SendFeedback").ToString();
        }

        #endregion

        #region properties

        private string subject;
        public string Subject
        {
            get { return this.subject; }
            set
            {
                if (this.subject == value) return;

                this.subject = value;
                this.NotifyOfPropertyChange(() => this.Subject);
            }
        }

        private string message;
        public string Message
        {
            get { return this.message; }
            set
            {
                if (this.message == value) return;

                this.message = value;
                this.NotifyOfPropertyChange(() => this.Message);
            }
        }

        private bool closeAfterSend = true;
        public bool CloseAfterSend
        {
            get { return this.closeAfterSend; }
            set
            {
                if (this.closeAfterSend == value) return;

                this.closeAfterSend = value;
                this.NotifyOfPropertyChange(() => this.CloseAfterSend);
            }
        }

        #endregion

        #region methods

        private void SendFeedback()
        {
            // TODO: feedback serialization for delayed send in case of error. send retries in a second moment (restart/countdown)
            var sent = this.feedbackSender.SendFeedback(this.Subject, this.Message);

            DialogViewModel resultDialog =
                DialogViewModel.Show(
                    sent ? DialogType.Information : DialogType.Error,
                    sent ? TranslationManager.Instance.Translate("FeedbackSent") : TranslationManager.Instance.Translate("FeedbackNotSent"),
                    sent ? TranslationManager.Instance.Translate("FeedbackSentMessage") : TranslationManager.Instance.Translate("FeedbackNotSentMessage"))
                 .Ok();

            this.windowManager.ShowDialog(resultDialog);

            if (sent & this.CloseAfterSend)
                this.TryClose();
        }

        #endregion

        #region commands

        private RelayCommand sendFeedbackCommand;
        public RelayCommand SendFeedbackCommand
        {
            get
            {
                return this.sendFeedbackCommand ?? (this.sendFeedbackCommand = 
                    new RelayCommand(
                        () => this.SendFeedback(),
                        () => !string.IsNullOrWhiteSpace(this.Message)));
            }
        }

        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return this.cancelCommand ?? (this.cancelCommand = 
                    new RelayCommand(
                        () => this.TryClose()));
            }
        }

        #endregion
    }
}