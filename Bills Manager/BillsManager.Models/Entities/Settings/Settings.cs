using System.Globalization;

namespace BillsManager.Models
{
    // TODO: add digits separator style
    public partial class Settings : ISettings
    {
        public Settings(
            CultureInfo language,
            bool startupDBLoad,
            string feedbackToEmailAddress
            )
        {
            this.Language = language;
            this.StartupDBLoad = startupDBLoad;
            this.FeedbackToEmailAddress = feedbackToEmailAddress;
        }

        public CultureInfo Language { get; set; }

        public bool StartupDBLoad { get; set; }

        public string FeedbackToEmailAddress { get; set; }
    }
}