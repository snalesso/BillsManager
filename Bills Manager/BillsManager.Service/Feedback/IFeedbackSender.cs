namespace BillsManager.Services.Feedback
{
    public interface IFeedbackSender
    {
        // TODO: add CanSend

        bool SendFeedback(string subject, string message);
    }
}