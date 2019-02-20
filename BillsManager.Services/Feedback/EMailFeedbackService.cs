using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BillsManager.Services.Feedback
{
    public class EMailFeedbackService : IFeedbackService
    {
        #region fields

        private readonly string toEmailAddress;

        #endregion

        #region ctor

        public EMailFeedbackService(string toEmailAddress)
        {
            this.toEmailAddress = toEmailAddress;
        }

        #endregion

        #region IFeedbackSender Members

        public bool SendFeedback(string subject, string message)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            var emailRegex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            if (!emailRegex.IsMatch(this.toEmailAddress))
            {
                return false;
            }

            try
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

                        mailSender.Send(mail);

                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}