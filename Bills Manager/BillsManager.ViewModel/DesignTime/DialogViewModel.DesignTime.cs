using Caliburn.Micro;

namespace BillsManager.ViewModel
{
#if DEBUG
    public partial class DialogViewModel
    {
        #region ctor

        public DialogViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.LoadDesignTimeData();
            }
        }

        #endregion

        #region methods

        void LoadDesignTimeData()
        {
            this.Caption = "Test";
            this.Message = "This is a fucking test";

            this.Responses = new[]
            {
                new DialogResponse(ResponseType.Yes, ".. go on!", "I wish to ..."),
                new DialogResponse(ResponseType.No, "Nein", "I say") {IsEnabled = true},
                new DialogResponse(ResponseType.Retry, 10),
                new DialogResponse(ResponseType.Abort, "Abort!", 3),
                new DialogResponse(ResponseType.Cancel),
            };
        }

        #endregion
    } 
#endif
}
