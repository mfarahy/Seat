using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Helpers
{
    public static class LinqHelper
    {
        public static IQueryable<T> WhereStringContains<T>(this IQueryable<T> query, string propertyName, string contains)
        {
            var parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, propertyName);
            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(contains, typeof(string));
            var containsExpression = Expression.Call(propertyExpression, method, someValue);

            return query.Where(Expression.Lambda<Func<T, bool>>(containsExpression, parameter));
        }
        
    }
}
