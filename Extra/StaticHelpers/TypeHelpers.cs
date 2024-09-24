using System;
using System.Collections;

public static class TypeHelpers
{
    public static object GetDefault(Type type)
    {
        if (type == typeof(string)) return "";
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    // Extension method to add a new item to the IList
    public static object AddNewItem(this IList list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));

        // Get the type of the elements in the list
        Type listType = list.GetType().GetGenericArguments()[0];
        
        // Ensure the type has a parameterless constructor
        if (listType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new InvalidOperationException("The type must have a parameterless constructor.");
        }

        // Create a new instance of the type
        object newItem = Activator.CreateInstance(listType);
        list.Add(newItem);
        return newItem;
    }
}