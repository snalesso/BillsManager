using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Billy.Core.Domain.Persistence
{
    public static class DbDataReaderExtensions
    {
        [DebuggerNonUserCode]
        public static async Task<T> GetSafeAsync<T>(this DbDataReader reader, string name, T fallbackValue = default)
        {
            try
            {
                return await reader.GetFieldValueAsync<T>(name)
                    //.ConfigureAwait(false)
                    ;
            }
            catch (SqlNullValueException) { return fallbackValue; }
            catch (IndexOutOfRangeException) { return fallbackValue; }
            catch { throw; }
        }
    }
}
