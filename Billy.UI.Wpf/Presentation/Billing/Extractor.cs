using System;
using System.Linq.Expressions;
using Billy.Domain.Models;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class Extractor<TValueObject, TProperty>
        where TValueObject : ValueObject<TValueObject>
    {
        public Func<TValueObject, TProperty> PropertyGetter { get; }
        public Expression<Func<TValueObject, TProperty>> OriginalPropertyGetterExpr { get; }
    }
}