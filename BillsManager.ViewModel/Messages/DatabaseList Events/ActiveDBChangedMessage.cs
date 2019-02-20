namespace BillsManager.ViewModels.Messages
{
    public sealed class ActiveDBChangedMessage
    {
        public ActiveDBChangedMessage(DBViewModel dbViewModel)
        {
            this.activeDB = dbViewModel;
        }
        
        private readonly DBViewModel activeDB;
        public DBViewModel ActiveDB
        {
            get { return this.activeDB; }
        }
    }
}