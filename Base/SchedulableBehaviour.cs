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
    private List<List<MethodObject>> orderedMethods = new List<List<MethodObject>>();
    private List<List<MethodObject>> orderedFixedMethods = new List<List<MethodObject>>();
    private readonly bool hasOrderedMethods;
    private readonly bool hasOrderedFixedMethods;
    private int currentFrame = -1;
    private int currentFixedFrame = -1;
    private IEnumerator updateEnumerator;
    private IEnumerator fixedUpdateEnumerator;
    private int enumeratorFrameWaitTime;
    private int fixedEnumeratorFrameWaitTime;
    private readonly bool isEnumerableDiscreteUpdateUsing;
    private readonly bool isEnumerableDiscreteFixedUpdateUsing;

    public SchedulableBehaviour()
    {
        #region OrderLocalDiscreteUpdates
        
        var methods = GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        
        orderedMethods.Add(methods.ToList().ConvertToMethodObjects<DiscreteUpdateAttribute>(this));
        orderedMethods = orderedMethods.CorrectAllMO();
        
        hasOrderedMethods = orderedMethods.Count > 0;
        
        orderedFixedMethods.Add(methods.ToList().ConvertToMethodObjects<DiscreteFixedUpdateAttribute>(this));
        orderedFixedMethods = orderedFixedMethods.CorrectAllMO();
        
        hasOrderedFixedMethods = orderedFixedMethods.Count > 0;
        
        #endregion

        isEnumerableDiscreteUpdateUsing = this.HasOverride(nameof(EnumerableDiscreteUpdate));
        isEnumerableDiscreteFixedUpdateUsing = this.HasOverride(nameof(EnumerableDiscreteFixedUpdate));
    }

    protected virtual void Awake()
    {
        #region OrderGlobalDiscreteUpdates
        
        var globalMethods = GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        
        GlobalDiscreteUpdateManager.Instance.Add(
            globalMethods.ToList().ConvertToMethodObjects<GlobalDiscreteUpdateAttribute>(this));

        #endregion
        
        updateEnumerator = EnumerableDiscreteUpdate();
        fixedUpdateEnumerator = EnumerableDiscreteFixedUpdate();
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

        if (!hasOrderedMethods) return;
        
        orderedMethods[currentFrame = (currentFrame + 1) % orderedMethods.Count].CallAll();
    }

    protected virtual void FixedUpdate()
    {
        if (isEnumerableDiscreteFixedUpdateUsing && --fixedEnumeratorFrameWaitTime <= 0)
        {
            if(!fixedUpdateEnumerator.MoveNext()) (fixedUpdateEnumerator = EnumerableDiscreteFixedUpdate()).MoveNext();
            if (fixedUpdateEnumerator.Current is int waitFrames)
            {
                fixedEnumeratorFrameWaitTime = waitFrames;
            }
        }
        
        if (!hasOrderedFixedMethods) return;
        orderedFixedMethods[currentFixedFrame = (currentFixedFrame + 1) % orderedFixedMethods.Count].CallAll();
    }

    protected virtual IEnumerator EnumerableDiscreteUpdate()
    {
        yield return 9999;
    }
    
    protected virtual IEnumerator EnumerableDiscreteFixedUpdate()
    {
        yield return 9999;
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal class DiscreteUpdateAttribute : UpdateBaseAttribute
{
    public DiscreteUpdateAttribute(int order) : base(order)
    {
        
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal class DiscreteFixedUpdateAttribute : UpdateBaseAttribute
{
    public DiscreteFixedUpdateAttribute(int order) : base(order)
    {
        
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal class GlobalDiscreteFixedUpdateAttribute : UpdateBaseAttribute
{
    public GlobalDiscreteFixedUpdateAttribute(int order) : base(order)
    {

    }
}

[AttributeUsage(AttributeTargets.Method)]
internal class GlobalDiscreteUpdateAttribute : UpdateBaseAttribute
{
    public GlobalDiscreteUpdateAttribute(int order) : base(order)
    {

    }
}

public class UpdateBaseAttribute : Attribute
{
    public readonly int order;
    public UpdateBaseAttribute(int order)
    {
        this.order = order;
    }
}
