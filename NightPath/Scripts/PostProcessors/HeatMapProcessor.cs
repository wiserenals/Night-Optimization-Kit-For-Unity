using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HeatMapProcessor : NightPathSceneTimePostProcessor
{
    public Transform target;
    private float weightDecreaseAmount; 
    public float weightDecreaseRate = 0.002f;
    public float minWeightToContinue = 0.2f;

    private NightPathNode startNode;

    public override void OnNodes(NightPathNodeDictionary nodes)
    {
        weightDecreaseAmount = 0;
        startNode = nodes.GetNearestByWorldPosition(target.position);
    }

    public override void SecondaryThread()
    {
        if (startNode == null) return;

        var nodes = new List<NightPathNode> { startNode };
        while (true)
        {
            var found = new List<NightPathNode>();
            weightDecreaseAmount += weightDecreaseRate;

            foreach (var node in nodes)
            {
                node.isUsed = true;
                foreach (var neighbour in node.neighboursNotNull)
                {
                    if (neighbour.isUsed) continue;
                    if(neighbour.weight < minWeightToContinue) continue;
                    
                    neighbour.weight -= weightDecreaseAmount;
                    neighbour.isUsed = true;
                    found.Add(neighbour);
                }
            }

            if (found.Count > 0)
            {
                nodes = found;
                continue;
            }

            break;
        }
    }
}
