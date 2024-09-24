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

    public static void CallAll(this List<MethodObject> source)
    {
        foreach (var x in source) x.Call();
    }

    public static List<List<MethodObject>> CorrectAllMO(this List<List<MethodObject>> source)
    {
        return source.SelectMany(subList => subList)
            .GroupBy(method => method.customOrder)
            .OrderBy(group => group.Key)
            .Select(group => group.ToList())
            .ToList();
    }
    
    public static List<MethodObject> 
        ConvertToMethodObjects<T>(this List<MethodInfo> source, object target) 
        where T : UpdateBaseAttribute
    {
        return source.FindAll(x => x.GetCustomAttribute<T>() != null).ConvertAll(x =>
        {
            var del = (Action)Delegate.CreateDelegate(typeof(Action), target, x);

            return new MethodObject(target, x, del, x.GetCustomAttribute<T>().order);
        });
    }

    public static List<Action> ConvertToDelegateActions(this List<MethodInfo> source, object target)
    {
        return source.ConvertAll(x => (Action)Delegate.CreateDelegate(typeof(Action), target, x));
    }
}