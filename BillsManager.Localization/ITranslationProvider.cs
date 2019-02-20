using System.Collections.Generic;
using System.Globalization;

namespace BillsManager.Localization
{
    public interface ITranslationProvider
    {
        string Translate(string key);

        IEnumerable<CultureInfo> Languages { get; }
    }
}