
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    private Text textMesh;

    private int times;
    private int frameCount;
    private void Awake()
    {
        textMesh = GetComponent<Text>();
    }

    private void Update()
    {
        frameCount++;
        
        if (Mathf.FloorToInt(Time.time) > times)
        {
            textMesh.text = frameCount + " FPS";
            frameCount = 0;
            times++;
        }
    }
}
