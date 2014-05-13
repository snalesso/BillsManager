using System.Globalization;

namespace BillsManager.Models
{
    // TODO: add digits separator style
    public partial class Settings
    {
        public Settings(
            CultureInfo language,
            bool startupDBLoad,
            string feedbackToEmailAddress
            /* string feedbackEmailProvider,
               string feedbackEmailPort */)
        {
            this.Language = language;
            this.StartupDBLoad = startupDBLoad;
            this.FeedbackToEmailAddress = feedbackToEmailAddress;
            //this.FeedbackEmailProvider = feedbackEmailProvider;
            //this.FeedbackEmailPort = feedbackEmailPort;
        }

        public CultureInfo Language { get; set; }

        public bool StartupDBLoad { get; set; }

        public string FeedbackToEmailAddress { get; set; }

        //public string FeedbackEmailProvider { get; set; }

        //public string FeedbackEmailPort { get; set; }
    }
}