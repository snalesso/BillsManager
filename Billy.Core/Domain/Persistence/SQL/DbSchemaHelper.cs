using System.Collections.Generic;

namespace Billy.Domain.Persistence.SQL
{
    public static class DbSchemaHelper
    {
        public const char ComposedColumnFieldsSeparator = '_';

        public static string ComposeColumnName(params string[] columnNames)
        {
            return string.Join(ComposedColumnFieldsSeparator, columnNames);
        }

        public static IDictionary<string, object> FlattenChanges(IDictionary<string, object> changes)
        {
            var kvpToAdd = new List<KeyValuePair<string, object>>();
            var kvpToRemove = new List<KeyValuePair<string, object>>();

            foreach (var item in changes)
            {
                switch (item.Value)
                {
                    case IDictionary<string, object> dict:
                        //case IReadOnlyDictionary<string, object> roDict:
                        //case IEnumerable<KeyValuePair<string, object>> kvps:

                        FlattenChanges(dict);

                        foreach (var kvp in dict)
                        {
                            kvpToAdd.Add(new KeyValuePair<string, object>(ComposeColumnName(item.Key, kvp.Key), kvp.Value));
                        }

                        kvpToRemove.Add(item);

                        break;
                }
            }

            foreach (var item in kvpToRemove)
            {
                changes.Remove(item);
            }

            foreach (var item in kvpToAdd)
            {
                changes.Add(item);
            }

            return changes;
        }
    }
}
