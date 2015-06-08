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
                throw new ArgumentNullException("trackedObject");

            this.trackedObject = trackedObject;
            this.validationRules = new Dictionary<string, ValidationAttribute[]>();

            this.MapValidationRules();
        }

        protected void MapValidationRules()
        {
            foreach (PropertyInfo pi in this.trackedObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                ValidationAttribute[] valAtts = (ValidationAttribute[])pi.GetCustomAttributes(typeof(ValidationAttribute), true);

                if (valAtts.Any()) // TODO: what if no validation rules?
                    this.validationRules.Add(pi.Name, valAtts);
            }
        }

        protected object GetPropertyValue(string propertyName)
        {
            return typeof(T).GetProperty(propertyName).GetValue(this.trackedObject);
        }

        public IEnumerable<string> GetErrorsForProperty(string propertyName)
        {
            if (this.validationRules.ContainsKey(propertyName))
            {
                var validations = this.validationRules[propertyName];

                var errors = validations
                   .Where(vr =>
                   {
                       var v = !vr.IsValid(this.GetPropertyValue(propertyName));
                       return v;
                   })
                   .Select(vr =>
                   {
                       var e = vr.ErrorMessage;
                       return e;
                   })
                   .ToArray();

                return errors;
            }
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetAllErrors() // TODO: optimize for speed
        {
            return this.validationRules.Keys.SelectMany(k => this.GetErrorsForProperty(k));
        }
    }
}