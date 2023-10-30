using System;
using JetBrains.Annotations;
using NightJob;
using UnityEngine;

[CreateAssetMenu(fileName = "NavigationWorldData", menuName = "Navigation/Navigation World", order = 0)]
public class NavigationWorld : ScriptableObject
{
    public NightPathBuilder NightPathBuilder { get; set; }
    public NavigationWorldInstance navInstance;
    
    public TimeSpan? tileCreationTime, heatmapCreationTime, flowFieldCreationTime;
    public bool obstacleBaked = true;
    public bool delayedObstacleBuild = true;
    public float obstacleBuildDelay = 0.5f;
    public bool generateLoop = true;
    public float generateDelay = 1;
    public LayerMask wallMask;
    public Vector2 navigationGridSize; 
    public float tileRadius;

    public float maxWeight;

    [NonSerialized] public bool isAvailable;

    public NightJobQueue JobQueue { get; private set; }
    
    [CanBeNull] public NavigationBuildJob Current { get; private set; }


    public void Init(NightPathBuilder nightPathBuilder)
    {
        NightPathBuilder = nightPathBuilder;
    }

    public void StopBuilding()
    {
        JobQueue.Dispose();
    }
    
    public void Build(Vector3 position, Vector3 targetPosition, bool bake)
    {
        JobQueue ??= NightJobManager.NewQueue();

        if (JobQueue.AliveJobCount == 0) JobQueue.AddJobToQueue(Current = new NavigationBuildJob
        {
            delayedObstacleBuild = delayedObstacleBuild,
            obstacleBuildDelay = obstacleBuildDelay,
            navigationWorld = this,
            position = position,
            targetPosition = targetPosition,
            bake = bake
        });
    }

    public Vector2 GetFlowAt(Vector2 position)
    {
        return Current.GetFlowFieldVector(position);
    }
}
