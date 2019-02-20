namespace BillsManager.Services.Data
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