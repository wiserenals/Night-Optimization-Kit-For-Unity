using Karayel;
using UnityEngine;

public class NOK_CharacterController : MonoBehaviour
{
    [HideInInspector] public Vector4 movementSpeed = Vector4.one * 10;
    [HideInInspector, SerializeField] private new Rigidbody rigidbody;
    [HideInInspector, SerializeField] private Transform crouchTarget;
    
    [Header("Moving")]
    public float inputSmoothTime = 0.1f;
    [Header("Jumping")]
    public Vector3 jumpForce = Vector3.up * 9;
    public float jumpAngleStrength = 0.005f;
    [Header("Running")]
    public float additionalRunSpeed = 5;
    [Header("Crouching")]
    public float additionalCrouchSpeed = -6.5f;
    public float crouchExhaustionSpeed = 1;
    public float crouchExhaustionMultiplier = 2;
    public float crouchExhaustionStrength = 0.02f;
    public int crouchExhaustionLimit = 4;
    public Vector3 additionalCrouchPosition = new Vector3(0,-0.2f,0);
    [Header("Advanced Settings")]
    public Vector3 angularGravityStrength = new Vector3(0, -2, 0);
    public float ungroundedVelocityInterrupt = 0.4f;
    [Header("Observational")] 
    [SerializeField, ReadOnly] private bool grounded = true;
    [SerializeField, ReadOnly] private bool jumping = false;
    [SerializeField, ReadOnly] private bool running = false;
    [SerializeField, ReadOnly] private bool crouching = false;
    [SerializeField, ReadOnly] private float nearbyCrouchTime;
    
    private Vector3 nativeInputVector;
    private Vector3 NormalizedInputVector => nativeInputVector.normalized;
    private Vector3 resultInputVector;

    private Vector3 tempDampVector;
    private Vector3 defaultCrouchTargetLocalPosition;
    private Vector3 targetCrouchTargetPosition;

    private float groundAngle;

    private bool _allowGround = true;

    

    private void Start()
    {
        defaultCrouchTargetLocalPosition = crouchTarget.localPosition;
        targetCrouchTargetPosition = defaultCrouchTargetLocalPosition;

        var loop = new Loop(1000);
        loop.Start(DefaultLoop);
    }

    private void DefaultLoop()
    {
        if (nearbyCrouchTime > 0) nearbyCrouchTime--;
    }

    private void Update()
    {
        ControlInputs();
        SmoothInputs();
        ApplyInputs();
        ApplyCrouch();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private Vector3 crouchDampTemp;
    private void ApplyCrouch()
    {
        crouchTarget.localPosition = Vector3.SmoothDamp(
            crouchTarget.localPosition, 
            targetCrouchTargetPosition, 
            ref crouchDampTemp, 
            crouchExhaustionStrength * Mathf.Pow(nearbyCrouchTime + 1, crouchExhaustionMultiplier));
    }


    private void SmoothInputs()
    {
        resultInputVector = 
            Vector3.SmoothDamp(
                resultInputVector, 
                NormalizedInputVector, 
                ref tempDampVector, 
                inputSmoothTime);
    }
    
    private void ApplyGravity()
    {
        if (!grounded || groundAngle <= 35) return;
        
        Vector3 gravity = angularGravityStrength * groundAngle;
        rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void ApplyInputs()
    {
        var resultMovementVector = movementSpeed 
                                  + Vector4.one * (crouching 
                                      ? additionalCrouchSpeed 
                                      : running ? additionalRunSpeed : 0);

        var resultMovementVector3 = new Vector3(
            (resultInputVector.x > 0 ? resultMovementVector.x : resultMovementVector.y) * resultInputVector.x,
            0,
            (resultInputVector.z > 0 ? resultMovementVector.z : resultMovementVector.w) * resultInputVector.z);
        
        var newXZVelocity = transform.TransformDirection(resultMovementVector3);

        newXZVelocity.y = rigidbody.velocity.y;
        
        rigidbody.velocity = newXZVelocity;
    }

    private void ControlInputs()
    {
        MoveInputs();

        if (grounded && Input.GetKeyDown(NOK_Keyboard.Jump)) Jump(); //JumpInputs

        running = Input.GetKey(NOK_Keyboard.Run); //RunInputs

        CrouchInputs();
    }

    private void MoveInputs()
    {
        nativeInputVector = Vector3.zero;
        if (Input.GetKey(NOK_Keyboard.MoveForward)) nativeInputVector.z = 1;
        else if (Input.GetKey(NOK_Keyboard.MoveBackward)) nativeInputVector.z = -1;
        if (Input.GetKey(NOK_Keyboard.MoveRight)) nativeInputVector.x = 1;
        else if (Input.GetKey(NOK_Keyboard.MoveLeft)) nativeInputVector.x = -1;
    }

    private void CrouchInputs()
    {
        var newCrouching = Input.GetKey(NOK_Keyboard.Crouch);
        
        if (newCrouching == crouching) return;
        
        crouching = newCrouching;

        if (crouching)
        {
            nearbyCrouchTime += crouchExhaustionSpeed;
            if (nearbyCrouchTime > crouchExhaustionLimit) nearbyCrouchTime = crouchExhaustionLimit;
            targetCrouchTargetPosition 
                = defaultCrouchTargetLocalPosition + additionalCrouchPosition;
        }
        else
        {
            targetCrouchTargetPosition 
                = defaultCrouchTargetLocalPosition;
        }
        
    }

    private void Jump()
    {
        grounded = false;
        jumping = true;
        rigidbody.AddForce(jumpForce / ((groundAngle * jumpAngleStrength) + 1), ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision other)
    {
        CheckGroundEnter(other);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        CheckGroundEnter(other);
    }

    private void OnCollisionExit(Collision other)
    {
        if (grounded && other.collider.CompareTag("NOK_Ground")) Ungrounded();
    }
    
    private static readonly float _ungroundedDelay = 0.1f;
    private void Ungrounded()
    {
        Debug.Log("UNGROUNDED");
        _allowGround = false;
        Invoke(nameof(AllowGround), _ungroundedDelay);
        grounded = false;
        if (jumping) return;
        var velocity = rigidbody.velocity;
        velocity.y *= ungroundedVelocityInterrupt;
        rigidbody.velocity = velocity;
    }
    
    private void CheckGroundEnter(Collision other)
    {
        if (!_allowGround) return;
        
        Vector3 contactNormal = other.contacts[0].normal;

        if (!other.collider.CompareTag("NOK_Ground")) return;
        
        float angle = Vector3.Angle(contactNormal, Vector3.up);

        if (angle > 60) return;
        
        groundAngle = angle;

        if (grounded)
        {
            jumping = false;
            return;
        }
        
        Debug.Log("GROUNDED");
        
        grounded = true;
    }

    private void AllowGround()
    {
        _allowGround = true;
    }
}
