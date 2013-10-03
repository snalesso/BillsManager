using Caliburn.Micro;

namespace BillsManager.ViewModel
{
#if DEBUG
    public partial class ShellViewModel : Screen, IShell
    {
        #region ctor

        public ShellViewModel()
        {
        }

        #endregion
    } 
#endif
}