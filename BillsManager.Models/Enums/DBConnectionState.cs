using BillsManager.Localization.Attributes;

namespace BillsManager.Models
{
    public enum DBConnectionState // TODO: consider mapping full type name to res value
    {
        [LocalizedDisplayName("Disconnected_toDB")]
        Disconnected = 0,
        [LocalizedDisplayName("Connected_toDB")]
        Connected = 1,
        [LocalizedDisplayName("Unsaved_toDB")]
        Unsaved = 2
    }
}