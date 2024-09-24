using System.Collections.Generic;
using System.Linq;

public class GlobalDiscreteUpdateManager : ForceSingletonDontDestroy<GlobalDiscreteUpdateManager>
{
    private List<List<MethodObject>> globalMethods = new List<List<MethodObject>>();

    private int currentFrame = -1;
    private bool isUsing;
    private bool isRunning = true;
    
    private void Update()
    {
        if (!isUsing) return;
        if (!isRunning) return;
        globalMethods[currentFrame = (currentFrame + 1) % globalMethods.Count].CallAll();
    }

    public void Add(List<MethodObject> list)
    {
        if (list.Count > 0)
        {
            isUsing = true;
            globalMethods.Add(list);
            globalMethods = globalMethods
                .SelectMany(subList => subList)
                .GroupBy(method => method.customOrder)
                .OrderBy(group => group.Key)
                .Select(group => group.ToList())
                .ToList();
        }
    }
    
    public void Stop()
    {
        isRunning = false;
    }
    
    public void Resume()
    {
        isRunning = true;
    }

    public void DestroyAllUpdatesForever()
    {
        globalMethods.Clear();
        isUsing = false;
    }
}