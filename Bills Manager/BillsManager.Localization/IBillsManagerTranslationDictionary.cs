namespace BillsManager.Localization
{
    public interface IBillsManagerTranslationDictionary : ITranslationDictionary
    {
        string Ok { get; }
        string Cancel { get; }
        string Exit { get; }
        string Connect { get; }
        string Disconnect { get; }
        string Open { get; }
        string Close { get; }
        string Supplier { get; }
        string Bill { get; }
        string Suppliers { get; }
        string Bills { get; }
    }
}