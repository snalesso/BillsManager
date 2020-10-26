namespace BillsManager.v1.Services.Data
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