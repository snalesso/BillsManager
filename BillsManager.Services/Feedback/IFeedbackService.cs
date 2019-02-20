namespace BillsManager.Services.Feedback
{
    public interface IFeedbackService
    {
        // TODO: add not sent explanation support, or CanSend

        bool SendFeedback(string subject, string message);
    }
}