using UnityEngine;

public class MainController : MonoBehaviour
{
    public GameObject mainCanvasPrefab;

    private void Awake()
    {
        if (!MainScript.HasInstance()) Instantiate(mainCanvasPrefab);
    }
}
