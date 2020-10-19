using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Billy.Domain.Persistence.SQL.SQLite3.Dapper
{
    [Obsolete("Old, unused. Not deleted just to use it as a snippet.")]
    public abstract class DapperSQLite3Repository
    {
        private readonly SQLiteConnection connection;

        public DapperSQLite3Repository(SQLiteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        protected async Task<T> CommandAsync<T>(Func<SQLiteConnection, SQLiteTransactionBase, int, Task<T>> command)
        {
            await this.connection.OpenAsync();

            using (var transaction = this.connection.BeginTransaction())
            {
                try
                {
                    var result = await command(this.connection, transaction, 500);

                    transaction.Commit();

                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    //Logger.Instance.Error(ex);
                    throw;
                }
            }
        }

        protected async Task<T> GetSingleAsync<T>(Func<SQLiteConnection, SQLiteTransactionBase, int, Task<T>> command)
        {
            return await this.CommandAsync(command);
        }

        protected async Task<IList<T>> SelectAsync<T>(Func<SQLiteConnection, SQLiteTransactionBase, int, Task<IList<T>>> command)
        {
            return await this.CommandAsync(command);
        }

        protected async Task ExecuteAsync(string sql, object parameters)
        {
            await this.CommandAsync(async (conn, trn, timeout) =>
            {
                await conn.ExecuteAsync(sql, parameters, trn, timeout);
                return 1;
            });
        }

        protected async Task<T> GetSingleAsync<T>(string sql, object parameters)
        {
            return await this.CommandAsync(async (conn, trn, timeout) =>
            {
                T result = await conn.QuerySingleAsync<T>(sql, parameters, trn, timeout);
                return result;
            });
        }

        protected async Task<IList<T>> SelectAsync<T>(string sql, object parameters)
        {
            return await this.CommandAsync<IList<T>>(async (conn, trn, timeout) =>
            {
                var result = (await conn.QueryAsync<T>(sql, parameters, trn, timeout)).ToList();
                return result;
            });
        }
    }
}