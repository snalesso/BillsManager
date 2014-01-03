using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BillsManager.Services.Feedback
{
    public class EMailFeedbackSender : IFeedbackSender
    {
        #region ctor

        public EMailFeedbackSender()
        {

        }

        #endregion

        #region IFeedbackSender Members

        public bool SendFeedback(string subject, string message)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                using (var mail = new MailMessage())
                {
                    mail.Sender = new MailAddress("nalesso.sergio@gmail.com", "Nalesso Sergio");
                    mail.From = new MailAddress("feedback@billsmanager.com","Bills Manager Feedback");
                    mail.Body = message;
                    mail.Subject = subject;
                    mail.To.Add(new MailAddress("nalesso.sergio@gmail.com", "Nalesso Sergio"));
                    mail.Priority = MailPriority.High;

                    using (var mailSender = new SmtpClient())
                    {
                        mailSender.Host = "out.aliceposta.it";
                        //mailSender.EnableSsl = true;
                        mailSender.Port = 25;

                        // URGENT: check on mail success required
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