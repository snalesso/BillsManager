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
                //this.caption = "Test";
                this.message = "This is a test";
                this.DisplayName = "Test";

                // TODO: move to a designdata project
                this.responses = new[]
                {
                    new DialogResponse(ResponseType.Yes, "go on", "Confirm"),
                    new DialogResponse(ResponseType.No, "Nein", "I say") {IsEnabled = true},
                    new DialogResponse(ResponseType.Retry),
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