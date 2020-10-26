namespace BillsManager.v1.Models
{
    public enum DBConnectionState // TODO: consider mapping full type name to res value
    {
        Disconnected = 0,
        Connected = 1,
        Unsaved = 2
    }
}