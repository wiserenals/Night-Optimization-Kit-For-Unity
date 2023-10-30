using System;
using UnityEngine;

[ExecuteAlways]
public class Cullable : ExpandedBehaviour
{
    [Range(0, 0.5f)] public float raycastPointSize = 0.15f;
    public RaycastPoint[] raycastPoints = Array.Empty<RaycastPoint>();

    private Transform _transform;

    public Vector3 Position => _transform == null ? (_transform = transform).position : _transform.position;

    private void Start()
    {
        _transform = transform;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!_transform) _transform = transform;
        var position = _transform.position;
        for (int i = 0; i < raycastPoints.Length; i++)
        {
            var point = raycastPoints[i];
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position + point.position, raycastPointSize * Vector3.one);
        }
    }
    
    #endif
}

[Serializable]
public class RaycastPoint
{
    public Vector3 position;
}
