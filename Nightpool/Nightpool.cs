using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Nightpool
{
    public List<Transform> AllTransforms => poolDebug.allTransforms;

    public readonly List<Cullable> cullables = new();
    
    [SerializeField] [ReadOnly] private PoolDebug poolDebug;
    
    public List<GameObject> Instantiate(GameObject prefab, int count, bool disableDirect = false)
    {
        var list = new List<GameObject>();
        for (int i = 0; i < count; i++) 
            list.Add(Instantiate(prefab, disableDirect));
        return list;
    }
    
    public GameObject Instantiate(GameObject prefab, bool disableDirect = false)
    {
        var obj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        if(disableDirect) obj.SetActive(false);
        var transform = obj.transform;
        poolDebug.allTransforms.Add(transform);
        var foundCullable = transform.GetComponentsInChildren<Cullable>();
        foreach (var cullable in foundCullable)
        {
            cullable.gameObject.SetActive(false);
            cullables.Add(cullable);
        }

        return obj;
    }
}
