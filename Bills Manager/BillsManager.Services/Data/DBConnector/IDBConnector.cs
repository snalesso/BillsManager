namespace BillsManager.Services.Providers
{
    public interface IDBConnector :
        IBillsProvider,
        ISuppliersProvider
    {
        bool Connect();

        bool Save();

        void Disconnect();
    }
}