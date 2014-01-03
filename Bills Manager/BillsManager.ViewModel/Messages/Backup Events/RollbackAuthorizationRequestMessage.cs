using System;

namespace BillsManager.ViewModels.Messages
{
    public class RollbackAuthorizationRequestMessage
    {
        public RollbackAuthorizationRequestMessage(string dbName, Action confirm, Action negate)
        {
            this.dbName = dbName;
            this.confirm = confirm;
            this.negate = negate;
        }
        
        private readonly string dbName;
        public string DBName
        {
            get { return this.dbName; }
        }
        
        private readonly Action confirm;
        public Action ConfirmAuthorization
        {
            get { return this.confirm; }
        }
        
        private readonly Action negate;
        public Action NegateAuthorization
        {
            get { return this.negate; }
        }
    }
}