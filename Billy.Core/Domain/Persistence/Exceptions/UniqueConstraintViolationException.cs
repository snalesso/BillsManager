using System;

namespace Billy.Domain.Persistence.Exceptions
{
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(
            string tableName,
            string columnName)
        {
            this.TableName = tableName?.TrimToNull() ?? throw new ArgumentException($"'{nameof(tableName)}' cannot be null or whitespace", nameof(tableName));
            this.ColumnName = columnName?.TrimToNull() ?? throw new ArgumentException($"'{nameof(columnName)}' cannot be null or whitespace", nameof(columnName));
        }

        public string TableName { get; }
        public string ColumnName { get; }
    }
}
