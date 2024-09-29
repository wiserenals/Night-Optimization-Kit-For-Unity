using System;
using System.Threading.Tasks;

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

    private void IncreaseCurrentCall()
    {
        currentCall += increaseRate;
    }

    public bool Request(Action mainThreadAction)
    {
        IncreaseCurrentCall();
        if (callCheck) return false;
        
        currentCall = 0;
        mainThreadAction();
        return true;
    }
    
    public async Task<bool> Request(Func<Task> taskAction)
    {
        IncreaseCurrentCall();
        if (callCheck) return false;
        
        currentCall = 0;
        await taskAction();
        return true;
    }
}