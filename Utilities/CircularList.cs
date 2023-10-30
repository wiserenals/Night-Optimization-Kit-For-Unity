using System;
using System.Collections.Generic;

[Serializable]
public class CircularList<T>
{
    public List<T> items;
    private int currentIndex = -1;

    public CircularList(IEnumerable<T> initialItems)
    {
        items = new List<T>(initialItems);
        if (items.Count > 0)
        {
            currentIndex = 0;
        }
    }

    public T Current
    {
        get
        {
            if (currentIndex >= 0 && currentIndex < items.Count)
            {
                return items[currentIndex];
            }
            throw new InvalidOperationException("List is empty or invalid index.");
        }
    }

    public T Next()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("List is empty.");
        }

        currentIndex = (currentIndex + 1) % items.Count;
        return Current;
    }
    
    public T Back()
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("List is empty.");
        }

        if (currentIndex <= 0) currentIndex = items.Count - 1;
        else currentIndex = (currentIndex - 1) % items.Count;
        return Current;
    }
}