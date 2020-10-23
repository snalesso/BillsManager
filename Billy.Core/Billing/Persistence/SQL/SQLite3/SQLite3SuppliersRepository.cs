﻿using Billy.Billing.Models;
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

        public abstract Task<Supplier> GetByIdAsync(long id);
        public abstract Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params long[] ids);
        public abstract Task<IReadOnlyCollection<Supplier>> GetMultipleAsync();

        public abstract Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        public abstract Task RemoveAsync(long id);
        public abstract Task RemoveAsync(IEnumerable<long> ids);
        public abstract Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);

        #endregion
    }

    public abstract class SQLite3BillsRepository : IBillsRepository
    {
        #region fields & constants

        protected readonly SQLiteConnection _connection;
        protected readonly SQLiteTransactionBase _transaction;

        #endregion

        #region ctor

        public SQLite3BillsRepository(
            SQLiteConnection connection,
            SQLiteTransactionBase transaction = null)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._transaction = transaction;
        }

        #endregion

        #region methods

        // TODO: define base impl
        public abstract Task<Bill> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data);
        public abstract Task<Bill> GetByIdAsync(long id);
        public abstract Task<IReadOnlyCollection<Bill>> GetMultipleAsync();
        public abstract Task RemoveAsync(long id);
        public abstract Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes);

        #endregion
    }
}
