using UnityEngine;

public class FixedCallActivator : CallActivator
{
    protected override float increaseRate => Time.deltaTime;

    protected override bool callCheck => currentCall * activateTime < 1;

    public FixedCallActivator()
    {
        
    }

    public FixedCallActivator(float activateTimePerSecond) : base(activateTimePerSecond)
    {
        
    }

}