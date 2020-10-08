﻿using Billy.Domain.Billing.Models;
using Billy.Domain.Billing.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Core.Domain.Billing.Persistence.SQL.MSSQLServer
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
        public abstract Task<Supplier> GetByIdAsync(int id);
        public abstract Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params int[] ids);
        public abstract Task<IReadOnlyCollection<Supplier>> GetMultipleAsync();
        public abstract Task RemoveAsync(int id);
        public abstract Task RemoveAsync(IEnumerable<int> ids);
        public abstract Task UpdateAsync(int id, IEnumerable<KeyValuePair<string, object>> changes);

        #endregion
    }
}