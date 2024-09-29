using System.Collections.Generic;
using UnityEngine;


/*
 Quadrant Growth and Intersection Search (QGIS) is a pathfinding algorithm designed to 
 identify the nodes an agent can access or "see" within its environment. 
 Starting from a central node, the algorithm expands outward in four quadrants (up, down, left, right), 
 as well as diagonally, forming a growing square. The expansion continues as long as 
 the neighboring nodes meet a minimum weight threshold, ensuring that only valid paths are explored. 
 By intersecting the square growth with diagonal searches, the algorithm determines the full set of 
 accessible nodes. This method is particularly useful for agents to detect reachable areas dynamically 
 and optimize movement through complex environments.
 Created by Yusuf Karayel.
 */
[CreateAssetMenu(menuName = "NightPath/AgentModels/" + nameof(QGISModel))]
public class QGISModel : NightPathAgentModel
{
    public float minWeightToContinue = 0.2f;
    public int maxGrowth = 10;
    
    public override object Calculate(NightPathNode startNode, NightPathNodeDictionary dictionary)
    {
        if (startNode == null) return null;
        var squareNodes = new List<NightPathNode>();
        squareNodes.AddRange(SubGrowth(startNode, 1, 1));
        squareNodes.AddRange(SubGrowth(startNode, 1, -1));
        squareNodes.AddRange(SubGrowth(startNode, -1, 1));
        squareNodes.AddRange(SubGrowth(startNode, -1, -1));

        // Çapraz genişleme
        var crossNodes = new List<NightPathNode>();
        crossNodes.AddRange(CrossGrowth(startNode, 1, 1));
        crossNodes.AddRange(CrossGrowth(startNode, 1, -1));
        crossNodes.AddRange(CrossGrowth(startNode, -1, 1));
        crossNodes.AddRange(CrossGrowth(startNode, -1, -1));

        // Ortak noktaları bulma
        var commonNodes = new List<NightPathNode>();
        foreach (var node in squareNodes)
        {
            if (crossNodes.Contains(node))
            {
                commonNodes.Add(node);
            }
        }

        return commonNodes.Heaviest();
    }

    private List<NightPathNode> SubGrowth(NightPathNode edgeNode, int x, int z)
    {
        var allNodes = new List<NightPathNode> { edgeNode };
        var xSides = new List<NightPathNode> { edgeNode };
        var zSides = new List<NightPathNode> { edgeNode };

        for (int i = 0; i < maxGrowth; i++)
        {
            var nextEdge = edgeNode[x, z];

            var newXSides = new List<NightPathNode>();
            var newZSides = new List<NightPathNode>();

            if (nextEdge != null && edgeNode.weight > minWeightToContinue)
            {
                edgeNode = nextEdge;
                newXSides.Add(nextEdge);
                newZSides.Add(nextEdge);
            }
            
            foreach (var side in xSides)
            {
                var next = side[x, 0];
                if (next == null) continue;
                if (next.weight > minWeightToContinue) newXSides.Add(next);
            }
            
            foreach (var side in zSides)
            {
                var next = side[0, z];
                if (next == null) continue;
                if (next.weight > minWeightToContinue) newZSides.Add(next);
            }
            
            allNodes.AddRange(newXSides);
            allNodes.AddRange(newZSides);
            
            xSides = newXSides;
            zSides = newZSides;
        }

        return allNodes;
    }

    private List<NightPathNode> CrossGrowth(NightPathNode edgeNode, int x, int z)
    {
        var allNodes = new List<NightPathNode> { edgeNode };

        var nextSideNode = edgeNode;
        for (int i = 0; i < maxGrowth; i++)
        {
            for (int j = 1; j <= (maxGrowth - i) / 2; j++)
            {
                var crossNode = nextSideNode[x * j, z * j];
                if (crossNode == null || crossNode.weight < minWeightToContinue) break;
                allNodes.Add(crossNode);
            }

            nextSideNode = nextSideNode[0, z];
            if (nextSideNode == null || nextSideNode.weight < minWeightToContinue) break;
            allNodes.Add(nextSideNode);
        }

        nextSideNode = edgeNode[x, 0];
        if (nextSideNode == null) return allNodes;
        for (int i = 0; i < maxGrowth; i++)
        {
            for (int j = 1; j <= (maxGrowth - i) / 2; j++)
            {
                var crossNode = nextSideNode[x * j, z * j];
                if (crossNode == null || crossNode.weight < minWeightToContinue) break;
                allNodes.Add(crossNode);
            }
            
            nextSideNode = nextSideNode[x, 0];
            if (nextSideNode == null || nextSideNode.weight < minWeightToContinue) break;
            allNodes.Add(nextSideNode);
        }

        return allNodes;
    }

}
