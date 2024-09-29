using System;
using UnityEngine;

[CreateAssetMenu(menuName = "NightPath/" + nameof(MapEdgeProcessor))]
public class MapEdgeProcessor : NightPathPostProcessor
{
    public override void OnNode(NightPathNode node)
    {
        if (node.neighboursNotNull.Length < node.neighbours.Length) node.weight = 0;
    }
}