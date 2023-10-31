using DotTrail;
using UnityEngine;

public class NOK_NightPathBridgeAgent : MonoBehaviour, IOptimizedAgentComponent
{
    public float smoothTime;
    private Transform _transform;
    private bool isActive;
    public Vector3 nativePosition { get; set; }
    
    public void SetNativePosition(Vector3 position)
    {
        nativePosition = position;
        if (isActive) return;
        isActive = true;
        gameObject.SetActive(true);
        _transform.position = position;
    }

    private void Awake()
    {
        _transform = transform;
        MoveJob();
    }

    /// <summary>
    /// WARNING: This is a job execution. Do not edit if you don't know about parallel programming.
    /// </summary>
    private void MoveJob()
    {
        Vector3 position = _transform.position;
        Quaternion rotation = _transform.rotation;
        Dot.Trail.Loop("BridgeAgentThread", deltaTime =>
        {
            position = Vector3.Lerp(position, nativePosition, deltaTime * smoothTime);
            rotation = Quaternion.Lerp(rotation, 
                Quaternion.LookRotation(nativePosition - position, Vector3.up), deltaTime);
            if (Vector3.Distance(position, nativePosition) > 2.5f) position = nativePosition;
        }, () =>
        {
            _transform.position = position;
            _transform.rotation = rotation;
        });
    }

}
