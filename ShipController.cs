using UnityEngine;

public class ShipController : MonoBehaviour {
    private const float MAX_PITCH = 89f;
    private const float MAX_DISTANCE_FROM_CENTER = 35f;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float strafeSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float shipAlignSpeed;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float cameraDistance;
    [SerializeField] private float cameraHeightOffset;

    [SerializeField] private float minCameraDistance = 5f;
    [SerializeField] private float maxCameraDistance = 20f;
    [SerializeField] private float zoomSpeed = 10f;

    private GameObject explosionFX;
    private Transform cameraTransform;
    private Transform centerObject;

    private float yaw = 0f;
    private float pitch = 0f;
    private float energyCost = 0f;
    private float speedMultiplier = 1f;
    private bool isDead = false;
    private bool hasEnoughEnergy = false;
    private Vector3 previousPosition;
    private Vector3 localVelocity;

    void Start() {
        cameraTransform = transform.Find("Main Camera");
        centerObject = GameObject.Find("WarpTunnel").transform.Find("joint27");
        explosionFX = Resources.Load<GameObject>("FX/FX_Explosion");
        previousPosition = transform.position;
    }

    void Update() {
        CheckCollisions();
        CheckAndDestroyShip();

        if (isDead) return;

        HandleKeyboardInput();
        HandleMouseInput();
        HandleZoom();
        UpdateCameraPosition();
        AlignShipWithCamera();
        ClampPositionInRadius();
        HandleBoosters();
    }

    public Vector3 GetVelocity() => localVelocity;

    private void CheckCollisions() {
        Transform shield = transform.Find("Shield(Clone)");

        if (shield != null) {
            HandleCollision(shield);
        } else {
            foreach (Transform child in transform) {
                if (child.name.Contains("Camera")) continue;
                HandleCollision(child);
            }
        }
    }

    private void HandleCollision(Transform target) {
        if (!target.TryGetComponent<Collider>(out var targetCollider)) return;

        Collider[] hitColliders = Physics.OverlapBox(targetCollider.bounds.center, targetCollider.bounds.extents);

        foreach (var hitCollider in hitColliders) {
            if (hitCollider != targetCollider && hitCollider.transform.root != target.transform.root) {
                if (hitCollider.transform.root.CompareTag("Pickup")) {
                    hitCollider.transform.root.GetComponent<Pickup>().CollectPickup();
                } else {
                    ApplyDamage(target, hitCollider.transform);
                }
            }
        }
    }

    private void ApplyDamage(Transform target, Transform hitTransform) {
        Shield targetShield = target.GetComponent<Shield>();
        ItemBlock targetItemBlock = target.GetComponent<ItemBlock>();

        Shield hitShield = hitTransform.GetComponent<Shield>();
        ItemBlock hitItemBlock = hitTransform.GetComponent<ItemBlock>();

        if (targetShield != null && hitItemBlock != null) {
            float shieldHealth = targetShield.Health;

            targetShield.ApplyDamage(hitItemBlock.Health);
            hitItemBlock.ApplyDamage(shieldHealth);
        } else if (targetItemBlock != null && hitShield != null) {
            float itemBlockHealth = targetItemBlock.Health;

            targetItemBlock.ApplyDamage(hitShield.Health);
            hitShield.ApplyDamage(itemBlockHealth);
        } else if (targetShield != null && hitShield != null) {
            float targetHealth = targetShield.Health;
            float hitHealth = hitShield.Health;

            targetShield.ApplyDamage(hitHealth);
            hitShield.ApplyDamage(targetHealth);
        } else if (targetItemBlock != null && hitItemBlock != null) {
            float targetHealth = targetItemBlock.Health;
            float hitHealth = hitItemBlock.Health;

            targetItemBlock.ApplyDamage(hitHealth);
            hitItemBlock.ApplyDamage(targetHealth);
        }
    }

