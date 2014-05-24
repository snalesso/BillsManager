using System.Net.Mail;

namespace BillsManager.Services.Feedback
{
    public class EMailFeedbackSender : IFeedbackSender
    {
        #region fields

        private readonly string toEmailAddress;

        #endregion

        #region ctor

        public EMailFeedbackSender(string toEmailAddress)
        {
            this.toEmailAddress = toEmailAddress;
        }

        #endregion

        #region IFeedbackSender Members

        public bool SendFeedback(string subject, string message)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress("feedback@billsmanager.com", "Bills Manager Feedback");
                    mail.Body = message;
                    mail.Subject = subject;
                    mail.To.Add(new MailAddress(toEmailAddress));
                    mail.Priority = MailPriority.High;

                    using (var mailSender = new SmtpClient())
                    {
                        mailSender.Host = "out.aliceposta.it";
                        //mailSender.EnableSsl = true;
                        mailSender.Port = 25;

                        // URGENT: check on send success required
                        mailSender.Send(mail);

                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}