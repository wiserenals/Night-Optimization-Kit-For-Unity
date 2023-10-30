using System;
using System.Collections.Generic;
using System.Linq;
using Karayel;
using UnityEngine;

[ExecuteAlways]
public class NightCullCamera : Singleton<NightCullCamera>
{
    public CullingProperties cullingProperties;
    public CullingArea cullingArea;

    public Nightpool cullingPool;

    private Transform _transform;

    private Vector3 CamPosition => _transform.position + transform.forward * cullingArea.distance;

    private void Start()
    {
        _transform = transform;
        CreateCullingLoop();
    }

    private void CreateCullingLoop()
    {
        var interval = cullingProperties.loopInterval;
        var loop = new Loop(interval);
        loop.Start(CullingLoop);
    }

    private void CullingLoop()
    {
        var result = new List<Cullable>();
        
        var camPosition = CamPosition;
        var center = CalculateCenterOfCullingArea();
        var radius = cullingArea.radius;
        
        var all = cullingPool.cullables;
        
        if (!cullingProperties.areaCulling)
        {
            result = all;
            goto Enabling;
        }
        
        foreach (var cullable in all)
        {
            if(!cullable) continue;
            cullable.gameObject.SetActive(false);
        }
        
        var available = all
            .FindAll(x => Vector3.Distance(center, x.Position) < radius);

        if (!cullingProperties.distanceCulling)
        {
            result = available;
            goto Enabling;
        }
        
        var ordered = available
            .OrderBy(x => Vector3.Distance(camPosition, x.Position)).ToList();

        if (cullingProperties.limitCulling)
            ordered = ordered.Take(cullingProperties.maxActiveLimit).ToList();

        if (!cullingProperties.raycastCulling)
        {
            result = ordered;
            goto Enabling;
        }

        var shift = cullingProperties.raycastShift;
        var rayMax = radius * 2;
        RaycastHit hitInfo;
        foreach (var cullable in ordered)
        {
            var pos = cullable.Position;
            var willActive = false;
            foreach (var raycastPoint in cullable.raycastPoints)
            {
                var direction = (pos + raycastPoint.position) - camPosition;
                var isHit = Physics.Raycast(camPosition, 
                    direction, 
                    out hitInfo, 
                    rayMax, 
                    cullingProperties.layers);
                if (!isHit
                    || hitInfo.distance >= direction.magnitude - shift) 
                    willActive = true;
            }
            cullable.gameObject.SetActive(willActive);
        }

        return;
        
        Enabling:
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
   

