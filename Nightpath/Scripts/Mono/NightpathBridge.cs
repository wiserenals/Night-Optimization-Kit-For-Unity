using System.Collections.Generic;
using System.Linq;
using Karayel;
using UnityEngine;

public class NightpathBridge : MonoBehaviour
{
    public OptimizedAgentManager optimizedAgentManager;
    public NightCullCamera cullingCamera;
    public GameObject agentPrefab;
    public bool triggerOnStart = true;
    public int bridgeLoopDelay = 100;

    private IOptimizedAgentComponent[] agentComponents;
    private Transform cameraTransform;
    private Vector3[] agentPositions;
    private bool _bridgeStarted;
    private int _limit;
    private List<Vector2> agents;
    private List<Vector2> lastAgents;
    private bool BridgeNotAvailable => _bridgeStarted || cullingCamera == null || agentPrefab == null;
    private void Start()
    {
        if(triggerOnStart) StartBridge();
    }

    public void StartBridge()
    {
        if (BridgeNotAvailable) return;
        _bridgeStarted = true;
        var properties = cullingCamera.cullingProperties;
        _limit = properties.maxActiveLimit;
        agents = optimizedAgentManager.agents;
        cameraTransform = cullingCamera.transform;
        SpawnPool();
        new Loop(() => bridgeLoopDelay).Start(BridgeLoop);
    }

    private void SpawnPool()
    {
        agentComponents = cullingCamera
            .cullingPool
            .Instantiate(agentPrefab, _limit)
            .ConvertAll(x => x.GetComponent<IOptimizedAgentComponent>())
            .ToArray();
    }
    
    private void BridgeLoop()
    {
        var pos = cameraTransform.position;
        var closestAgents = FindClosestPoints(new Vector2(pos.x, pos.z), agents, _limit);
        foreach (var exc in closestAgents.Except(lastAgents))
        {
            
        }
        for (int i = 0; i < _limit; i++)
        {
            var agent = closestAgents[i];
            agentComponents[i].SetNativePosition(new Vector3(agent.x, 0, agent.y));
        }
    }

    private List<Vector2> FindClosestPoints(Vector2 target, IEnumerable<Vector2> points, int numPoints)
    {
        return points.OrderBy(point => Vector2.Distance(target, point)).Take(numPoints).ToList();
    }
}


public interface IOptimizedAgentComponent
{
    public Vector3 nativePosition { get; set; }
    public void SetNativePosition(Vector3 position);
}