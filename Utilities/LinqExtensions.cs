using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static partial class Extensions
{
    public static bool HasOverride(this object obj, string nameOfMethod)
    {
        var enumerableMethod = obj.GetType()
            .GetMethod(nameOfMethod, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        return enumerableMethod != null && enumerableMethod.DeclaringType != typeof(SchedulableBehaviour);
    }
    public static IEnumerable<T> OrderByWithoutNegatives<T>(this IEnumerable<T> source, Func<T, int> selector)
    {
        return source
            .Where(x => selector(x) >= 0)
            .OrderBy(selector);
    }
}