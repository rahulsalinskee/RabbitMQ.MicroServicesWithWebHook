using Product.API.Repository.FilterServices.Services;
using System.Linq.Expressions;
using System.Reflection;

namespace Product.API.Repository.FilterServices.Implementations
{
    public class FilterImplementation<T> : IFilterService<T>
    {
        private const string EACH_ITEM = "eachItem";

        public IQueryable<T?> ApplyFilterOn(IQueryable<T?> queryOn, string? columnName, string? filterKeyWord)
        {
            if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(filterKeyWord))
            {
                return queryOn;
            }

            /* Get the property info for the specified column name */
            var propertyInfo = typeof(T).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo is null || propertyInfo.PropertyType != typeof(string))
            {
                return queryOn;
            }

            /* Create expression: eachItem => eachItem != null && eachItem.PropertyName != null && eachItem.PropertyName.Contains(filterKeyword) */
            var parameter = Expression.Parameter(typeof(T?), EACH_ITEM);

            /* Check if the item itself is not null */
            var itemNotNullCheck = Expression.NotEqual(parameter, Expression.Constant(null, typeof(T?)));

            /* Convert nullable parameter to non-nullable for property access */
            var nonNullableParameter = Expression.Convert(parameter, typeof(T));

            /* Access the property dynamically */
            var property = Expression.Property(nonNullableParameter, propertyInfo);

            /* Check if property value is not null */
            var propertyNotNullCheck = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));

            /* Get the basic Contains method (without StringComparison) - EF Core can translate this */
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            if (containsMethod == null)
            {
                throw new InvalidOperationException("The Contains method was not found on the string type.");
            }

            /* For case-insensitive search, convert both property and search term to lowercase */
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

            if (toLowerMethod == null)
            {
                throw new InvalidOperationException("The ToLower method was not found on the string type.");
            }

            /* Convert property to lowercase */
            var propertyToLower = Expression.Call(property, toLowerMethod);

            /* Convert search keyword to lowercase */
            var filterKeyWordLower = filterKeyWord.ToLower();

            /* Create the Contains method call with lowercase comparison */
            var containsCall = Expression.Call(instance: propertyToLower, method: containsMethod, arguments: Expression.Constant(filterKeyWordLower));

            /* Combine all conditions: item != null && property != null && property.Contains(filterKeyword) */
            var combinedExpression = Expression.AndAlso(itemNotNullCheck, Expression.AndAlso(propertyNotNullCheck, containsCall));

            /* Create the lambda expression */
            var lambda = Expression.Lambda<Func<T?, bool>>(combinedExpression, parameter);

            return queryOn.Where(lambda);
        }
    }
}
