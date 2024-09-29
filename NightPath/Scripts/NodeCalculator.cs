using UnityEngine;

public abstract class NodeCalculator : ScriptableObject
{
    public abstract NightPathNodeDictionary Calculate(Vector3 center);

    public abstract void OnGizmos();
}