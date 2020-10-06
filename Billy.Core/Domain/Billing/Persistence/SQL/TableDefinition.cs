using Billy.Domain.Persistence.SQL;
using System.Collections.Generic;
using System.Linq;

namespace Billy.Domain.Billing.Persistence.SQL
{
    internal class TableDefinition
    {
        public TableDefinition(string name, IEnumerable<SQLColumnDef> columnDefs)
        {
            this.Name = name;
            this.ColumnDefs = columnDefs.ToList().AsReadOnly();
        }

        public string Name { get; }
        public IReadOnlyCollection<SQLColumnDef> ColumnDefs { get; }
    }
}