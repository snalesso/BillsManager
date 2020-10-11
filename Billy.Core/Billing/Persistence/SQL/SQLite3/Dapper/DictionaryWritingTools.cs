using System;
using System.Collections.Generic;

namespace Billy.Domain.Billing.Persistence.SQL.SQLite3.Dapper
{
    [Obsolete("Temporary solution")]
    internal class DictionaryWritingTools
    {
        public DictionaryWritingTools(IDictionary<string, object> dictionary, Func<string, string> buildPropertyName)
        {
            this.Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            this.BuildPropertyName = buildPropertyName ?? throw new ArgumentNullException(nameof(buildPropertyName));
        }

        public IDictionary<string, object> Dictionary { get; }
        public Func<string, string> BuildPropertyName { get; }
    }
}