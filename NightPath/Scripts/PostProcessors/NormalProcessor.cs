using System;
using UnityEngine;

[CreateAssetMenu(menuName = "NightPath/" + nameof(NormalProcessor))]
public class NormalProcessor : NightPathPostProcessor
{
    [Header("Normal Value Settings")]
    [Tooltip("The angle below which the value is set to 1.")]
    [Range(0f, 90f)]
    public float angleThreshold = 1f;

    [Tooltip("The angle above which the value is set to 0.")]
    [Range(0f, 90f)]
    public float maxAngle = 90f;

    public override void OnNode(NightPathNode node)
    {
        node.weight = NightPathHelpers.GetValueFromNormal(node.normal, angleThreshold, maxAngle);
    }
}