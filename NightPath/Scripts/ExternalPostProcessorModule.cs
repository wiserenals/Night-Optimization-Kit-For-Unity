using System;

[Serializable]
public class ExternalPostProcessorModule
{
    public NightPath nightPath;
    public NightPathNodeDictionary Execute(NightPathSceneTimePostProcessor postProcessor)
    {
        var cloneNodes = nightPath.nodes.Clone();
        postProcessor.OnNodes(cloneNodes);
        postProcessor.SecondaryThread();
        return cloneNodes;
    }
}