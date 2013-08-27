using Caliburn.Micro;

namespace BillsManager.ViewModel
{
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
                new DialogResponse(ResponseType.Retry, "Retry", 10),
                new DialogResponse(ResponseType.Abort, "Abort"),
            };
        }

        #endregion
    }
}
