using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.UI.Wpf.Common.Services
{
    public sealed class WindowsDialogService : IDialogService
    {
        private readonly IWindowManager _windowManager;

        public WindowsDialogService(IWindowManager windowManager)
        {
            this._windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        }

        public Task<DialogResult<IReadOnlyCollection<string>>> OpenFileDialogAsync(
            string title,
            string initialDirectoryPath,
            bool isMultiselectAllowed,
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> filters)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = string.Join("|", filters?.Select(kvp => kvp.Key + "|" + string.Join(";", kvp.Value.Select(ext => $"*.{ext}")))),
                Multiselect = isMultiselectAllowed,
                InitialDirectory = initialDirectoryPath,
                Title = title
            };

            var resultCode = ofd.ShowDialog();

            return Task.FromResult(
                new DialogResult<IReadOnlyCollection<string>>(
                    resultCode, resultCode == true 
                    ? ofd.FileNames 
                    : null));
        }

        public Task ShowDialogAsync(object dataContext)
        {
            return this._windowManager.ShowDialogAsync(dataContext);
        }

        public Task ShowOSToastNotification()
        {
            throw new NotImplementedException();
        }

        public Task ShowWindowAsync(object dataContext)
        {
            return this._windowManager.ShowWindowAsync(dataContext);
        }
    }
}