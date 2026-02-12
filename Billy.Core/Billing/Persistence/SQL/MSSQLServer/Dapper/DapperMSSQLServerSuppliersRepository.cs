using Billy.Billing.Models;
using Billy.Domain.Persistence;
using Billy.Domain.Persistence.SQL;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
//using Billy.Domain.Persistence.SQL.MSSQLServer;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.MSSQLServer.Dapper
{
    public class DapperMSSQLServerSuppliersRepository : MSSQLServerSuppliersRepository
    {
        #region ctor

        public DapperMSSQLServerSuppliersRepository(
            SqlConnection connection,
            SqlTransaction transaction = null)
            : base(connection, transaction)
        {
        }

        #endregion

        #region methods

        #region READ

        #region helpers

        // TODO: get rid of this trick?
        private SqlTransaction GetTransactionIfAvailable() => this._transaction?.Connection != null ? this._transaction : null;

        #endregion

        public override async Task<IReadOnlyCollection<Supplier>> GetMultipleAsync(SupplierCriteria criteria = null)
        {
            try
            {
                var suppliers = new List<Supplier>();

                await using (var reader = await SqlMapper.ExecuteReaderAsync(
                    cnn: this._connection,
                    sql: $"select * from [{nameof(Supplier)}];",
                    transaction: this.GetTransactionIfAvailable()).ConfigureAwait(false))
                {
                    while (await reader.ReadAsync())
                    {
                        var address = Address.Create(
                            country: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Country))).ConfigureAwait(false),
                            province: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Province))).ConfigureAwait(false),
                            city: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.City))).ConfigureAwait(false),
                            zip: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Zip))).ConfigureAwait(false),
                            street: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Street))).ConfigureAwait(false),
                            number: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Address), nameof(Address.Number))).ConfigureAwait(false));

                        var agent = Agent.Create(
                            name: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Name))).ConfigureAwait(false),
                            surname: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Surname))).ConfigureAwait(false),
                            phone: await reader.GetSafeAsync<string>(DbSchemaHelper.ComposeColumnName(nameof(Supplier.Agent), nameof(Agent.Phone))).ConfigureAwait(false));

                        suppliers.Add(
                            new Supplier(
                                id: await reader.GetSafeAsync<int>(nameof(Supplier.Id)).ConfigureAwait(false),
                                name: await reader.GetSafeAsync<string>(nameof(Supplier.Name)).ConfigureAwait(false),
                                eMail: await reader.GetSafeAsync<string>(nameof(Supplier.Email)).ConfigureAwait(false),
                                webSite: await reader.GetSafeAsync<string>(nameof(Supplier.Website)).ConfigureAwait(false),
                                phone: await reader.GetSafeAsync<string>(nameof(Supplier.Phone)).ConfigureAwait(false),
                                fax: await reader.GetSafeAsync<string>(nameof(Supplier.Fax)).ConfigureAwait(false),
                                notes: await reader.GetSafeAsync<string>(nameof(Supplier.Notes)).ConfigureAwait(false),
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

        public override async Task<Supplier> GetByIdAsync(long id)
        {
            try
            {
                var queryParams = new { SearchedId = id };
                var query = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(queryParams.SearchedId)};";

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: query,
                    param: queryParams,
                    transaction: this.GetTransactionIfAvailable()).ConfigureAwait(false) as IDictionary<string, object>;

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

        public override Task<Supplier> GetSingleAsync(SupplierCriteria criteria = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region WRITE

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
                        transaction: this._transaction)).ConfigureAwait(false);

                sql = $"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(Supplier.Id)};";

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: sql, //$"select * from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = "+id,
                    param: new Dictionary<string, object>()
                    {
                        { nameof(Supplier.Id), id }
                    },
                    transaction: this._transaction).ConfigureAwait(false)
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

        public override async Task UpdateAsync(long id, IEnumerable<KeyValuePair<string, object>> changes)
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
                        transaction: this._transaction)).ConfigureAwait(false);

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

        public override async Task RemoveAsync(long id)
        {
            try
            {
                var queryParams = new { Id = id };

                var query = $"delete from [{nameof(Supplier)}] where [{nameof(Supplier.Id)}] = @{nameof(queryParams.Id)};";

                var affectedRows = await SqlMapper.ExecuteAsync(
                    cnn: this._connection,
                    sql: query,
                    param: queryParams,
                    transaction: this._transaction).ConfigureAwait(false);

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

        #endregion
    }
}