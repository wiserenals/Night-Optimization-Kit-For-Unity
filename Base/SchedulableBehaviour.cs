using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SchedulableBehaviour : MonoBehaviour
{
    private int _frameTimeElapsed;
    protected int FrameTimeElapsed
    {
        get
        {
            var elapsed = _frameTimeElapsed;
            _frameTimeElapsed = 0;
            return elapsed;
        }
    }
    private readonly List<MethodInfo> orderedMethods;
    private readonly bool hasOrderedMethods;
    private int currentFrame = -1;
    private IEnumerator updateEnumerator;
    private int enumeratorFrameWaitTime;
    private readonly bool isEnumerableDiscreteUpdateUsing;

    public SchedulableBehaviour()
    {
        var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        
        // Sort the methods based on their order
        orderedMethods = methods
            .OrderByWithoutNegatives(m => m.GetCustomAttribute<DiscreteUpdateAttribute>()?.order ?? -1).ToList();

        // Set boolean flag to avoid checking the list count each time
        hasOrderedMethods = orderedMethods.Count > 0;
        isEnumerableDiscreteUpdateUsing = this.HasOverride(nameof(EnumerableDiscreteUpdate));
    }

    private void Start()
    {
        updateEnumerator = EnumerableDiscreteUpdate();
    }

    protected virtual void Update()
    {
        _frameTimeElapsed++;
        
        if (isEnumerableDiscreteUpdateUsing && --enumeratorFrameWaitTime <= 0)
        {
            if(!updateEnumerator.MoveNext()) (updateEnumerator = EnumerableDiscreteUpdate()).MoveNext();
            if (updateEnumerator.Current is int waitFrames)
            {
                enumeratorFrameWaitTime = waitFrames;
            }
        }
        // If there are no ordered methods, skip the update logic
        if (!hasOrderedMethods) return;

        // Find the method to execute for this frame
        MethodInfo methodToInvoke = orderedMethods[currentFrame = (currentFrame + 1) % orderedMethods.Count];

        // Invoke the method
        methodToInvoke.Invoke(this, null);
    }

    protected virtual IEnumerator EnumerableDiscreteUpdate()
    {
        yield return 9999;
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal class DiscreteUpdateAttribute : Attribute
{
    public int order;
    public DiscreteUpdateAttribute(int order)
    {
        this.order = order;
    }
}
