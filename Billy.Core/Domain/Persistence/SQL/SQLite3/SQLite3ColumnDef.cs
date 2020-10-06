using System;
using System.Data.SQLite;

namespace Billy.Domain.Persistence.SQL.SQLite3
{
    internal class SQLite3ColumnDef : SQLColumnDef
    {
        public SQLite3ColumnDef(string name, TypeAffinity typeAffinity, bool isNotNull = false, bool isPK = false, bool isUnique = false)
            : base(name, GetTypeName(typeAffinity), isNotNull, isPK, isUnique)
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
}