using BillsManager.Models;

namespace BillsManager.Services
{
    public interface ISettingsProvider
    {
        Models.Settings Settings { get; }

        bool Save();
    }
}