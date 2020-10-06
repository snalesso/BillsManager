using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Billy
{
    public static class PropertyHelper
    {
        public static string GetMemberName(this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member.Name;

                case ExpressionType.Convert:
                    return GetMemberName(((UnaryExpression)expression).Operand);

                default:
                    throw new NotSupportedException(expression.NodeType.ToString());
            }
        }

        public static string GetMemberName<TObject, TMember>(this Expression<Func<TObject, TMember>> expression)
        {
            return GetMemberName(expression.Body);
        }
    }
}
