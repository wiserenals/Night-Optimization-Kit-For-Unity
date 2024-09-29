using System.Threading.Tasks;
using UnityEngine;

public abstract class NightPathSceneTimePostProcessor : NightPathSceneTimePostProcessorBase
{
    public abstract void OnNodes(NightPathNodeDictionary nodes);

    public abstract void SecondaryThread();
}

public abstract class NightPathSceneTimePostProcessorBase : MonoBehaviour
{
    
}