using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BillsManager.ViewModels.Validation
{
    public class ValidationRulesTracker<T>
        where T : class
    {
        private readonly T trackedObject;
        private readonly Dictionary<string, ValidationAttribute[]> validationRules;

        public ValidationRulesTracker(T trackedObject)
        {
            if (trackedObject == null)
                throw new ArgumentNullException("trackedObject cannot be null");

            this.trackedObject = trackedObject;
            this.validationRules = new Dictionary<string, ValidationAttribute[]>();

            this.MapValidationRules();
        }

        protected void MapValidationRules()
        {
            foreach (PropertyInfo pi in this.trackedObject.GetType().GetProperties())
            {
                ValidationAttribute[] valAtts = (ValidationAttribute[])pi.GetCustomAttributes(typeof(ValidationAttribute), true);

                if (valAtts.Count() > 0) // TODO: what if no validation rules?
                    this.validationRules.Add(pi.Name, valAtts);
            }
        }

        protected object GetPropertyValue(string propertyName)
        {
            return typeof(T).GetProperty(propertyName).GetValue(this.trackedObject, null);
        }

        public string GetErrorsForProperty(string propertyName)
        {
            var validations = this.validationRules[propertyName];

            var errors = this.validationRules[propertyName]
                .Where(vr =>
                {
                    var v = !vr.IsValid(this.GetPropertyValue(propertyName));
                    return v;
                })
                .Select(vr =>
                {
                    var e = vr.ErrorMessage;
                    return e;
                });

            return string.Join(Environment.NewLine, errors);
        }

        public string GetAllErrors() // TODO: optimize for speed
        {
            string errors = string.Empty;

            foreach (KeyValuePair<string, ValidationAttribute[]> kvp in this.validationRules)
            {
                var propVal = this.GetPropertyValue(kvp.Key);

                foreach (var va in kvp.Value)
                {
                    bool valid = va.IsValid(propVal);

                    if (!valid)
                    {
                        if (!string.IsNullOrEmpty(errors))
                            errors += Environment.NewLine;
                        errors += va.ErrorMessage;
                    }
                }
            }

            return errors;
        }
    }
}