using System;
using System.Data.SQLite;

namespace Billy.Domain.Persistence.SQL.SQLite3
{
    internal class SQLite3ColumnDef<TTable, TColumn> : SQLColumnDef<TTable, TColumn>
    {
        public SQLite3ColumnDef(
            Func<TTable, TColumn> columnValueExtractor,
            string name,
            TypeAffinity typeAffinity,
            bool isNotNull = false,
            bool isPK = false,
            bool isUnique = false)
            : base(columnValueExtractor, name, GetTypeName(typeAffinity), isNotNull, isPK, isUnique)
        {
        }

        public override string GetSQLDef()
        {
            var sql = $"[{this.Name}] {this.TypeName}";

            if (this.IsPK)
                sql += " PRIMARY KEY";

            if (this.IsNotNull)
                sql += " NOT NULL";

            if (this.IsUnique)
                sql += " UNIQUE";

            return sql;
        }

        private static string GetTypeName(TypeAffinity typeAffinity)
        {
            return Enum.GetName(typeof(TypeAffinity), typeAffinity);
        }
    }

    internal class SQLite3ColumnDef<TEdit> : SQLColumnDef<TEdit>
    {
        public SQLite3ColumnDef(
            Func<TEdit, string> columnValueExtractor,
            string name,
            TypeAffinity typeAffinity,
            bool isNotNull = false,
            bool isPK = false,
            bool isUnique = false)
            : base(columnValueExtractor, name, GetTypeName(typeAffinity), isNotNull, isPK, isUnique)
        {
        }

        public override string GetSQLDef()
        {
            var sql = $"[{this.Name}] {this.TypeName}";

            if (this.IsPK)
                sql += " PRIMARY KEY AUTOINCREMENT";

            if (this.IsNotNull && !this.IsPK)
                sql += " NOT NULL";

            if (this.IsUnique && !this.IsPK)
                sql += " UNIQUE";

            return sql;
        }

        private static string GetTypeName(TypeAffinity typeAffinity)
        {
            return Enum.GetName(typeof(TypeAffinity), typeAffinity);
        }
    }
}