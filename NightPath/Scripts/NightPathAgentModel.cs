using UnityEngine;

public abstract class NightPathAgentModel : ScriptableObject
{
    public abstract object Calculate(NightPathNode startNode, NightPathNodeDictionary dictionary);
}