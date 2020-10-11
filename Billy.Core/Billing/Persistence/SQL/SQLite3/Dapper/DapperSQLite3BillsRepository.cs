using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billy.Domain.Billing.Models;
using Billy.Domain.Billing.Persistence;
using Billy.Domain.Persistence.SQL.SQLite3.Dapper;
using Dapper;

namespace Billy.Domain.Billing.Persistence.SQL.SQLite3.Dapper
{
    public class DapperSQLite3BillsRepository : /*DapperSQLite3Repository,*/ IBillsRepository
    {
        private readonly SQLiteConnection _connection;

        public DapperSQLite3BillsRepository(SQLiteConnection connection) //: base(connection)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #region methods

        public async Task<IReadOnlyCollection<Bill>> GetBillsAsync()
        {
            var x = await this._connection.QueryAsync<Bill>("SELECT ... FROM ... ", new { });

            return x.ToImmutableList();
        }

        public Task RemoveAsync(uint id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(IEnumerable<uint> ids)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Bill bill)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(uint id, Bill changes)
        {
            throw new NotImplementedException();
        } 

        #endregion

        #region events

        //public IObservable<IReadOnlyCollection<Bill>> BillsAddeded => throw new NotImplementedException();
        //public IObservable<IReadOnlyCollection<Bill>> BillsRemoved => throw new NotImplementedException();
        //public IObservable<IReadOnlyCollection<Bill>> BillsUpdated => throw new NotImplementedException();

        public event EventHandler<IReadOnlyCollection<Bill>> BillsAddeded;
        public event EventHandler<IReadOnlyCollection<Bill>> BillsRemoved;
        public event EventHandler<IReadOnlyCollection<Bill>> BillsUpdated;

        protected virtual void OnBillsAddeded(IReadOnlyCollection<Bill> suppliers)
        {
            this.BillsAddeded?.Invoke(this, suppliers);
        }

        protected virtual void OnBillsRemoved(IReadOnlyCollection<Bill> suppliers)
        {
            this.BillsRemoved?.Invoke(this, suppliers);
        }

        protected virtual void OnBillsUpdated(IReadOnlyCollection<Bill> suppliers)
        {
            this.BillsUpdated?.Invoke(this, suppliers);
        }

        #endregion
    }
}
