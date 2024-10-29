using UnityEngine;

public class FreeCameraMovement : MonoBehaviour {
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomDistance = 5f;
    [SerializeField] private float maxZoomDistance = 50f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Update() {
        HandleMovement();
        HandleMouseLook();
        HandleZoom();
    }

    void HandleMovement() {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Q)) {
            moveY = -1f;
        } else if (Input.GetKey(KeyCode.E)) {
            moveY = 1f;
        }

        Vector3 movement = new Vector3(moveX, moveY, moveZ);
        transform.Translate(moveSpeed * Time.deltaTime * movement, Space.Self);
    }

    void HandleMouseLook() {
        if (Input.GetMouseButton(1)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * rotationSpeed;

            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }

    void HandleZoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 direction = scroll * zoomSpeed * transform.forward;
        float distance = Vector3.Distance(transform.position + direction, transform.position);

        if (distance > minZoomDistance && distance < maxZoomDistance) {
            transform.Translate(direction, Space.World);
        }
    }
}
