using System;
using UnityEngine;

[Serializable]
public class CullingArea
{
    public float distance;
    [Min(0)] public float radius = 5;
    public Color color = Color.blue;
}