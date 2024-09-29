using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotTrail;
using UnityEngine;

[ExecuteAlways]
public class NightPath : MonoBehaviour
{
    public NightPathOptions options;
    
    public NodeCalculator nodeCalculator;
    public List<ConditionalObject<NightPathSceneTimePostProcessorBase, NightPathPostProcessorBase>> staticPostProcessors;
    public List<ConditionalObject<NightPathSceneTimePostProcessorBase, NightPathPostProcessorBase>> runtimePostProcessors;

    private NightPathNodeDictionary staticNodesProcessing;
    private NightPathNodeDictionary lateNodesProcessing;

    public NightPathNodeDictionary nodes;
    
    private CallActivator calculateNodesCallActivator;
    
    private async void OnValidate()
    {
        var activateTime = Application.isPlaying
            ? options.calculateNodesActivateTime
            : Mathf.FloorToInt(options.calculateNodesActivateTime / 10f);
        calculateNodesCallActivator = new CallActivator(activateTime);
        await Build();
    }

    private async void OnEnable()
    {
        await Build();
        StartCoroutine(Loop());
    }

    private void Start()
    {
        calculateNodesCallActivator = new CallActivator(options.calculateNodesActivateTime);
    }

    public async Task Build()
    {
        CalculateNodes();
        await ApplyPostProcessing(staticPostProcessors, staticNodesProcessing);
        lateNodesProcessing = staticNodesProcessing.Clone();
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            var isCalled = calculateNodesCallActivator.Request(Build);
            yield return new WaitUntil(() => isCalled.IsCompleted);
            if (!isCalled.Result) lateNodesProcessing = staticNodesProcessing.Clone();
            var task = ApplyPostProcessing(runtimePostProcessors, lateNodesProcessing);
            yield return new WaitUntil(() => task.IsCompleted);
            nodes = lateNodesProcessing.Clone();
            yield return null;
        }
    }

    private void CalculateNodes()
    {
        staticNodesProcessing = nodeCalculator.Calculate(transform.position);
    }

    private async Task ApplyPostProcessing(List<ConditionalObject<NightPathSceneTimePostProcessorBase, 
        NightPathPostProcessorBase>> pps, NightPathNodeDictionary toDictionary)
    {
        foreach (var postProcessor in pps) 
        {
            var getPostProcessor = postProcessor.GetSelectedObject();

            switch (getPostProcessor)
            {
                case NightPathCompletePostProcessor nightPathCompletePostProcessor:
                    if (nightPathCompletePostProcessor.secondaryThread)
                    {
                        await Dot.DummyTask(() => nightPathCompletePostProcessor.OnNodes(toDictionary));
                    }
                    else nightPathCompletePostProcessor.OnNodes(toDictionary);
                    break;
                case NightPathPostProcessor nightPathPostProcessor:
                    if (nightPathPostProcessor.secondaryThread)
                    {
                        await Dot.DummyTask(() =>
                        {
                            toDictionary.ForAll((node, _) =>
                            {
                                nightPathPostProcessor.OnNode(node);
                            });
                        });
                    }
                    else toDictionary.ForAll((node, _) =>
                    {
                        nightPathPostProcessor.OnNode(node);
                    });
                    break;
                case NightPathSceneTimePostProcessor sceneTimePostProcessor:
                    sceneTimePostProcessor.OnNodes(toDictionary);
                    await Dot.DummyTask(sceneTimePostProcessor.SecondaryThread);
                    break;
            }
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        var standardColor = options.gizmosColor;
        var gizmosSphereRadius = options.gizmosNodeRadius;
        nodes.ForAll((node, _) =>
        {
            var blendAmount = 1 - node.weight;
            Gizmos.color = node.flag 
                ? Color.red
                : GizmosHelpers.InvertColor(standardColor, blendAmount);
            Gizmos.DrawSphere(node.position, gizmosSphereRadius);
        });
        
        nodeCalculator.OnGizmos();
    }
    #endif
}