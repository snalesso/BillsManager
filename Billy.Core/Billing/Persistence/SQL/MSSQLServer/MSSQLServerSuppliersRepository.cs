using Billy.Billing.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.MSSQLServer
{
    public abstract class MSSQLServerSuppliersRepository : ISuppliersRepository
    {
        #region fields & constants

        protected readonly SqlConnection _connection;
        protected readonly SqlTransaction _transaction;

        #endregion

        #region ctor

        public MSSQLServerSuppliersRepository(
            SqlConnection connection,
            SqlTransaction transaction = null)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._transaction = transaction;
        }

        #endregion

        #region methods

        public abstract Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        public abstract Task<Supplier> GetByIdAsync(long id);
        public abstract Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params long[] ids);
        public abstract Task<IReadOnlyCollection<Supplier>> GetMultipleAsync();
        public abstract Task RemoveAsync(long id);
        public abstract Task RemoveAsync(IEnumerable<long> ids);
        public abstract Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);

        #endregion
    }
}
