using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public Transform bossTarget;
    public float lockOnSmoothSpeed = 5f;
    public float lockOnCameraDistance = 4f;
    public float lockOnHeightOffset = 1.5f;
    public float cameraCollisionSmoothness = 10f;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;
    private bool isDodging = false;
    private float lastDodgeTime = -Mathf.Infinity;
    private bool isLockedOn = false;
    private Vector3 defaultCameraLocalPosition;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCameraLocalPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isLockedOn = !isLockedOn;
        }

        if (!isDodging && Input.GetKeyDown(KeyCode.LeftControl) && Time.time > lastDodgeTime + dodgeCooldown)
        {
            StartCoroutine(DodgeRoll());
            return;
        }

        Vector3 forward, right;
        if (isLockedOn && bossTarget != null)
        {
            forward = playerCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();
            right = playerCamera.transform.right;
            right.y = 0;
            right.Normalize();

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float speed = isRunning ? runSpeed : walkSpeed;
            
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            
            // Only add forward push when not moving backward
            if (Mathf.Abs(horizontalInput) > 0.1f && verticalInput >= 0)
            {
                verticalInput = Mathf.Max(verticalInput, Mathf.Abs(horizontalInput) * 0.2f);
            }
            
            moveDirection = (forward * verticalInput * speed) + (right * horizontalInput * speed);
        }
        else
        {
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        }

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (isLockedOn && bossTarget != null)
        {
            LockOnBehavior();
        }
        else
        {
            NormalCameraControl();
        }

        HandleCameraCollision();
    }

    void LockOnBehavior()
    {
        if (bossTarget == null) return;

        Vector3 lookDirection = bossTarget.position - transform.position;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * lockOnSmoothSpeed);
        }

        Vector3 desiredCameraPosition = transform.position - (transform.forward * lockOnCameraDistance) + Vector3.up * lockOnHeightOffset;
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredCameraPosition, Time.deltaTime * lockOnSmoothSpeed);
        playerCamera.transform.LookAt(bossTarget.position + Vector3.up * lockOnHeightOffset);
    }

    void NormalCameraControl()
    {
        rotationX += -Input.GetAxis("Mouse Y") * 2f;
        rotationX = Mathf.Clamp(rotationX, -45f, 45f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 2f, 0);
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

    IEnumerator DodgeRoll()
    {
        isDodging = true;
        canMove = false;
        lastDodgeTime = Time.time;

        Vector3 dodgeDirection = Vector3.zero;
        
        if (isLockedOn && bossTarget != null)
        {
            if (Input.GetKey(KeyCode.W)) dodgeDirection += playerCamera.transform.forward;
            if (Input.GetKey(KeyCode.S)) dodgeDirection -= playerCamera.transform.forward;
            if (Input.GetKey(KeyCode.A)) dodgeDirection -= playerCamera.transform.right;
            if (Input.GetKey(KeyCode.D)) dodgeDirection += playerCamera.transform.right;
        }
        else
        {
            if (Input.GetKey(KeyCode.W)) dodgeDirection += transform.forward;
            if (Input.GetKey(KeyCode.S)) dodgeDirection -= transform.forward;
            if (Input.GetKey(KeyCode.A)) dodgeDirection -= transform.right;
            if (Input.GetKey(KeyCode.D)) dodgeDirection += transform.right;
        }

        if (dodgeDirection == Vector3.zero)
        {
            dodgeDirection = isLockedOn ? playerCamera.transform.forward : transform.forward;
        }

        dodgeDirection.y = 0;
        dodgeDirection.Normalize();
        
        float elapsedTime = 0;
        while (elapsedTime < dodgeDuration)
        {
            characterController.Move(dodgeDirection * (dodgeDistance / dodgeDuration) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canMove = true;
        isDodging = false;
    }
}