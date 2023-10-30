using UnityEngine;

public class LegacyAgentNavigation : LegacyAgentBase
{
    public Rigidbody rb;
    public float force = 1;

    private void Start() 
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    private void FixedUpdate()
    {
        GetFlow();
    }

    protected override void OnFlow(Vector2 flowVector)
    {
        Vector3 moveDir = new Vector3(flowVector.x, 0, flowVector.y);
        rb.velocity = moveDir * force;
    }
}
