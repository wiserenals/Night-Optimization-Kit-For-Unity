using UnityEngine;

public class CharacterController : SchedulableBehaviour
{
    private Vector3 moveDirection;
    private Animator animator;
    private bool isInteracting;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [DiscreteUpdate(1)] // Update every => currentFrame % 3 == 0
    void UpdateMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveInput, 0, 0) * Time.deltaTime * 5f;

        transform.position += moveDirection;

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    [DiscreteUpdate(2)] // Update every => currentFrame % 3 == 1
    void UpdateAnimation()
    {
        if (animator != null)
        {
            float speed = animator.GetFloat("Speed");
            animator.SetFloat("AnimationSpeed", speed);
        }
    }

    [DiscreteUpdate(3)] // Update every => currentFrame % 3 == 2
    void UpdateInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInteracting = !isInteracting;
            Debug.Log($"Interaction: {isInteracting}");
        }
        if (isInteracting)
        {
            Debug.Log("The character interacts with an object.");
        }
    }
}