using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Threading.Tasks;

namespace Billy.Domain.Persistence
{
    public static class DbDataReaderExtensions
    {
        // TODO: consider inlining
        //[DebuggerNonUserCode]
        public static async Task<T> GetSafeAsync<T>(this DbDataReader reader, string name, T fallbackValue = default)
        {
            try
            {
                // TODO: investigate performance and difference brtween sync/async
                if (await reader.IsDBNullAsync(name).ConfigureAwait(false))
                    return fallbackValue;

                return await reader.GetFieldValueAsync<T>(name).ConfigureAwait(false);
            }
            catch (SqlNullValueException ex) { return fallbackValue; }
            catch (IndexOutOfRangeException ex) { return fallbackValue; }
            catch (InvalidCastException ex) { return fallbackValue; }
            catch { throw; }
        }
    }
}
