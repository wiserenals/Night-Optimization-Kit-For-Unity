using System;
using System.Threading.Tasks;
using DotTrail;
using UnityEngine;

public class CallActivator
{
    protected readonly float activateTime = 5;
    protected float currentCall = 0;

    protected virtual float increaseRate => 1;
    protected virtual bool callCheck => currentCall < activateTime;
    
    
    public CallActivator()
    {
        
    }
    
    protected CallActivator(float activateTime)
    {
        this.activateTime = activateTime;
    }

    public CallActivator(int activateTime)
    {
        this.activateTime = activateTime;
    }

    public void Request<T>(Func<T> mainThreadAction, Action<T> secondaryThreadAction = null)
    {
        IncreaseCurrentCall();
        if (callCheck) return;
        
        currentCall = 0;
        var result = mainThreadAction();
        if(secondaryThreadAction != null) Dot.Trail.After(delegate
        {
            secondaryThreadAction(result);
            return Task.CompletedTask;
        });
    }

    private void IncreaseCurrentCall()
    {
        currentCall += increaseRate;
    }

    public void Request(Action mainThreadAction)
    {
        IncreaseCurrentCall();
        if (callCheck) return;
        
        currentCall = 0;
        mainThreadAction();
    }
    
    public void RequestSec(Action secondaryThreadAction)
    {
        IncreaseCurrentCall();
        if (callCheck) return;
        
        currentCall = 0;
        Dot.Trail.After(delegate
        {
            secondaryThreadAction();
            return Task.CompletedTask;
        });
    }
}