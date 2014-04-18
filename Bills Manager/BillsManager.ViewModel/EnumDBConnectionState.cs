using BillsManager.Localization;

namespace BillsManager.ViewModels
{
    public enum DBConnectionState // TODO: consider mapping full type name to res value
    {
        [Localize("Disconnected_toDB")]
        Disconnected = 0,
        [Localize("Connected_toDB")]
        Connected = 1,
        [Localize("Unsaved_toDB")]
        Unsaved = 2
    }
}