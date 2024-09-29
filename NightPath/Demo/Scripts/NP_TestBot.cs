using System;
using UnityEngine;

public class NP_TestBot : MonoBehaviour
{
    public float speed = 10;
    public float maxDistanceToNextNode = 1;
    public float stopCalculateDistance = 2;
    public NightPath path;
    public HeatMapProcessor heatMap;
    public NightPathAgentModel agentModel;

    private Vector3 nextPosition;

    private void Start()
    {
        nextPosition = transform.position;
    }
    public Animator animator;
    public float rotationLerpSpeed = 10;
    private static readonly int Speed = Animator.StringToHash("Speed");

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
        
        
        animator.SetFloat(Speed, 1);

        var dir = nextPosition - transform.position;
        if(dir != Vector3.zero) transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.LookRotation(dir), 
            Time.deltaTime * rotationLerpSpeed);
    }

    private Vector3 GetNextNodePosition()
    {
        var nearestNode = path.nodes.GetNearestByWorldPosition(transform.position);
        var foundNode = (NightPathNode) agentModel.Calculate(nearestNode, path.nodes);
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
