using System;
using System.Threading.Tasks;
using DotTrail;

public class CallActivator
{
    private readonly int activateTime = 5;
    private int currentCall = 0;
    
    public CallActivator()
    {
        
    }

    public CallActivator(int activateTime)
    {
        this.activateTime = activateTime;
    }

    public void Request<T>(Func<T> mainThreadAction, Action<T> secondaryThreadAction = null)
    {
        if (++currentCall % activateTime != 0) return;
        
        currentCall = 0;
        var result = mainThreadAction();
        if(secondaryThreadAction != null) Dot.Trail.After(delegate
        {
            secondaryThreadAction(result);
            return Task.CompletedTask;
        });
    }
    
    public void Request(Action mainThreadAction)
    {
        if (++currentCall % activateTime != 0) return;
        
        currentCall = 0;
        mainThreadAction();
    }
    
    public void RequestSec(Action secondaryThreadAction)
    {
        if (++currentCall % activateTime != 0) return;
        
        currentCall = 0;
        Dot.Trail.After(delegate
        {
            secondaryThreadAction();
            return Task.CompletedTask;
        });
    }
}