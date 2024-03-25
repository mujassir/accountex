using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AccountEx.Common
{

    public static class QueryableExtensions
    {
        private static readonly MethodInfo StringContainsMethod;
        private static readonly MethodInfo StringStartsWithMethod;
       
        
        static QueryableExtensions()
        {
            var singleStringParam = new[] { typeof(string) };
            StringContainsMethod = typeof(string).GetMethod("Contains", singleStringParam);
            StringStartsWithMethod = typeof(string).GetMethod("StartsWith", singleStringParam);
        }
        
        public static IQueryable<TResult> LeftOuterJoin<TSource, TInner, TKey, TResult>(this IQueryable<TSource> source, IQueryable<TInner> other, Func<TSource, TKey> func, Func<TInner, TKey> innerkey, Func<TSource, TInner, TResult> res)
        {
            return from f in source
                   join b in other on func.Invoke(f) equals innerkey.Invoke(b) into g
                   from result in g.DefaultIfEmpty()
                   select res.Invoke(f, result);
        }

        public static IQueryable<T> AppendTextFilter<T>(this IQueryable<T> queryable, Expression<Func<T, string>> memberSelector, MatchCondition condition, string value)
        {
            Expression expression = null;
            switch (condition)
            {
                case MatchCondition.StartsWith:
                    expression = Expression.Call(
                                    memberSelector.Body,
                                    StringStartsWithMethod,
                                    Expression.Constant(value));
                    break;

                case MatchCondition.Equal:
                    expression = Expression.Equal(
                                    memberSelector.Body,
                                    Expression.Constant(value));
                    break;

                case MatchCondition.Contains:
                    expression = Expression.Call(
                                    memberSelector.Body,
                                    StringContainsMethod,
                                    Expression.Constant(value));
                    break;

                default:
                    throw new NotSupportedException(string.Format("'{0}' is not a supported condition", condition));
            }

            var lambda = Expression.Lambda<Func<T, bool>>(
                            expression,
                            memberSelector.Parameters);
            return queryable.Where(lambda);
        }

        #region Private expression tree helpers

        private static LambdaExpression GenerateSelector<TEntity>(String propertyName, out Type resultType) where TEntity : class
        {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                var childProperties = propertyName.Split('.');
                property = typeof(TEntity).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (var i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(TEntity).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }
        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, String fieldName) where TEntity : class
        {
            var type = typeof(TEntity);
            Type selectorResultType;
            var selector = GenerateSelector<TEntity>(fieldName, out selectorResultType);
            var resultExp = Expression.Call(typeof(Queryable), methodName,
                            new[] { type, selectorResultType },
                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }
        #endregion
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "ThenBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = GenerateMethodCall(source, "ThenByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        public static IOrderedQueryable<TEntity> OrderUsingSortExpression<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            var orderFields = sortExpression.Split(',');
            IOrderedQueryable<TEntity> result = null;
            for (var currentFieldIndex = 0; currentFieldIndex < orderFields.Length; currentFieldIndex++)
            {
                var expressionPart = orderFields[currentFieldIndex].Trim().Split(' ');
                var sortField = expressionPart[0];
                var sortDescending = (expressionPart.Length == 2) && (expressionPart[1].Equals("DESC", StringComparison.OrdinalIgnoreCase));
                if (sortDescending)
                {
                    result = currentFieldIndex == 0 ? source.OrderByDescending(sortField) : result.ThenByDescending(sortField);
                }
                else
                {
                    result = currentFieldIndex == 0 ? source.OrderBy(sortField) : result.ThenBy(sortField);
                }
            }
            return result;
        }


    }
    public enum MatchCondition
    {
        StartsWith,
        Contains,
        Equal,
    }
}