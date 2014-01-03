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

            this.DisplayName = "Feedback";
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
                        "Feedback sent", // TODO: language
                        "Your feedback has been sent successfully."));

                if (this.CloseAfterSend)
                    this.TryClose();
            }
            else
                // TODO: feedback serialization for delayed send in case of error. send retry at new start
                this.windowManager.ShowDialog(
                    new DialogViewModel(
                        "Feedback not sent", // TODO: language
                        "An error has occurred. Your feedback hasn't been sent. Please check your connection and try again."));
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