    private void HandleBoosters() {
        Vector3 currentPosition = transform.position;
        Vector3 worldVelocity = (currentPosition - previousPosition) / Time.deltaTime;
        localVelocity = transform.InverseTransformDirection(worldVelocity);

        energyCost = 0;
        speedMultiplier = 1f;

        foreach (Transform child in transform) {
            if (child.TryGetComponent<BoosterScaleController>(out var boosterScale)) {
                boosterScale.ShipSpeed = 0;

                Transform booster = child.Find("Booster");
                Vector3 localBoosterUp = child.localRotation * booster.localRotation * Vector3.up;

                if (Mathf.Abs(localBoosterUp.z) > 0.9f) {
                    if (localBoosterUp.z > 0.9f && localVelocity.z > 0) {
                        boosterScale.ShipSpeed = Mathf.Abs(localVelocity.z);
                    } else if (localBoosterUp.z < -0.9f && localVelocity.z < 0) {
                        boosterScale.ShipSpeed = Mathf.Abs(localVelocity.z);
                    }
                }

                if (Mathf.Abs(localBoosterUp.y) > 0.9f) {
                    if (localBoosterUp.y > 0.9f && localVelocity.y > 0) {
                        boosterScale.ShipSpeed = Mathf.Abs(localVelocity.y);
                    } else if (localBoosterUp.y < -0.9f && localVelocity.y < 0) {
                        boosterScale.ShipSpeed = Mathf.Abs(localVelocity.y);
                    }
                }

                if (Mathf.Abs(localBoosterUp.x) > 0.9f) {
                    if (localBoosterUp.x > 0.9f && localVelocity.x > 0) {
                        boosterScale.ShipSpeed = Mathf.Abs(localVelocity.x);
                    } else if (localBoosterUp.x < -0.9f && localVelocity.x < 0) {
                        boosterScale.ShipSpeed = Mathf.Abs(localVelocity.x);
                    }
                }

                boosterScale.ShipSpeed *= 5;

                Item item = child.GetComponent<ItemBlock>().GetItem();

                if (item != null) {
                    energyCost += Inventory.BlockEnergyCosts["Engine"](item.Tier, item.GetGrade());
                    speedMultiplier *= Inventory.BlockParameters["speedTotal"](item.Tier, item.GetGrade());
                }
            }
        }

        hasEnoughEnergy = EnergyStorage.CalculateTotalCurrentEnergy(transform) >= energyCost;

        if (currentPosition != previousPosition) {
            if (hasEnoughEnergy) {
                EnergyStorage.RemoveEnergy(transform, energyCost * Time.deltaTime);

                if (!AmbientManager.Instance.IsClipPlaying("Thruster", 4)) {
                    AmbientManager.Instance.PlayAmbient("Thruster", 4);
                }
            } else {
                if (AmbientManager.Instance.IsClipPlaying("Thruster", 4)) {
                    AmbientManager.Instance.StopChannel(4);
                }
            }
        } else {
            if (AmbientManager.Instance.IsClipPlaying("Thruster", 4)) {
                AmbientManager.Instance.StopChannel(4);
            }
        }

        previousPosition = currentPosition;
    }

    private void HandleKeyboardInput() {
        if (!hasEnoughEnergy) return;

        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveUpDown = 0f;

        if (Input.GetKey(KeyCode.Q)) {
            moveUpDown = -1f;
        } else if (Input.GetKey(KeyCode.E)) {
            moveUpDown = 1f;
        }

        Vector3 forwardMovement = moveSpeed * moveVertical * cameraTransform.forward;
        Vector3 strafeMovement = moveHorizontal * strafeSpeed * cameraTransform.right;
        Vector3 verticalMovement = moveUpDown * verticalSpeed * cameraTransform.up;
        Vector3 desiredVelocity = forwardMovement + strafeMovement + verticalMovement;
        Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;

        if (desiredVelocity.magnitude + currentVelocity.magnitude > maxVelocity) {
            float scaleFactor = maxVelocity / (desiredVelocity.magnitude + currentVelocity.magnitude);
            desiredVelocity *= scaleFactor;
        }

        transform.position += speedMultiplier * Time.deltaTime * desiredVelocity;
    }

    private void HandleMouseInput() {
        if (Input.GetMouseButtonDown(1)) {
            Cursor.lockState = CursorLockMode.Locked;
        } else if (Input.GetMouseButtonUp(1)) {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButton(1)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed;
            pitch -= mouseY * rotationSpeed;

            pitch = Mathf.Clamp(pitch, -MAX_PITCH, MAX_PITCH);
        }
    }

    private void HandleZoom() {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        cameraDistance -= scrollInput * zoomSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
    }

    private void UpdateCameraPosition() {
        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 localOffset = new Vector3(0, cameraHeightOffset, -cameraDistance);
        Vector3 cameraPosition = transform.position + (cameraRotation * localOffset);                                            

        cameraTransform.position = cameraPosition;
        cameraTransform.LookAt(transform.position);
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

    private void AlignShipWithCamera() {
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, shipAlignSpeed * Time.deltaTime);
    }

    private void ClampPositionInRadius() {
        Vector3 centerPosition = centerObject.position;
        Vector3 directionFromCenter = transform.position - centerPosition;
        float distanceFromCenter = directionFromCenter.magnitude;

        if (distanceFromCenter > MAX_DISTANCE_FROM_CENTER) {
            Vector3 clampedPosition = centerPosition + (directionFromCenter.normalized * MAX_DISTANCE_FROM_CENTER);
            transform.position = clampedPosition;
        }
    }

    private void CheckAndDestroyShip() {
        if (transform.childCount == 0 || isDead) {
            if (!isDead) {
                TriggerExplosion();
                isDead = true;
            }

            return;
        }

        bool hasValidBridge = false;

        foreach (Transform child in transform) {
            string childName = child.name.ToLower();

            if (childName.Contains("bridge") && !childName.Contains("advanced")) {
                hasValidBridge = true;
                break;
            }
        }

        if (!hasValidBridge) {
            TriggerExplosion();
            isDead = true;

            Cursor.lockState = CursorLockMode.None;

            foreach (Transform child in transform) {
                if (!child.name.Contains("Camera")) {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void TriggerExplosion() {
        GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);
        explosion.transform.localScale = transform.localScale;
        explosion.GetComponent<ParticleSystem>().Play();
        SoundManager.Instance.PlaySound("Explosion");
        Destroy(explosion, 5f);
    }
}
