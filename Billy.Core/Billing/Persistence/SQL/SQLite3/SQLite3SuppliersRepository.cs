using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public abstract class SQLite3SuppliersRepository : ISuppliersRepository
    {
        #region fields & constants

        protected readonly SQLiteConnection _connection;
        protected readonly SQLiteTransactionBase _transaction;

        #endregion

        #region ctor

        public SQLite3SuppliersRepository(
            SQLiteConnection connection,
            SQLiteTransactionBase transaction = null)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._transaction = transaction;
        }

        #endregion

        #region methods

        public abstract Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        public abstract Task<Supplier> GetByIdAsync(int id);
        public abstract Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params int[] ids);
        public abstract Task<IReadOnlyCollection<Supplier>> GetMultipleAsync();
        public abstract Task RemoveAsync(int id);
        public abstract Task RemoveAsync(IEnumerable<int> ids);
        public abstract Task UpdateAsync(int id, IEnumerable<KeyValuePair<string, object>> changes);

        #endregion
    }
}
