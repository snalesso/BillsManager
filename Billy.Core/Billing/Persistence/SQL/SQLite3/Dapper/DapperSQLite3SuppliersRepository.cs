using Billy.Billing.Models;
using Billy.Domain.Persistence;
using Billy.Domain.Persistence.SQL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
//using Billy.Domain.Persistence.SQL.MSSQLServer;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3.Dapper
{
    public class DapperSQLite3SuppliersRepository : SQLite3SuppliersRepository
    {
        #region ctor

        public DapperSQLite3SuppliersRepository(
            SQLiteConnection connection,
            SQLiteTransactionBase transaction = null)
            : base(connection, transaction)
        {
        }

        #endregion

        #region methods

        public override async Task<Supplier> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data)
        {
            try
            {
                var flattenedData = DbSchemaHelper.FlattenChanges(new Dictionary<string, object>(data));
                var columns = flattenedData.Select(x => "[" + x.Key + "]");
                //var values = data.Select(x => x.Value);
                var values = flattenedData.Select(x => "@" + x.Key);
                var sql =
                    $"insert into [{nameof(Supplier)}] ({string.Join(",", columns)}) values ({string.Join(",", values)});" +
                    "SELECT SCOPE_IDENTITY();";

                // TODO: consider using QueryMultipleAsync
                var id = await SqlMapper.ExecuteScalarAsync<int>(
                    this._connection,
                    new CommandDefinition(
                        commandText: sql,
                        parameters: flattenedData.Select(kvp => new KeyValuePair<string, object>("@" + kvp.Key, kvp.Value)),
                        transaction: this._transaction));

                sql = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(Supplier.Id)};";

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: sql, //$"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = "+id,
                    param: new Dictionary<string, object>()
                    {
                        { nameof(Supplier.Id), id }
                    },
                    transaction: this._transaction)
                    as IDictionary<string, object>;

                // TODO: improve parsing, handling nulls etc.
                var insertedSupplier = new Supplier(
                    (int)lastInsertedRow[nameof(Supplier.Id)],
                    (string)lastInsertedRow[nameof(Supplier.Name)],
                    (string)lastInsertedRow[nameof(Supplier.Email)],
                    (string)lastInsertedRow[nameof(Supplier.Website)],
                    (string)lastInsertedRow[nameof(Supplier.Phone)],
                    (string)lastInsertedRow[nameof(Supplier.Fax)],
                    (string)lastInsertedRow[nameof(Supplier.Notes)],
                    Address.Create(
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Country))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Province))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.City))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Zip))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Street))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Number))]),
                    Agent.Create(
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Name))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Surname))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Phone))]));

                //await Task.Delay(1000 * 3);

                return insertedSupplier;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public override async Task<IReadOnlyCollection<Supplier>> GetMultipleAsync()
        {
            try
            {
                var suppliers = new List<Supplier>();

                await using (var reader = await SqlMapper.ExecuteReaderAsync(
                    cnn: this._connection,
                    sql: $"select * from [{nameof(Supplier)}];",
                    transaction: this._transaction))
                {
                    while (await reader.ReadAsync())
                    {
                        var address = Address.Create(
                            country: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Country))),
                            province: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Province))),
                            city: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.City))),
                            zip: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Zip))),
                            street: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Street))),
                            number: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Number))));

                        var agent = Agent.Create(
                            name: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Name))),
                            surname: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Surname))),
                            phone: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Phone))));

                        suppliers.Add(
                            new Supplier(
                                id: await reader.GetSafeAsync<int>(nameof(Supplier.Id)),
                                name: await reader.GetSafeAsync<string>(nameof(Supplier.Name)),
                                eMail: await reader.GetSafeAsync<string>(nameof(Supplier.Email)),
                                webSite: await reader.GetSafeAsync<string>(nameof(Supplier.Website)),
                                phone: await reader.GetSafeAsync<string>(nameof(Supplier.Phone)),
                                fax: await reader.GetSafeAsync<string>(nameof(Supplier.Fax)),
                                notes: await reader.GetSafeAsync<string>(nameof(Supplier.Notes)),
                                address: address,
                                agent: agent));
                    }
                }

                //await Task.Delay(1000 * 3);

                return suppliers;
            }
            catch (InvalidCastException ex)
            {
                Debug.WriteLine(ex);
                return Array.Empty<Supplier>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Array.Empty<Supplier>();
            }
        }

        public override async Task<Supplier> GetByIdAsync(int id)
        {
            try
            {
                var queryParams = new { SearchedId = id };
                var query = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(queryParams.SearchedId)};";

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: query,
                    param: queryParams,
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
                    Address.Create(
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Supplier.Address.Country))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Supplier.Address.Province))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Supplier.Address.City))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Supplier.Address.Zip))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Supplier.Address.Street))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Supplier.Address.Number))]),
                    Agent.Create(
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Supplier.Agent.Name))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Supplier.Agent.Surname))],
                        (string)lastInsertedRow[DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Supplier.Agent.Phone))]));

                return supplier;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public override async Task<IReadOnlyCollection<Supplier>> GetByIdAsync(params int[] ids)
        {
            try
            {
                var query = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] in ({string.Join(",", ids)});";

                var suppliers = new List<Supplier>();

                await using (var reader = await SqlMapper.ExecuteReaderAsync(
                    cnn: this._connection,
                    sql: query,
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
                                Address.Create(
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Country))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Province))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.City))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Zip))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Street))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Number)))),
                                Agent.Create(
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Name))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Surname))),
                                    await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Phone))))));
                    }
                }

                return suppliers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public override async Task RemoveAsync(int id)
        {
            try
            {
                var queryParams = new { Id = id };

                var query = $"delete from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(queryParams.Id)};";

                var affectedRows = await SqlMapper.ExecuteAsync(
                    cnn: this._connection,
                    sql: query,
                    param: queryParams,
                    transaction: this._transaction);

                if (affectedRows <= 0)
                {
                    throw new KeyNotFoundException();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public override Task RemoveAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public override async Task UpdateAsync(int id, IEnumerable<KeyValuePair<string, object>> changes)
        {
            try
            {
                // TODO: find a way to avoid parameter keys collisions, like id / changes.id, maybe =<param_name> insread of @param_name ()
                var queryParams = new
                {
                    UpdateId = id,
                    Changes = changes
                };
                var flattenedChanges = DbSchemaHelper.FlattenChanges(new Dictionary<string, object>(changes));
                var sets = flattenedChanges.Select(x => $"[{x.Key}] = @{x.Key}").ToArray();
                var sql = $"update [{nameof(Supplier)}] set {string.Join(",", sets)} where [{nameof(Supplier.Id)}] = {id};"; // TODO: is it ok to encode id into string?

                var affectedRows = await SqlMapper.ExecuteAsync(
                    this._connection,
                    new CommandDefinition(
                        commandText: sql,
                        parameters: flattenedChanges,
                        transaction: this._transaction));

                if (affectedRows <= 0)
                {
                    throw new KeyNotFoundException();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion
    }
}