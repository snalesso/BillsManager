namespace BillsManager.Services.Settings
{
    public interface ISettingsProvider
    {
        Models.Settings Settings { get; }

        bool Save();
    }
}