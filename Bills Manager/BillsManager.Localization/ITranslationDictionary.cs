using System.Globalization;

namespace BillsManager.Localization
{
    public interface ITranslationDictionary
    {
        CultureInfo Language { get; }
    }
}