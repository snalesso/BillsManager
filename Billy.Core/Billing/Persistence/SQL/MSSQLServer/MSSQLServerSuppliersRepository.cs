using Billy.Billing.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.MSSQLServer
{
    public abstract class MSSQLServerSuppliersRepository : IReadSuppliersRepository, IWriteSuppliersRepository
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

        public abstract Task<Supplier> GetByIdAsync(long id);
        public abstract Task<Supplier> GetSingleAsync(SupplierCriteria criteria = null);
        public abstract Task<IReadOnlyCollection<Supplier>> GetMultipleAsync(SupplierCriteria criteria = null);

        public abstract Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        public abstract Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);
        public abstract Task RemoveAsync(long id);

        #endregion
    }
}
