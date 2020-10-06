using System.Collections.Generic;
using System.Collections.Immutable;

namespace Billy.Domain.Persistence.SQL
{
    internal class TableDefinition<TTable, TEditor>
    {
        public TableDefinition(string name, IEnumerable<SQLColumnDef<TEditor>> columnDefs)
        {
            this.Name = name;
            this.ColumnDefs = columnDefs.ToImmutableList();
        }

        public TableDefinition(IEnumerable<SQLColumnDef<TEditor>> columnDefs) : this(typeof(TEditor).Name, columnDefs)
        {
        }

        public string Name { get; }
        public IReadOnlyCollection<SQLColumnDef<TEditor>> ColumnDefs { get; }
    }
}