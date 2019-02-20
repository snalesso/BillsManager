using BillsManager.Models;

namespace BillsManager.Services
{
    public interface ISettingsService
    {
        Models.Settings Settings { get; }

        bool Save();
    }
}