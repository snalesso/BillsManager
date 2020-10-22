using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Billy.Domain.Persistence
{
    public static class DbDataReaderExtensions
    {
        // TODO: consider inlining
        [DebuggerNonUserCode]
        public static async Task<T> GetSafeAsync<T>(this DbDataReader reader, string name, T fallbackValue = default)
        {
            try
            {
                return await reader.GetFieldValueAsync<T>(name).ConfigureAwait(false);
            }
            catch (SqlNullValueException) { return fallbackValue; }
            catch (IndexOutOfRangeException) { return fallbackValue; }
            catch (InvalidCastException) { return fallbackValue; }
            catch { throw; }
        }
    }
}
