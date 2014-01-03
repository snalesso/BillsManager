namespace BillsManager.Services.Providers
{
    public interface IDBConnector :
        IBillsProvider,
        ISuppliersProvider,
        IAgentsProvider
    {
        bool Open();

        bool Save();

        void Close();

        string Path { get; }

        string DBName { get; } /* TODO: a warning says:
                              * Warning	2	'BillsManager.Services.Providers.IDBConnector.Name'
                              * hides inherited member 'BillsManager.Services.Providers.IBillsProvider.Name'.
                              * Use the new keyword if hiding was intended.
                              * C:\Users\Sergio\Source\Workspaces\billsmanager\Bills Manager\BillsManager.Service
                              * \Providers\DBConnector\IDBConnector.cs	16	16	BillsManager.Services */
    }
}