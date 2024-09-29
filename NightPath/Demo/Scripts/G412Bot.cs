using UnityEngine;

public class G412Bot : MonoBehaviour
{
    public NightPath nightPath;
    public NightPathAgentModel model;
    public Rigidbody rb;
    [Space(5)] public float speed = 3;

    private NightPathNode nearestNode => nightPath.nodes.GetNearestByWorldPosition(transform.position);

    private void FixedUpdate()
    {
        var flowVector = (Vector3) model.Calculate(nearestNode, nightPath.nodes);

        rb.velocity = flowVector * speed;
    }
}
