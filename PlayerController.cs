using UnityEngine;

public class PlayerController : MonoBehaviour {
    private const float MAX_PITCH = 89f;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float strafeSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float playerAlignSpeed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float cameraDistance;
    [SerializeField] private float cameraHeightOffset;

    [SerializeField] private float boostMultiplier = 2.0f;

    private Rigidbody rigid;
    private Transform cameraTransform;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start() {
        rigid = GetComponent<Rigidbody>();
        cameraTransform = transform.Find("Main Camera");
    }

    void Update() {
        HandleKeyboardInput();
        HandleMouseInput();
        UpdateCameraPosition();
        AlignPlayerWithCamera();
        HandleBoosters();
    }

    private void HandleBoosters() {
        Vector3 localVelocity = transform.InverseTransformDirection(transform.GetComponent<Rigidbody>().velocity) * 10;

        foreach (Transform child in transform) {
            if (child.TryGetComponent<BoosterScaleController>(out var boosterScale)) {
                boosterScale.ShipSpeed = 0;

                if ((Input.GetKey(KeyCode.Q) && child.name.Contains("Down")) || (Input.GetKey(KeyCode.E) && child.name.Contains("Up"))) {
                    boosterScale.ShipSpeed = Mathf.Abs(localVelocity.y);
                } else if ((Input.GetKey(KeyCode.W) && child.name.Contains("Forward")) || (Input.GetKey(KeyCode.S) && child.name.Contains("Back"))) {
                    boosterScale.ShipSpeed = Mathf.Abs(localVelocity.z);
                } else if ((Input.GetKey(KeyCode.A) && child.name.Contains("Left")) || (Input.GetKey(KeyCode.D) && child.name.Contains("Right"))) {
                    boosterScale.ShipSpeed = Mathf.Abs(localVelocity.x);
                }
            }
        }
    }

    private void HandleKeyboardInput() {
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveUpDown = 0f;

        if (Input.GetKey(KeyCode.Q)) {
            moveUpDown = -1f;
        } else if (Input.GetKey(KeyCode.E)) {
            moveUpDown = 1f;
        }

        float currentMoveSpeed = moveSpeed;
        float currentStrafeSpeed = strafeSpeed;
        float currentVerticalSpeed = verticalSpeed;
        float currentMaxVelocity = maxVelocity;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            currentMoveSpeed *= boostMultiplier;
            currentStrafeSpeed *= boostMultiplier;
            currentVerticalSpeed *= boostMultiplier;
            currentMaxVelocity *= boostMultiplier;
        }

        if (cameraTransform == null) return;

        Vector3 forwardMovement = currentMoveSpeed * moveVertical * cameraTransform.forward;
        Vector3 strafeMovement = currentStrafeSpeed * moveHorizontal * cameraTransform.right;
        Vector3 verticalMovement = currentVerticalSpeed * moveUpDown * cameraTransform.up;

        if (rigid.velocity.magnitude < currentMaxVelocity) {
            rigid.AddForce(forwardMovement + strafeMovement + verticalMovement);
        }
    }

    private void HandleMouseInput() {
        if (Input.GetMouseButton(1)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * rotationSpeed;

            pitch = Mathf.Clamp(pitch, -MAX_PITCH, MAX_PITCH);
        }
    }

    private void UpdateCameraPosition() {
        if (cameraTransform == null) return;

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 localOffset = new Vector3(0, cameraHeightOffset, -cameraDistance);
        Vector3 cameraPosition = transform.position + (cameraRotation * localOffset);                                            

        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(transform.position);
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    private void AlignPlayerWithCamera() {
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerAlignSpeed * Time.deltaTime);
    }
}
