using Cinemachine;
using UnityEngine;

public class CameraTrack : MonoBehaviour {
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
    [SerializeField] private CinemachineSmoothPath pathStoreToMain;
    [SerializeField] private CinemachineSmoothPath pathMapToMain;
    [SerializeField] private CinemachineSmoothPath pathBridgeToMain;
    [SerializeField] private CinemachineSmoothPath pathCreationToMain;
    [SerializeField] private CinemachineSmoothPath pathJumpToMain;
    [SerializeField] private CinemachineSmoothPath pathMainToStore;
    [SerializeField] private CinemachineSmoothPath pathMainToMap;
    [SerializeField] private CinemachineSmoothPath pathMainToBridge;
    [SerializeField] private CinemachineSmoothPath pathMainToCreation;
    [SerializeField] private CinemachineSmoothPath pathMainToJump;
    [SerializeField] private CinemachineSmoothPath pathCreationToShip;
    [SerializeField] private CinemachineSmoothPath pathJumpToWarp;

    [SerializeField] private float bobSpeed = 10f;
    [SerializeField] private float bobHeight = 0.05f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float accelerationTime = 2f;

    private CinemachineBasicMultiChannelPerlin noiseComponent;
    private CinemachineTrackedDolly trackedDolly;
    private CinemachineSmoothPath currentPath;

    private float currentSpeed = 0f;
    private float accelerationProgress = 0f;
    private float currentPosition = 100f;
    private float defaultYPos;
    private float timeElapsed = 0f;

    void Start() {
        noiseComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        trackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        defaultYPos = transform.localPosition.y;
    }

    void Update() {
        if (currentPath == null) return;

        if (currentPosition < trackedDolly.m_Path.PathLength) {
            MoveAlongPath();
            if (currentPath != pathCreationToShip && currentPath != pathJumpToWarp) {
                ApplyWalkingBob();

                if (Input.GetKeyDown(KeyCode.W)) {
                    movementSpeed = 100f;
                }
            }

            if (movementSpeed != 100f || (currentPath == pathJumpToWarp && movementSpeed < 500f)) {
                if (currentPath == pathJumpToWarp) {
                    movementSpeed += 1f;
                    movementSpeed = Mathf.Clamp(movementSpeed, 5f, 500f);
                } else if (currentPath == pathCreationToShip) {
                    movementSpeed = 15f;
                } else {
                    movementSpeed = 5f;
                }
            }

            if (currentPosition >= trackedDolly.m_Path.PathLength) {
                Utility.IsMoving = false;
                UIController.Instance.UIUpdateOnArrived(currentPath.name);
            }
        } else {
            ResetPosition();
        }

        if (UIController.Instance.UIFinishedMoving(currentPath.name)) SwitchPath(currentPath);
    }

    public void MoveAlongPath() {
        if (accelerationProgress < 1f) {
            accelerationProgress += Time.deltaTime / accelerationTime;
            currentSpeed = Mathf.Lerp(0f, movementSpeed, accelerationProgress);
        } else {
            currentSpeed = movementSpeed;
        }

        currentPosition += currentSpeed * Time.deltaTime;
        currentPosition = Mathf.Clamp(currentPosition, 0, trackedDolly.m_Path.PathLength);

        trackedDolly.m_PathPosition = currentPosition;
    }

    public void SwitchPath(CinemachineSmoothPath newPath) {
        currentPosition = 0f;
        trackedDolly.m_PathPosition = 0;
        trackedDolly.m_Path = newPath;
        movementSpeed = 5f;
        currentSpeed = 0f;
        accelerationProgress = 0f;

        if (!newPath.name.Contains("ToWarp")) AmbientManager.Instance.PlayAmbient("Walk");
    }

    public void MovePath(string path) {
        if (path == "StoreToMain") {
            currentPath = pathStoreToMain;
        } else if (path == "JumpToMain") {
            currentPath = pathJumpToMain;
        } else if (path == "CreationToMain") {
            currentPath = pathCreationToMain;
        } else if (path == "MapToMain") {
            currentPath = pathMapToMain;
        } else if (path == "BridgeToMain") {
            currentPath = pathBridgeToMain;
        } else if (path == "MainToStore") {
            currentPath = pathMainToStore;
        } else if (path == "MainToJump") {
            currentPath = pathMainToJump;
        } else if (path == "MainToCreation") {
            currentPath = pathMainToCreation;
        } else if (path == "MainToMap") {
            currentPath = pathMainToMap;
        } else if (path == "MainToBridge") {
            currentPath = pathMainToBridge;
        } else if (path == "CreationToShip") {
            currentPath = pathCreationToShip;
        } else if (path == "JumpToWarp") {
            currentPath = pathJumpToWarp;
        }

        UIController.Instance.MoveUI(currentPath.name);
    }

    private void ApplyWalkingBob() {
        timeElapsed += Time.deltaTime * bobSpeed;
        float bobOffset = Mathf.Sin(timeElapsed) * bobHeight;
        Vector3 newPosition = transform.localPosition;
        newPosition.y = defaultYPos + bobOffset;
        transform.localPosition = newPosition;
        noiseComponent.enabled = true;
    }

    private void ResetPosition() {
        timeElapsed = 0f;
        Vector3 resetPosition = transform.localPosition;
        resetPosition.y = defaultYPos;
        transform.localPosition = resetPosition;
        noiseComponent.enabled = false;
    }
}
