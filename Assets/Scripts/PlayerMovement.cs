using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    public float cameraCollisionSmoothness = 10f;
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private Vector3 defaultCameraLocalPosition;
    private bool isDodging = false;
    private float lastDodgeTime = -Mathf.Infinity;

    void Start()
    {
        
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCameraLocalPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        if (!isDodging && Input.GetKeyDown(KeyCode.LeftControl) && Time.time > lastDodgeTime + dodgeCooldown)
        {
            StartCoroutine(DodgeRoll());
            return;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        HandleCameraCollision();
    }

    IEnumerator DodgeRoll()
{
    isDodging = true;
    canMove = false;
    lastDodgeTime = Time.time;

    // Calculate dodge direction based on WASD input
    Vector3 dodgeDirection = Vector3.zero;

    if (Input.GetKey(KeyCode.W)) // Forward
        dodgeDirection += transform.forward;

    if (Input.GetKey(KeyCode.S)) // Backward
        dodgeDirection -= transform.forward;

    if (Input.GetKey(KeyCode.A)) // Left
        dodgeDirection -= transform.right;

    if (Input.GetKey(KeyCode.D)) // Right
        dodgeDirection += transform.right;

    // If no direction was pressed, default to forward
    if (dodgeDirection == Vector3.zero)
        dodgeDirection = transform.forward;

    // Normalize the direction to prevent faster dodging in diagonal directions
    dodgeDirection.Normalize();

    float elapsedTime = 0;

    while (elapsedTime < dodgeDuration)
    {
        // Move the character in the dodge direction
        characterController.Move(dodgeDirection * (dodgeDistance / dodgeDuration) * Time.deltaTime);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    canMove = true;
    isDodging = false;
}


    void HandleCameraCollision()
    {
        Vector3 desiredCameraWorldPosition = transform.TransformPoint(defaultCameraLocalPosition);
        RaycastHit hit;
        if (Physics.Linecast(transform.position, desiredCameraWorldPosition, out hit))
        {
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, hit.point + hit.normal * 0.1f, Time.deltaTime * cameraCollisionSmoothness);
        }
        else
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, defaultCameraLocalPosition, Time.deltaTime * cameraCollisionSmoothness);
        }
    }
}
