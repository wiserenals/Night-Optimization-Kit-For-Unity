using System;
using UnityEngine;

[CreateAssetMenu(menuName = "NightPath/" + nameof(NeighbourHeightProcessor))]
public class NeighbourHeightProcessor : NightPathPostProcessor
{
    [Header("Height Difference Settings")]
    [Tooltip("The factor that determines how sensitive the weight change is to the height difference.")]
    [Range(0.01f, 1)]
    public float heightDifferenceFactor = 0.1f;

    [Tooltip("The minimum height difference required to affect the node weight.")]
    public float minHeightDifference = 0.1f; 

    [Tooltip("The balance between lowering weight for higher or lower nodes. 0 means only the lower node loses weight, 1 means only the higher node loses weight.")]
    [Range(0, 1)]
    public float heightWeightBias = 0.5f; // Yeni parametre

    public override void OnNode(NightPathNode node)
    {
        float currentHeight = node.position.y;
        float totalHeightDifference = 0f;

        var neighbours = node.neighboursNotNull;
        int neighborCount = neighbours.Length;

        if (neighborCount <= 0) return;

        foreach (var neighbour in neighbours)
        {
            float neighbourHeight = neighbour.position.y;
            float heightDifference = Mathf.Abs(currentHeight - neighbourHeight);

            totalHeightDifference += heightDifference;

            // Yükseklik farkına bağlı ağırlık değişimi
            if (heightDifference > minHeightDifference)
            {
                // Yükseklik farkına bağlı olarak iki taraftan birini daha fazla cezalandır
                float weightReduction = heightDifference * heightDifferenceFactor;
                if (currentHeight > neighbourHeight)
                {
                    // Yukarıda olanın ağırlığı heightWeightBias ile azaltılır
                    node.weight -= weightReduction * heightWeightBias;
                    neighbour.weight -= weightReduction * (1 - heightWeightBias);
                }
                else
                {
                    // Aşağıda olanın ağırlığı heightWeightBias ile azaltılır
                    node.weight -= weightReduction * (1 - heightWeightBias);
                    neighbour.weight -= weightReduction * heightWeightBias;
                }

                // Ağırlığı belirlenen sınırlar içinde tut
                node.weight = Mathf.Clamp(node.weight, 0, 1);
                neighbour.weight = Mathf.Clamp(neighbour.weight, 0, 1);
            }
        }
    }
}
