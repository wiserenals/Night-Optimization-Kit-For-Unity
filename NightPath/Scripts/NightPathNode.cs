using System.Linq;
using UnityEngine;

public class NightPathNode
{
    public NightPathNodeDictionary nodes { get; }
    #region Clone

    public Vector3 position;
    public Vector3 normal;
    public float weight = 1;
    public Transform bound;
    public int xIndex, zIndex;

    #endregion
    
    public Vector3 Vector3Index => new Vector3(xIndex, 0, zIndex);
    
    public bool isUsed; // Postprocessing time value
    public bool flag; // Visualize on gizmos


    public NightPathNode(NightPathNodeDictionary dictionary)
    {
        nodes = dictionary;
    }
    
    public NightPathNode this[int x, int z] => nodes[xIndex + x, zIndex + z];

    public NightPathNode[] neighbours => new[] { 
        this[1, -1],
        this[1, 0], 
        this[1, 1],
        this[0, -1],
        this[0, 1], 
        this[-1, -1], 
        this[-1, 1], 
        this[-1, 0] };
    
    public NightPathNode[] neighboursNotNull => neighbours.Where(x => x != null).ToArray();

    public NightPathNode Clone(NightPathNodeDictionary dictionary)
    {
        return new NightPathNode(dictionary)
        {
            position = position,
            normal = normal,
            bound = bound,
            weight = weight,
            xIndex = xIndex,
            zIndex = zIndex
        };
    }

    
}