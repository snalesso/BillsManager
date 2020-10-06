namespace Billy.Domain.Persistence.SQL
{
    internal abstract class SQLColumnDef
    {
        public SQLColumnDef(string name, string typeName, bool isNotNull = false, bool isPK = false, bool isUnique = false)
        {
            this.Name = name;
            this.TypeName = typeName;
            this.IsNotNull = isNotNull;
            this.IsPK = isPK;
            this.IsUnique = isUnique;
        }

        public string Name { get; }
        public string TypeName { get; }
        public bool IsNotNull { get; }
        public bool IsPK { get; }
        public bool IsUnique { get; }

        public abstract string GetSQLDef();
    }
}