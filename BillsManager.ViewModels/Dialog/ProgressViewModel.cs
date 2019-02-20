using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public partial class ProgressViewModel : Screen
    {
        #region ctor

        public ProgressViewModel(string message)
        {
            this.message = message;

            this.DisplayName = message;
        }

        #endregion

        #region properties

        private readonly string message;
        public string Message
        {
            get { return this.message; }
        }

        #endregion
    }
}