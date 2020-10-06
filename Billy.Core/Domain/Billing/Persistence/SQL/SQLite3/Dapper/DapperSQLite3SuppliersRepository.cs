using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Billy.Core.Domain.Persistence;
using Billy.Domain.Billing.Models;
using Billy.Domain.Billing.Persistence.SQL.SQLite3.Dapper;
using Billy.Domain.Persistence.SQL.SQLite3;
using Dapper;

namespace Billy.Domain.Billing.Persistence.SQL.SQLite3.Dapper
{
    public class DapperSQLite3SuppliersRepository : ISuppliersRepository
    {
        #region fields & constants

        private readonly SQLiteConnection _connection;
        private readonly SQLiteTransactionBase _transaction;

        #endregion

        #region ctor

        public DapperSQLite3SuppliersRepository(
            SQLiteConnection connection,
            SQLiteTransactionBase transaction = null)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._transaction = transaction;
        }

        #endregion

        #region methods

        public async Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data)
        {
            try
            {
                var columns = data.Select(x => "[" + x.Key + "]");
                var values = data.Select(x => x.Value);
                var valuePlaceholders = data.Select(x => "@" + x.Key);
                var sql =
                    $"insert into [{nameof(Supplier)}] ({string.Join(",", columns)}) values ({string.Join(",", valuePlaceholders)})" +
                    ";SELECT last_insert_rowid();";

                var id = await SqlMapper.ExecuteScalarAsync<int>(
                    this._connection,
                    new CommandDefinition(
                        commandText: sql,
                        parameters: data.Select(kvp => new KeyValuePair<string, object>("@" + kvp.Key, kvp.Value)),
                        transaction: this._transaction));

                sql = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(Supplier.Id)}";
                //var dynParams = new DynamicParameters();
                //dynParams.Add(nameof(Supplier.Id), id);

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: sql, //$"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = "+id,
                    param: new Dictionary<string, object>()
                    {
                        { nameof(Supplier.Id), id }
                    },
                    transaction: this._transaction) as IDictionary<string, object>;

                var insertedSupplier = new Supplier(
                    (int)lastInsertedRow[nameof(Supplier.Id)],
                    (string)lastInsertedRow[nameof(Supplier.Name)],
                    (string)lastInsertedRow[nameof(Supplier.Email)],
                    (string)lastInsertedRow[nameof(Supplier.Website)],
                    (string)lastInsertedRow[nameof(Supplier.Phone)],
                    (string)lastInsertedRow[nameof(Supplier.Fax)],
                    (string)lastInsertedRow[nameof(Supplier.Notes)],
                    (Address)lastInsertedRow[nameof(Supplier.Address)],
                    (Agent)lastInsertedRow[nameof(Supplier.Agent)]);

                return insertedSupplier;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<IReadOnlyCollection<Supplier>> GetMultipleAsync()
        {
            try
            {
                var suppliers = new List<Supplier>();

                await using (var reader = await SqlMapper.ExecuteReaderAsync(
                    cnn: this._connection,
                    sql: $"select * from [{nameof(Supplier)}]",
                    transaction: this._transaction))
                {
                    while (await reader.ReadAsync())
                    {
                        suppliers.Add(
                            new Supplier(
                                await reader.GetSafeAsync<int>(nameof(Supplier.Id)),
                                await reader.GetSafeAsync<string>(nameof(Supplier.Name)),
                                await reader.GetSafeAsync<string>(nameof(Supplier.Email)),
                                await reader.GetSafeAsync<string>(nameof(Supplier.Website)),
                                await reader.GetSafeAsync<string>(nameof(Supplier.Phone)),
                                await reader.GetSafeAsync<string>(nameof(Supplier.Fax)),
                                await reader.GetSafeAsync<string>(nameof(Supplier.Notes)),
                                await reader.GetSafeAsync<Address>(nameof(Supplier.Address)),
                                await reader.GetSafeAsync<Agent>(nameof(Supplier.Agent))));
                    }
                }

                return suppliers;
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine(ex);
                return Array.Empty<Supplier>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Array.Empty<Supplier>();
            }
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            try
            {
                var query = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(Supplier.Id)}";

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: query,
                    param: id,
                    transaction: this._transaction) as IDictionary<string, object>;

                // TODO: create helper method dictionary -> supplier
                var supplier = new Supplier(
                    (int)lastInsertedRow[nameof(Supplier.Id)],
                    (string)lastInsertedRow[nameof(Supplier.Name)],
                    (string)lastInsertedRow[nameof(Supplier.Email)],
                    (string)lastInsertedRow[nameof(Supplier.Website)],
                    (string)lastInsertedRow[nameof(Supplier.Phone)],
                    (string)lastInsertedRow[nameof(Supplier.Fax)],
                    (string)lastInsertedRow[nameof(Supplier.Notes)],
                    (Address)lastInsertedRow[nameof(Supplier.Address)],
                    (Agent)lastInsertedRow[nameof(Supplier.Agent)]);

                return supplier;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var query = $"delete from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(Supplier.Id)}";

                var affectedRows = await SqlMapper.ExecuteAsync(
                    cnn: this._connection,
                    sql: query,
                    param: id,
                    transaction: this._transaction);

                if (affectedRows <= 0)
                    throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Task RemoveAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(int id, IEnumerable<KeyValuePair<string, object>> changes)
        {
            try
            {
                // TODO: find a way to avoid parameter keys collisions, like id / changes.id, maybe =<param_name> insread of @param_name ()
                var sets = changes.Select(x => $"[{x.Key}] = @{nameof(changes)}.{x.Key}");
                var sql = $"update from [{nameof(Supplier)}] set {string.Join(",", sets)} where [{nameof(Supplier.Id)}] = @{nameof(Supplier.Id)}";

                var affectedRows = await SqlMapper.ExecuteAsync(
                    this._connection,
                    new CommandDefinition(
                        commandText: sql,
                        parameters: new Dictionary<string, object>()
                        {
                            { nameof(Supplier.Id), id },
                            { nameof(changes), changes }
                        },
                        transaction: this._transaction));

                if (affectedRows <= 0)
                    throw new KeyNotFoundException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        #region events

        //public IObservable<IReadOnlyCollection<Supplier>> SuppliersAddeded => throw new NotImplementedException();
        //public IObservable<IReadOnlyCollection<Supplier>> SuppliersRemoved => throw new NotImplementedException();
        //public IObservable<IReadOnlyCollection<Supplier>> SuppliersUpdated => throw new NotImplementedException();

        public event EventHandler<IReadOnlyCollection<Supplier>> SuppliersAddeded;
        public event EventHandler<IReadOnlyCollection<Supplier>> SuppliersRemoved;
        public event EventHandler<IReadOnlyCollection<Supplier>> SuppliersUpdated;

        protected virtual void OnSuppliersAddeded(IReadOnlyCollection<Supplier> suppliers)
        {
            this.SuppliersAddeded?.Invoke(this, suppliers);
        }

        protected virtual void OnSuppliersRemoved(IReadOnlyCollection<Supplier> suppliers)
        {
            this.SuppliersRemoved?.Invoke(this, suppliers);
        }

        protected virtual void OnSuppliersUpdated(IReadOnlyCollection<Supplier> suppliers)
        {
            this.SuppliersUpdated?.Invoke(this, suppliers);
        }

        public Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params int[] ids)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}