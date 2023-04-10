using System.Linq.Expressions;
using System.Reflection;

namespace Core.Extentions;

public static class Extentions
{
    public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
    {
        var type = typeof(T);
        var parameter = Expression.Parameter(type, "p");
        PropertyInfo property;
        Expression propertyAccess;
        if (propertyName.Contains('.'))
        {
            String[] childProperties = propertyName.Split('.');
            property = type.GetProperty(childProperties[0]);
            propertyAccess = Expression.MakeMemberAccess(parameter, property);
            for (int i = 1; i < childProperties.Length; i++)
            {
                property = property.PropertyType.GetProperty(childProperties[i]);
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
        }
        else
        {
            property = typeof(T).GetProperty(propertyName);
            propertyAccess = Expression.MakeMemberAccess(parameter, property);
        }
        var orderByExp = Expression.Lambda(propertyAccess, parameter);
        MethodCallExpression resultExp = Expression.Call(typeof(Queryable),
                                                         ascending ? "OrderBy" : "OrderByDescending",
                                                         new[] { type, property.PropertyType }, source.Expression,
                                                         Expression.Quote(orderByExp));
        return source.Provider.CreateQuery<T>(resultExp);
    }
}
