using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3
{
    public abstract class SQLite3SuppliersRepository : ISuppliersRepository, IReadSuppliersRepository, IWriteSuppliersRepository
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

        public abstract Task<Supplier> GetByIdAsync(long id);
        public abstract Task<Supplier> GetSingleAsync(SupplierCriteria criteria = null);
        public abstract Task<IReadOnlyCollection<Supplier>> GetMultipleAsync(SupplierCriteria criteria = null);

        public abstract Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        public abstract Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);
        public abstract Task RemoveAsync(long id);


        #endregion
    }
}
