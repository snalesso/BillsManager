using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.UI.Wpf.Services
{
    public interface IDialogService
    {
        Task<DialogResult<IReadOnlyCollection<string>>> OpenFileDialogAsync(
            string title,
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> filters);

        Task ShowWindowAsync(object dataContext);

        Task ShowDialogAsync(object dataContext);

        Task ShowOSToastNotification();
    }
}