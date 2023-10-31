using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class OptimizedAgentManager : ExpandedBehaviour
{
    public NightPathBuilder nightPathBuilder;
    public float agentSlowness;
    public float moveScale = 2;
    
    [HideInInspector] public List<Vector2> agents;
    [NonSerialized] public bool drawGizmosTrigger = false;
    [HideInInspector] public int selectedIndex = 0;

    private NavigationTile _currentTile;
    private IEnumerator Start()
    {
        var world = nightPathBuilder.navigationWorld;
        while (true)
        {
            yield return new WaitForSeconds(agentSlowness);
            if (world.Current?.tiles == null) continue;
            Parallel.For(0, agents.Count, i =>
            {
                agents[i] += world.GetFlowAt(agents[i]) * moveScale;
            });
        }
    }

    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (NightJobManager.gamePlaying)
            {
                const int times = 333;
                var dv = agents.Count / times;
                for (int i = 0; i < agents.Count; i += dv)
                {
                    var agent = agents[i];
                    var agentV3 = new Vector3(agent.x, 0, agent.y);
                    Gizmos.DrawLine(agentV3, agentV3 + Vector3.up * 1000);
                }

                return;
            }
            if (!debuggingSettings.drawDebugging
                || !agents.Any()
                || !drawGizmosTrigger) return;


            if (agents.Count <= selectedIndex) return;
            
            Gizmos.color = Color.red;
            Vector3 agentPosition = new Vector3(agents[selectedIndex].x, 0, agents[selectedIndex].y);
            Gizmos.DrawCube(agentPosition, new Vector3(1,10000,1));
        }
    #endif

}