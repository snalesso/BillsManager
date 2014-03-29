using Caliburn.Micro;

namespace BillsManager.ViewModels
{
#if DEBUG
    public partial class DialogViewModel
    {
        #region ctor

        public DialogViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.caption = "Test";
                this.message = "This is a fucking test";
                this.DisplayName = this.caption;

                // TODO: move to a designdata project
                this.responses = new[]
                {
                    new DialogResponse(ResponseType.Yes, "go on", "Confirm"),
                    new DialogResponse(ResponseType.No, "Nein", "I say") {IsEnabled = true},
                    new DialogResponse(ResponseType.Retry, 10),
                    new DialogResponse(ResponseType.Cancel),
                };

                this.LoadDesignTimeData();
            }
        }

        #endregion

        #region methods

        void LoadDesignTimeData()
        {
        }

        #endregion
    }
#endif
}