using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CullingProperties
{
    public LayerMask layers;
    public bool areaCulling = true;
    public bool distanceCulling = true;
    public bool limitCulling = true;
    public bool raycastCulling = true;
    [Min(0)] public float raycastShift = 1;
    public int loopInterval = 100;
    [Min(0)] public int maxActiveLimit = 10;
}
