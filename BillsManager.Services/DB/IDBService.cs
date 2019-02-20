namespace BillsManager.Services.DB
{
    public interface IDBService :
        IBillsRepository,
        ISuppliersRepository
    {
        bool Connect();

        bool Save();

        void Disconnect();
    }
}