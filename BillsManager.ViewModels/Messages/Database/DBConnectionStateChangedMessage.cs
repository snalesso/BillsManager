using BillsManager.Models;
namespace BillsManager.ViewModels.Messages
{
    public class DBConnectionStateChangedMessage
    {
        public DBConnectionStateChangedMessage(DBConnectionState dbState)
        {
            this.dbState = dbState;
        }

        private readonly DBConnectionState dbState;
        public DBConnectionState DBState
        {
            get { return this.dbState; }
        }
    }
}