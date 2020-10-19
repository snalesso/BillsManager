using System;

namespace Billy.Domain.Persistence.SQL
{
    internal abstract class SQLColumnDef<TTable, TColumn>
    {
        public SQLColumnDef(
            Func<TTable, TColumn> columnValueExtractor,
            string name, string typeName, bool isNotNull = false, bool isPK = false, bool isUnique = false)
        {
            this.ColumnValueExtractor = columnValueExtractor;
            this.Name = name;
            this.TypeName = typeName;
            this.IsNotNull = isNotNull;
            this.IsPK = isPK;
            this.IsUnique = isUnique;
        }

        public Func<TTable, TColumn> ColumnValueExtractor { get; }
        public string Name { get; }
        public string TypeName { get; }
        public bool IsNotNull { get; }
        public bool IsPK { get; }
        public bool IsUnique { get; }

        public abstract string GetSQLDef();
    }

    internal abstract class SQLColumnDef<TEditor>
    {
        public SQLColumnDef(
            Func<TEditor, string> columnValueExtractor,
            string name, string typeName, bool isNotNull = false, bool isPK = false, bool isUnique = false)
        {
            this.ColumnValueExtractor = columnValueExtractor;
            this.Name = name;
            this.TypeName = typeName;
            this.IsNotNull = isNotNull;
            this.IsPK = isPK;
            this.IsUnique = isUnique;
        }

        public Func<TEditor, string> ColumnValueExtractor { get; }
        public string Name { get; }
        public string TypeName { get; }
        public bool IsNotNull { get; }
        public bool IsPK { get; }
        public bool IsUnique { get; }

        public abstract string GetSQLDef();
    }
}