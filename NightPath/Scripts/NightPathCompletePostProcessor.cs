
using UnityEngine;

public abstract class NightPathCompletePostProcessor : NightPathPostProcessorBase
{
    public abstract void OnNodes(NightPathNodeDictionary nodes);
}