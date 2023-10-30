using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_CullingDemoScript : MonoBehaviour
{
    private void Start()
    {
        transform.position += 10 * Random.insideUnitSphere;
    }
}
