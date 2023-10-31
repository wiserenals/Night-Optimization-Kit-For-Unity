using System.Collections.Generic;
using System.Linq;
using DotTrail;
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
    private List<Vector2> lastAgents = new List<Vector2>();
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
        BridgeJob();
    }

    /// <summary>
    /// WARNING: This is a job execution. Do not edit if you don't know about parallel programming.
    /// </summary>
    private void BridgeJob()
    {
        var closestAgents = new List<Vector2>();
        Vector3 pos = cameraTransform.position;
        Dot.Trail.Loop("SecondaryThread", async deltaTime =>
        {
            closestAgents = FindClosestPoints(new Vector2(pos.x, pos.z), agents);
        }, () =>
        {
            pos = cameraTransform.position;
            closestAgents = cullingCamera.CullPositions(closestAgents);
            for (int i = 0; i < closestAgents.Count; i++)
            {
                var agent = closestAgents[i];
                agentComponents[i].SetNativePosition(new Vector3(agent.x, 0, agent.y));
            }
        });
    }

    private void SpawnPool()
    {
        agentComponents = cullingCamera
            .cullingPool
            .Instantiate(agentPrefab, _limit, true)
            .ConvertAll(x => x.GetComponent<IOptimizedAgentComponent>())
            .ToArray();
    }

    private List<Vector2> FindClosestPoints(Vector2 target, IEnumerable<Vector2> points)
    {
        return points.OrderBy(point => Vector2.Distance(target, point)).Take(500).ToList();
    }
}


public interface IOptimizedAgentComponent
{
    public Vector3 nativePosition { get; set; }
    public void SetNativePosition(Vector3 position);
}