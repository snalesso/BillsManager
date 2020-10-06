using System;
using System.Collections.Generic;
using System.Text;
using Billy.Domain.Models;

namespace Billy.Core.Domain.Persistence.SQL.MSSQLServer
{
    internal static class MSSQLServerSchemaHelper
    {
        public const char PropertySeparator = '_';

        public static string ComposeColumnName(params string[] propertyNames)
        {
            return string.Join(PropertySeparator, propertyNames);
        }
    }

    internal struct ColParam
    {
        public string ColName { get; }
        public string Paramkey { get; }
        public object Value { get; }

        public ColParam(
            string colName,
            string paramkey,
            object value)
        {
            this.ColName = colName;
            this.Paramkey = paramkey;
            this.Value = value;
        }
    }
}