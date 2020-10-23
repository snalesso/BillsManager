using Billy.Billing.Models;
using Billy.Billing.Persistence.SQL.SQLite3;
using Billy.Domain.Persistence;
using Billy.Domain.Persistence.SQL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.SQL.SQLite3.Dapper
{
    public class DapperSQLite3BillsRepository : SQLite3BillsRepository
    {
        #region ctor

        public DapperSQLite3BillsRepository(
            SQLiteConnection connection,
            SQLiteTransactionBase transaction = null)
            : base(connection, transaction)
        {
        }

        #endregion

        #region READ

        #region helpers

        // TODO: get rid of this trick?
        private SQLiteTransactionBase GetTransactionIfAvailable() => this._transaction?.Connection != null ? this._transaction : null;

        #endregion

        public override async Task<IReadOnlyCollection<Bill>> GetMultipleAsync()
        {
            try
            {
                var bills = new List<Bill>();

                var cmd = new CommandDefinition(
                    commandText: $"select * from [{nameof(Bill)}]",
                    transaction: this.GetTransactionIfAvailable());

                //var resultsGrid = SqlMapper.QueryMultipleAsync(this._connection, cmd);
                //var x = await resultsGrid.Result.ReadAsync();

                await using (var reader = await SqlMapper.ExecuteReaderAsync(cnn: this._connection, cmd))
                {
                    while (await reader.ReadAsync())
                    {
                        // TODO: cache column names composition
                        bills.Add(
                            new Bill(
                                id: await reader.GetSafeAsync<long>(nameof(Bill.Id)),
                                supplierId: await reader.GetSafeAsync<long>(nameof(Bill.SupplierId)),
                                releaseDate: await reader.GetSafeAsync<DateTime>(nameof(Bill.ReleaseDate)),
                                dueDate: await reader.GetSafeAsync<DateTime>(nameof(Bill.DueDate)),
                                paymentDate: await reader.GetSafeAsync<DateTime?>(nameof(Bill.PaymentDate)),
                                registrationDate: await reader.GetSafeAsync<DateTime>(nameof(Bill.RegistrationDate)),
                                amount: await reader.GetSafeAsync<double>(nameof(Bill.Amount)),
                                agio: await reader.GetSafeAsync<double>(nameof(Bill.Agio)),
                                additionalCosts: await reader.GetSafeAsync<double>(nameof(Bill.AdditionalCosts)),
                                code: await reader.GetSafeAsync<string>(nameof(Bill.Code)),
                                notes: await reader.GetSafeAsync<string>(nameof(Bill.Notes))
                                ));
                    }
                }

                return bills;
            }
            catch (InvalidCastException ex)
            {
                Debug.WriteLine(ex);
                return Array.Empty<Bill>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return Array.Empty<Bill>();
            }
        }

        public override async Task<Bill> GetByIdAsync(long id)
        {
            try
            {
                var queryParams = new { SearchedId = id };
                var query = $"select * from [{nameof(Bill)}] where \"{nameof(Bill.Id)}\" = @{nameof(queryParams.SearchedId)};";

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    sql: query,
                    param: queryParams,
                    transaction: this.GetTransactionIfAvailable()) as IDictionary<string, object>;

                // TODO: create helper method dictionary -> bill
                var bill = new Bill(
                    (long)lastInsertedRow[nameof(Bill.Id)],
                    (long)lastInsertedRow[nameof(Bill.SupplierId)],
                    (DateTime)lastInsertedRow[nameof(Bill.ReleaseDate)],
                    (DateTime)lastInsertedRow[nameof(Bill.DueDate)],
                    (DateTime?)lastInsertedRow[nameof(Bill.PaymentDate)],
                    (DateTime)lastInsertedRow[nameof(Bill.RegistrationDate)],
                    (double)lastInsertedRow[nameof(Bill.Amount)],
                    (double)lastInsertedRow[nameof(Bill.Agio)],
                    (double)lastInsertedRow[nameof(Bill.AdditionalCosts)],
                    (string)lastInsertedRow[nameof(Bill.Code)],
                    (string)lastInsertedRow[nameof(Bill.Notes)]);

                return bill;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        #endregion

        #region WRITE

        public override async Task<Bill> CreateAndAddAsync(IEnumerable<KeyValuePair<string, object>> data)
        {
            try
            {
                var flattenedData = DbSchemaHelper.FlattenChanges(new Dictionary<string, object>(data));
                var columns = flattenedData.Select(x => "[" + x.Key + "]");
                //var values = data.Select(x => x.Value);
                var values = flattenedData.Select(x => "@" + x.Key);

                // TODO: consider using QueryMultipleAsync
                var id = await SqlMapper.ExecuteScalarAsync<int>(
                    this._connection,
                    new CommandDefinition(
                        commandText: string.Join(";",
                            $"insert into [{nameof(Bill)}] ({string.Join(",", columns)}) values ({string.Join(",", values)})",
                            "SELECT last_insert_rowid()"),
                        parameters: flattenedData.Select(kvp => new KeyValuePair<string, object>("@" + kvp.Key, kvp.Value)),
                        transaction: this._transaction));

                var lastInsertedCmd = new CommandDefinition(
                    commandText: $"select * from [{nameof(Bill)}] where \"{nameof(Bill.Id)}\" = @{nameof(Bill.Id)}",
                    parameters: new Dictionary<string, object>()
                    {
                        [nameof(Bill.Id)] = id
                    },
                    transaction: this._transaction);

                var lastInsertedRow = await SqlMapper.QueryFirstOrDefaultAsync(
                    cnn: this._connection,
                    lastInsertedCmd)
                    as IDictionary<string, object>;

                // TODO: improve parsing, handling nulls etc.
                var insertedBill = new Bill(
                    (long)lastInsertedRow[nameof(Bill.Id)],
                    (long)lastInsertedRow[nameof(Bill.SupplierId)],
                    (DateTime)lastInsertedRow[nameof(Bill.ReleaseDate)],
                    (DateTime)lastInsertedRow[nameof(Bill.DueDate)],
                    (DateTime?)lastInsertedRow[nameof(Bill.PaymentDate)],
                    (DateTime)lastInsertedRow[nameof(Bill.RegistrationDate)],
                    (double)lastInsertedRow[nameof(Bill.Amount)],
                    (double)lastInsertedRow[nameof(Bill.Agio)],
                    (double)lastInsertedRow[nameof(Bill.AdditionalCosts)],
                    (string)lastInsertedRow[nameof(Bill.Code)],
                    (string)lastInsertedRow[nameof(Bill.Notes)]);

                //await Task.Delay(1000 * 3);

                return insertedBill;
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
                var sql = $"update [{nameof(Bill)}] set {string.Join(",", sets)} where \"{nameof(Bill.Id)}\" = {id};"; // TODO: is it ok to encode id into string?

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

        public override async Task RemoveAsync(long id)
        {
            try
            {
                var queryParams = new { Id = id };

                var query = $"delete from [{nameof(Bill)}] where \"{nameof(Bill.Id)}\" = @{nameof(queryParams.Id)};";

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

        #endregion
    }
}
