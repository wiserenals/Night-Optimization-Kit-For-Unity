using UnityEngine;

[CreateAssetMenu(menuName = "NightPath/AgentModels/" + nameof(FlowVectorModel))]
public class FlowVectorModel : NightPathAgentModel
{
    public override object Calculate(NightPathNode startNode, NightPathNodeDictionary dictionary)
    {
        var flowVector = Vector3.zero;
        foreach (var nodeNeighbour in startNode.neighbours)
        {
            var direction = nodeNeighbour.Vector3Index - startNode.Vector3Index;
            flowVector -= (1 - nodeNeighbour.weight) * (1 - nodeNeighbour.weight) * direction;
        }

        return flowVector.normalized;
    }
}