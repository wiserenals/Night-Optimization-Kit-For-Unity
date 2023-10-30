using Karayel;
using UnityEngine;

public class NightLimiter : MonoBehaviour
{
    private void Awake()
    {
        var loop = new Loop(1000);
        loop.Start(() => Application.targetFrameRate = NightContainer.CurrentProperties.fpsLimit);
    }
}
