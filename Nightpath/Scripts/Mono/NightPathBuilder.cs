
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif 

public class NightPathBuilder : ExpandedBehaviour
{
    public Transform targetTransform;
    public NavigationWorld navigationWorld;

    private Transform _transform;

    [NonSerialized] public bool drawGizmosTrigger;

    public bool IsBuilding => navigationWorld is { JobQueue: { AliveJobCount: > 0 } };

    private void Start()
    {
        _transform = transform;
        navigationWorld.Init(this);
        if (navigationWorld.obstacleBaked)
        {
            navigationWorld.Build(_transform.position, targetTransform.position, true);
        }
        if(navigationWorld.generateLoop) StartCoroutine(ThreadLoop());
    }

    private IEnumerator ThreadLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(navigationWorld.generateDelay);
            if (IsBuilding) continue;
            
            navigationWorld.Build(_transform.position, targetTransform.position, false);
        }
    }



    #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!debuggingSettings.drawDebugging
                || !drawGizmosTrigger || !navigationWorld) return;
            

            var agreen = Color.green;
            agreen.a = 0.2f;
            Gizmos.color = agreen;
            var worldSize = navigationWorld.navigationGridSize;
            var tileRadius = navigationWorld.tileRadius;
            var tileDiameter = tileRadius * 2;

            int gridSizeX = Mathf.RoundToInt(worldSize.x / tileDiameter);
            int gridSizeY = Mathf.RoundToInt(worldSize.y / tileDiameter);

            Vector3 gridCenter = new Vector3(worldSize.x * 0.5f, 0, worldSize.y * 0.5f);
            
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 tilePosition = new Vector3(x * tileDiameter, 0.3f, y * tileDiameter) - gridCenter + transform.position;
                    Gizmos.DrawWireCube(tilePosition, new Vector3(tileDiameter, 0, tileDiameter));
                }
            }

            var data = navigationWorld.Current;

            if (data?.tiles == null) return;
            
            var nodeSize = data.nodeSize;
            
            var planeSize = new Vector3(nodeSize.x, 0, nodeSize.y);
            
            foreach (NavigationTile tile in data.tiles)
            {
                if(tile.Weight <= 0) continue;
                var color = Color.red;
                color.a = tile.Weight / navigationWorld.maxWeight;
                Gizmos.color = color;
                Gizmos.DrawCube(tile.WorldPosition + Vector3.up * 0.2f, planeSize);
            }
        }
        
    #endif
}