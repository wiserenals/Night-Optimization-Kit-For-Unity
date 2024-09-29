using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NightPath/" + nameof(NegativeFlowProcessor))]
public class NegativeFlowProcessor : NightPathCompletePostProcessor
{
    [Min(0)] public float effectRadius = 5f; // Etki alanı yarıçapı
    [Min(0)] public float effectIntensity = 0.5f; // Etki yoğunluğu
    [Range(0,1)] public float falloff = 0.1f; // Etkinin mesafeye göre düşüş oranı
    [Min(0)]public float maxBaseNodeWeight = 0.3f;

    public override void OnNodes(NightPathNodeDictionary nodes)
    {
        // Ağırlığı düşük nodeları bul ve etki yayılımı uygula
        List<Vector2Int> lowWeightNodes = new List<Vector2Int>();

        nodes.ForAll((node, index) =>
        {
            if (node.weight < maxBaseNodeWeight) // Düşük ağırlıklı nodeları belirliyoruz
            {
                lowWeightNodes.Add(index);
            }
        });

        // Ağırlığı düşük nodeların çevresine etki uygula
        foreach (var nodeIndex in lowWeightNodes)
        {
            ApplySmoothWeightReduction(nodes, nodeIndex);
        }
    }

    private void ApplySmoothWeightReduction(NightPathNodeDictionary nodes, Vector2Int sourceIndex)
    {
        NightPathNode sourceNode = nodes[sourceIndex.x, sourceIndex.y];
        if (sourceNode == null) return;

        // Etki alanını belirle
        int maxRadius = Mathf.CeilToInt(effectRadius);

        for (int dx = -maxRadius; dx <= maxRadius; dx++)
        {
            for (int dz = -maxRadius; dz <= maxRadius; dz++)
            {
                Vector2Int currentIndex = new Vector2Int(sourceIndex.x + dx, sourceIndex.y + dz);
                NightPathNode currentNode = nodes[currentIndex.x, currentIndex.y];

                if (currentNode != null && currentNode != sourceNode)
                {
                    // Mesafeyi hesapla
                    float distance = Vector2.Distance(sourceIndex, currentIndex);

                    // Etki alanı içinde olup olmadığını kontrol et
                    if (distance <= effectRadius)
                    {
                        // Gaussian smoothing benzeri bir etki
                        float distanceFactor = Mathf.Exp(-(distance * distance) / (2 * (effectRadius * effectRadius)));
                        
                        // Mesafeye bağlı ağırlık azaltımı
                        float weightReduction = effectIntensity * distanceFactor * falloff;
                        
                        // Ağırlığı azalt
                        currentNode.weight -= weightReduction;

                        // Ağırlığı 0 ile 1 arasında sınırla
                        currentNode.weight = Mathf.Clamp(currentNode.weight, 0f, 1f);
                    }
                }
            }
        }
    }
}
