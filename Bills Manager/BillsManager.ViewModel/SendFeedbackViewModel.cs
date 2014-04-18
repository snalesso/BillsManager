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
        private readonly IFeedbackSender feedbackSender;

        #endregion

        #region ctor

        public SendFeedbackViewModel(
            IWindowManager windowManager,
            IFeedbackSender feedbackSender)
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
            if (this.feedbackSender.SendFeedback(this.Subject, this.Message))
            {
                this.windowManager.ShowDialog(
                    new DialogViewModel(
                        TranslationManager.Instance.Translate("FeedbackSent").ToString(),
                        TranslationManager.Instance.Translate("FeedbackSentMessage").ToString()));

                if (this.CloseAfterSend)
                    this.TryClose();
            }
            else
                // TODO: feedback serialization for delayed send in case of error. send retry at new start
                this.windowManager.ShowDialog(
                    new DialogViewModel(
                        TranslationManager.Instance.Translate("FeedbackNotSent").ToString(),
                        TranslationManager.Instance.Translate("FeedbackNotSentMessage").ToString()));
        }

        #endregion

        #region commands

        private RelayCommand sendFeedbackCommand;
        public RelayCommand SendFeedbackCommand
        {
            get
            {
                if (this.sendFeedbackCommand == null)
                    this.sendFeedbackCommand = new RelayCommand(
                        () => this.SendFeedback(),
                        () => !string.IsNullOrWhiteSpace(this.Message));

                return this.sendFeedbackCommand;
            }
        }

        private RelayCommand cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                    this.cancelCommand = new RelayCommand(
                        () => this.TryClose());

                return this.cancelCommand;
            }
        }

        #endregion
    }
}