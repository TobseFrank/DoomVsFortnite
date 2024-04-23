using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public float WalkSpeed = 5f;
    public float SprintMultiplier = 2f;
    public float JumpForce = 5f;
    public float TrampoForce = 50f;
    public float GroundCheckDistance = 1.5f;
    public float LookSensitivityX = 1f;
    public float LookSensitivityY = 1f;
    public float MinYLookAngle = -90f;
    public float MaxYLookAngle = 90f;
    public Transform PlayerCamera;
    public float Gravity = -9.8f;
    private Vector3 velocity;
    private float verticalRotation;

    private CharacterController characterController;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
        moveDirection.Normalize();

        float speed = WalkSpeed;

        if (Input.GetKey(KeyCode.LeftAlt)) {
            speed *= SprintMultiplier;
        }

        characterController.Move(speed * Time.deltaTime * moveDirection);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, GroundCheckDistance)) {
            if (hit.transform.tag.Equals("Trampo")){
                velocity.y = TrampoForce;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
            velocity.y = JumpForce;
        } else {
            velocity.y += Gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);

        if (PlayerCamera != null){
            float mouseX = Input.GetAxis("Mouse X") * LookSensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * LookSensitivityY;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, MinYLookAngle, MaxYLookAngle);

            PlayerCamera.localRotation = Quaternion.Euler(verticalRotation,0f,0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    bool IsGrounded() {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, GroundCheckDistance))
        {
            return true;
        }
        return false;
    }
}