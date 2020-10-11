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
            return expression.NodeType switch
            {
                ExpressionType.MemberAccess => ((MemberExpression)expression).Member.Name,
                ExpressionType.Convert => GetMemberName(((UnaryExpression)expression).Operand),
                _ => throw new NotSupportedException(expression.NodeType.ToString()),
            };
        }

        public static string GetMemberName<TObject, TMember>(this Expression<Func<TObject, TMember>> expression)
        {
            return GetMemberName(expression.Body);
        }
    }
}
