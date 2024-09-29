using UnityEngine;

[CreateAssetMenu(menuName = "NightPath/NodeCalculators/" + nameof(GridNodeCalculator))]
public class GridNodeCalculator : NodeCalculator
{
    public float rayStartHeight = 100;
    public Vector2 size;
    [Range(1, 10)]public float nodeGap = 1;
    public LayerMask targetLayers;

    private Vector3 lastCenter;
    private float lastRaycastHeight;
    
    public override NightPathNodeDictionary Calculate(Vector3 center)
    {
        #region Preprocessing

        lastCenter = center;
        lastRaycastHeight = rayStartHeight;
        
        var minGap = (size.x + size.y) / 200f;
        nodeGap = Mathf.Max(nodeGap, minGap);
        var x = Mathf.FloorToInt(size.x / nodeGap);
        var z = Mathf.FloorToInt(size.y / nodeGap);
        var nodes = new NightPathNodeDictionary(x, z);
         
        var startPosition = center - new Vector3(x * nodeGap, 0, z * nodeGap) / 2f;

        #endregion

        for (int xx = 0; xx < x; xx++)
        {
            for (int zz = 0; zz < z; zz++)
            {
                var rayStartPosition = startPosition 
                                       + new Vector3(xx * nodeGap, rayStartHeight, zz * nodeGap);
                if (Physics.Raycast(rayStartPosition, Vector3.down, 
                        out var hit, rayStartHeight + 10, targetLayers))
                {
                    nodes[xx, zz] = new NightPathNode (nodes)
                    {
                        position = hit.point,
                        normal = hit.normal,
                        bound = hit.transform,
                        xIndex = xx,
                        zIndex = zz
                    };
                }
            }
        }

        return nodes;
    }

    public override void OnGizmos()
    {
        GizmosHelpers.DrawArrowDown(lastCenter + Vector3.up * lastRaycastHeight, lastRaycastHeight, Color.red);
    }
}