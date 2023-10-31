using System;
using System.Collections.Generic;
using System.Linq;
using Karayel;
using UnityEngine;

[ExecuteAlways]
public class NightCullCamera : Singleton<NightCullCamera>
{
    public string collectionTag;
    public CullingProperties cullingProperties;
    public CullingArea cullingArea;

    public Nightpool cullingPool;

    private Transform _transform;

    private Vector3 CamPosition => _transform.position + transform.forward * cullingArea.distance;

    private void Awake()
    {
        cullingPool = new Nightpool();
        _transform = transform;
        if (!string.IsNullOrWhiteSpace(collectionTag))
        {
            var found = GameObject.FindGameObjectsWithTag(collectionTag);
            if(found != null) cullingPool.Add(found);
        }
        CreateCullingLoop();
    }

    private List<T> Cull<T>(List<T> objects, Func<T, Vector3> getPosition)
    {
        List<T> result = new List<T>();
    
        var camPosition = CamPosition;
        var center = CalculateCenterOfCullingArea();
        var radius = cullingArea.radius;
    
        if (!cullingProperties.areaCulling)
        {
            return objects;
        }
    
        var available = objects
            .FindAll(x => Vector3.Distance(center, getPosition(x)) < radius);

        if (!cullingProperties.distanceCulling)
        {
            return available;
        }
    
        var ordered = available
            .OrderBy(x => Vector3.Distance(camPosition, getPosition(x))).ToList();

        if (cullingProperties.raycastCulling)
        {
            var shift = cullingProperties.raycastShift;
            RaycastHit hitInfo;
            foreach (var obj in ordered)
            {
                if (obj is Cullable cullable) MakeRayC(cullable);
                else MakeRay(getPosition(obj));

                void MakeRay(Vector3 pos)
                {
                    var direction = camPosition - pos;
                    var rayMax = direction.magnitude - shift;
                    var isHit = Physics.Raycast(camPosition,
                        direction,
                        out hitInfo,
                        rayMax,
                        cullingProperties.layers);
                    if (!isHit)
                    {
                        result.Add(obj);
                        return;
                    }
                }

                void MakeRayC(Cullable cullableToRay)
                {
                    var points = cullableToRay.raycastPoints;
                    foreach (var point in points)
                    {
                        var direction = point.position - camPosition + cullable.Position;
                        var rayMax = direction.magnitude - shift;
                        bool isHit = Physics.Raycast(camPosition,
                            direction,
                            out hitInfo,
                            rayMax,
                            cullingProperties.layers);
                        if (!isHit)
                        {
                            result.Add(obj);
                            return;
                        }
                    }

                }
            }
        }
        else result = ordered;

        if (cullingProperties.limitCulling)
            result = result.Take(cullingProperties.maxActiveLimit).ToList();

        return result;
    }

    public List<Cullable> CullObjects(List<Cullable> cullables)
    {
        return Cull(cullables, c => c.Position);
    }

    public List<Vector3> CullPositions(List<Vector3> positions)
    {
        return Cull(positions, p => p);
    }
    
    public List<Vector2> CullPositions(List<Vector2> positions)
    {
        return Cull(positions, p => new Vector3(p.x, 0, p.y));
    }

    private void CreateCullingLoop()
    {
        var interval = cullingProperties.loopInterval;
        var loop = new Loop(interval);
        loop.Start(CullingLoop);
    }

    private void CullingLoop()
    {
        var all = cullingPool.cullables;
    
        var result = Cull(all, c => c.Position);
    
        foreach (var cullable in all)
        {
            cullable.gameObject.SetActive(false);
        }
        
        foreach (var cullable in result)
        {
            cullable.gameObject.SetActive(true);
        }
    }

    private Vector3 CalculateCenterOfCullingArea()
    {
        return _transform.position + _transform.forward * (cullingArea.distance + cullingArea.radius);
    }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            PreviewCullingArea();
        }

        private void PreviewCullingArea()
        {
            if (!debuggingSettings.drawDebugging) return;
            _transform = transform;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(CamPosition, 1);
            Gizmos.color = cullingArea.color;
            Gizmos.DrawWireSphere(CalculateCenterOfCullingArea(), cullingArea.radius);
        }
#endif
}
   

