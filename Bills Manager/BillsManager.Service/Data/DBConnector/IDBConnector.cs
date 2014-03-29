namespace BillsManager.Services.Providers
{
    public interface IDBConnector :
        IBillsProvider,
        ISuppliersProvider,
        ITagsProvider/*,
        IAgentsProvider*/
    {
        bool Open();

        bool Save();

        void Close();

        //string DBPath { get; } // TODO: obsolete?

        //string DBName { get; }
    }
}