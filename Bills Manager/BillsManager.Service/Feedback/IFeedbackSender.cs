namespace BillsManager.Services.Feedback
{
    public interface IFeedbackSender
    {
        bool SendFeedback(string subject, string message);
    }
}