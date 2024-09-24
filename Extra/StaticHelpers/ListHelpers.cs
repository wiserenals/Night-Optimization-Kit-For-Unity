using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class ListHelpers
{

    public static void Remove<T>(this List<T> list, Func<T, bool> checkAction)
    {
        
    }
    public static IListPicker CloneListPicker(this IListPicker listPicker)
    {
        if (listPicker == null)
            throw new ArgumentNullException(nameof(listPicker));

        // Get the type of the ListPicker instance
        Type listPickerType = listPicker.GetType();
        IListPicker newListPicker = (IListPicker)Activator.CreateInstance(listPickerType);
        newListPicker.SetList(listPicker.GetList);
        newListPicker.selectedIndex = listPicker.selectedIndex;

        return newListPicker;
    }
    
    public static T CloneListPicker<T>(this IListPicker listPicker) where T : IListPicker
    {
        return (T) CloneListPicker(listPicker);
    }
    
    public static IList CloneShallow(IList originalList)
    {
        if (originalList == null)
            throw new ArgumentNullException(nameof(originalList));
        
        Type listType = originalList.GetType();
        Type itemType = listType.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(itemType);
        IList newList = (IList)Activator.CreateInstance(genericListType);

        foreach (var item in originalList)
        {
            newList.Add(item);
        }

        return newList;
    }
    
    public static IList CloneDeep(IList originalList)
    {
        if (originalList == null)
            throw new ArgumentNullException(nameof(originalList));
        
        Type listType = originalList.GetType();
        Type itemType = listType.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(itemType);
        IList newList = (IList)Activator.CreateInstance(genericListType);

        MethodInfo cloneMethod = itemType.GetMethod("Clone");
        if (cloneMethod == null)
            throw new InvalidOperationException("Items in the list do not implement a Clone method.");

        foreach (var item in originalList)
        {
            var clonedItem = cloneMethod.Invoke(item, null);
            newList.Add(clonedItem);
        }

        return newList;
    }
}