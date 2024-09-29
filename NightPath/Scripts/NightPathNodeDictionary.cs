using System;
using UnityEngine;

public class NightPathNodeDictionary
{
    public NightPathNode[,] List { get; }

    public int sizeX { get; private set; }
    public int sizeZ { get; private set; }
    public int Size => sizeX * sizeZ;

    public NightPathNodeDictionary(int x, int z)
    {
        sizeX = x;
        sizeZ = z;
        List = new NightPathNode[x, z];
    }
    
    public NightPathNode this[int x, int z]
    {
        get
        {
            if (x >= 0 && x < sizeX && z >= 0 && z < sizeZ)
                return List[x, z];
            return null;
        }
        set
        {
            if (x >= 0 && x < sizeX && z >= 0 && z < sizeZ)
                List[x, z] = value;
        }
    }
    
    public NightPathNode GetNearestByWorldPosition(Vector3 position)
    {
        NightPathNode nearestNode = null;
        float nearestDistance = float.MaxValue;

        ForAll((node, index) =>
        {
            if (node != null)
            {
                float distance = Vector3.Distance(position, node.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestNode = node;
                }
            }
        });

        return nearestNode;
    }

    public void ForAll(Action<NightPathNode, Vector2Int> action)
    {
        if (List == null) return; 
        for (int xx = 0; xx < sizeX; xx++)
        {
            for (int zz = 0; zz < sizeZ; zz++)
            {
                var node = List[xx, zz];
                if(node != null) action(node, new Vector2Int(xx, zz));
            }
        }
    }
    
    public NightPathNodeDictionary Clone()
    {
        NightPathNodeDictionary clone = new NightPathNodeDictionary(sizeX, sizeZ);

        for (int xx = 0; xx < sizeX; xx++)
        {
            for (int zz = 0; zz < sizeZ; zz++)
            {
                if (List[xx, zz] != null)
                {
                    clone[xx, zz] = List[xx, zz].Clone(clone);
                }
            }
        }
        
        return clone;
    }
}