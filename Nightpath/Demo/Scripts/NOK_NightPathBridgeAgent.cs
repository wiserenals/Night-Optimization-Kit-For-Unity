using UnityEngine;

public class NOK_NightPathBridgeAgent : MonoBehaviour, IOptimizedAgentComponent
{
    public float smoothTime;
    private Transform _transform;
    private bool isActive;
    public Vector3 nativePosition { get; set; }
    
    public void SetNativePosition(Vector3 position)
    {
        nativePosition = position;
        if (isActive) return;
        //gameObject.SetActive(true);
        _transform.position = position;
    }

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        isActive = gameObject.activeSelf;
        _transform.position = Vector3.Lerp(_transform.position, nativePosition, Time.deltaTime * smoothTime);
    }

}
