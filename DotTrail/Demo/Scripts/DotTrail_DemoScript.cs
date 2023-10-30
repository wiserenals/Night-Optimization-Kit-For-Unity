using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DotTrail.Demo
{
    public class DotTrail_DemoScript : MonoBehaviour
    {
        private void Start()
        {
            Dot.Trail
                .After(container => // 2) main
                {
                    Debug.Log("Result Value: " + container[0]);
                }, container => // 1) secondary
                {
                    var i = 0;
                    for (int j = 0; j < 1000000; j++) i = j;
                    container.Add(i);
                })
                    .Parallel()
                .Loop(() =>
                {
                    Debug.Log("1");
                    return true;
                }, 1)
                    .Parallel()
                .Wait(5)
                .Loop(() =>
                {
                    Debug.Log("2");
                    return true;
                }, 1);
        }
    }
}
