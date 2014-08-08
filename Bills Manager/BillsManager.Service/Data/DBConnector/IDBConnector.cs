namespace BillsManager.Services.Providers
{
    public interface IDBConnector :
        IBillsProvider,
        ISuppliersProvider/*,
        ITagsProvider,
        IAgentsProvider*/
    {
        bool Connect();

        bool Save();

        void Disconnect();

        //string DBPath { get; } // TODO: obsolete?

        //string DBName { get; }
    }
}