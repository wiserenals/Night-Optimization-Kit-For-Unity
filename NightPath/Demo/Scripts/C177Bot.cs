using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C177Bot : MonoBehaviour
{
    public float speed = 10;
    public float maxDistanceToNextNode = 1;
    public float stopCalculateDistance = 2;
    private Vector3 nextPosition;
    
    public NightPathAgentModel agentModel;
    public ExternalPostProcessorModule postProcessorModule;
    public HeatMapProcessor heatMap;

    private NightPathNodeDictionary currentNodes;

    private Vector3 lastTargetPosition; // Optimization trick
    
    private void Start()
    {
        nextPosition = transform.position;
        lastTargetPosition = transform.position;
    }
    
    private void Update()
    {
        var targetPosition = heatMap.target.position;
        targetPosition.y = transform.position.y;
        var targetDistance = Vector3.Distance(targetPosition, transform.position);
        if (targetDistance < stopCalculateDistance) return;
        
        var distance = Vector3.Distance(nextPosition, transform.position);
        if (distance < maxDistanceToNextNode)
        {
            nextPosition = GetNextNodePosition();
        }
        
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * speed);
    }

    private Vector3 GetNextNodePosition()
    {
        if (Vector3.Distance(lastTargetPosition, heatMap.target.position) > 1)
        {
            lastTargetPosition = heatMap.target.position;
            currentNodes = postProcessorModule.Execute(heatMap);
        }
            
        var nearestNode = currentNodes.GetNearestByWorldPosition(transform.position);
        var foundNode = (NightPathNode) agentModel.Calculate(nearestNode, currentNodes);
        return foundNode.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(nextPosition, Vector3.one * 3 + Vector3.up);
    }
#endif
}