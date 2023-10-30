using UnityEngine;
using UnityEngine.Serialization;

public abstract class LegacyAgentBase : ExpandedBehaviour
{
    [FormerlySerializedAs("nightpathBuilder")] public NightPathBuilder nightPathBuilder;
    private NavigationTile _currentTile;
    private NavigationTile _lastValidTile;
    protected void GetFlow()
    {
        OnFlow(
            nightPathBuilder
            .navigationWorld
            .GetFlowAt(transform.position)
            );
    }

    protected abstract void OnFlow(Vector2 flowVector);
}