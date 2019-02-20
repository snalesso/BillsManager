using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillsManager.ViewModels.DesignTime
{
#if DEBUG
    public partial class SearchViewModel<T> : Screen
            where T : class
    {
        public SearchViewModel()
        {
        }
    }
#endif
}