using System.Collections.Generic;
using Billy.Domain.Billing.Models;

namespace Billy.Domain.Billing.Persistence.SQL.SQLite3.Dapper
{
    internal static class DapperSQLite3ValueObjectSplitter
    {
        public const string PropertySeparator = ".";

        public static string SubProp(params string[] propertyNames)
        {
            return string.Format(PropertySeparator, propertyNames);
        }

        public static Supplier ToSupplier(this IDictionary<string, object> values)
        {
            // TODO: create helper method dictionary -> supplier
            return new Supplier(
                (int)values[nameof(Supplier.Id)],
                (string)values[nameof(Supplier.Name)],
                (string)values[nameof(Supplier.Email)],
                (string)values[nameof(Supplier.Website)],
                (string)values[nameof(Supplier.Phone)],
                (string)values[nameof(Supplier.Fax)],
                (string)values[nameof(Supplier.Notes)],

                //(Address)values[nameof(Supplier.Address)],
                Address.Create(
                    (string)values[SubProp(nameof(Supplier.Address), nameof(Address.Country))],
                    (string)values[SubProp(nameof(Supplier.Address), nameof(Address.Province))],
                    (string)values[SubProp(nameof(Supplier.Address), nameof(Address.City))],
                    (string)values[SubProp(nameof(Supplier.Address), nameof(Address.Zip))],
                    (string)values[SubProp(nameof(Supplier.Address), nameof(Address.Street))],
                    (string)values[SubProp(nameof(Supplier.Address), nameof(Address.Number))]),

                //(Agent)values[nameof(Supplier.Agent)]
                Agent.Create(
                    (string)values[SubProp(nameof(Supplier.Agent), nameof(Agent.Name))],
                    (string)values[SubProp(nameof(Supplier.Agent), nameof(Agent.Surname))],
                    (string)values[SubProp(nameof(Supplier.Agent), nameof(Agent.Phone))]));
        }

        public static IDictionary<string, object> ToDictionary(this Supplier supplier, DictionaryWritingTools tools = null)
        {
            var dict = tools?.Dictionary ?? new Dictionary<string, object>();
            dict.Add(nameof(Supplier.Id), supplier.Id);
            dict.Add(nameof(Supplier.Name), supplier.Name);
            dict.Add(nameof(Supplier.Email), supplier.Email);
            dict.Add(nameof(Supplier.Website), supplier.Website);
            dict.Add(nameof(Supplier.Phone), supplier.Phone);
            dict.Add(nameof(Supplier.Fax), supplier.Fax);
            dict.Add(nameof(Supplier.Notes), supplier.Notes);

            supplier.Address.ToDictionary(new DictionaryWritingTools(dict, (string propName) => SubProp(nameof(Supplier.Address), propName)));
            supplier.Agent.ToDictionary(new DictionaryWritingTools(dict, (string propName) => SubProp(nameof(Supplier.Agent), propName)));

            //if (supplier.Address != null)
            //{
            //    //(Address)values[nameof(Supplier.Address)
            //    dict.Add(SubProp(nameof(Supplier.Address), nameof(Address.Country)), supplier.Address.Country);
            //    dict.Add(SubProp(nameof(Supplier.Address), nameof(Address.Province)), supplier.Address.Province);
            //    dict.Add(SubProp(nameof(Supplier.Address), nameof(Address.City)), supplier.Address.City);
            //    dict.Add(SubProp(nameof(Supplier.Address), nameof(Address.Zip)), supplier.Address.Zip);
            //    dict.Add(SubProp(nameof(Supplier.Address), nameof(Address.Street)), supplier.Address.Country);
            //    dict.Add(SubProp(nameof(Supplier.Address), nameof(Address.Number)), supplier.Address.Number);
            //}

            //if (supplier.Agent != null)
            //{
            //    //(Agent)valuesdict.Add(nameof(Supplier.Agent)]
            //    dict.Add(SubProp(nameof(Supplier.Agent), nameof(Agent.Name)), supplier.Agent.Name);
            //    dict.Add(SubProp(nameof(Supplier.Agent), nameof(Agent.Surname)), supplier.Agent.Surname);
            //    dict.Add(SubProp(nameof(Supplier.Agent), nameof(Agent.Phone)), supplier.Agent.Phone);
            //}

            return dict;
        }

        public static IDictionary<string, object> ToDictionary(this Address address, DictionaryWritingTools tools = null)
        {
            var dict = tools?.Dictionary ?? new Dictionary<string, object>();

            if (address != null)
            {
                dict.Add(tools.BuildPropertyName(nameof(Address.Country)), address.Country);
                dict.Add(tools.BuildPropertyName(nameof(Address.Province)), address.Province);
                dict.Add(tools.BuildPropertyName(nameof(Address.City)), address.City);
                dict.Add(tools.BuildPropertyName(nameof(Address.Zip)), address.Zip);
                dict.Add(tools.BuildPropertyName(nameof(Address.Street)), address.Street);
                dict.Add(tools.BuildPropertyName(nameof(Address.Number)), address.Number);
            }

            return dict;
        }

        public static IDictionary<string, object> ToDictionary(this Agent agent, DictionaryWritingTools tools = null)
        {
            var dict = tools?.Dictionary ?? new Dictionary<string, object>();

            if (agent != null)
            {
                dict.Add(tools.BuildPropertyName(nameof(Agent.Name)), agent.Name);
                dict.Add(tools.BuildPropertyName(nameof(Agent.Surname)), agent.Surname);
                dict.Add(tools.BuildPropertyName(nameof(Agent.Phone)), agent.Phone);
            }

            return dict;
        }
    }
}