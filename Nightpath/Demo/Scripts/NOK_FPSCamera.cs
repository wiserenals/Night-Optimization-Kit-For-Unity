using System;
using UnityEngine;

public class NOK_FPSCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public float rotationSmoothTime = 0.1f;

    private Vector3 rotation;
    private Vector3 rotationVelocity;
    private Vector3 currentRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotation.y += mouseX;
        rotation.x -= mouseY;
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);
    }

    private void FixedUpdate()
    {
        currentRotation = Vector3.SmoothDamp(currentRotation, rotation, ref rotationVelocity, 
            rotationSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0, currentRotation.y, 0);
    }
}