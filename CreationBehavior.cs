using UnityEngine;

public enum SpawnBehavior {
    Infront = 0,
    Behind = 1
}

public enum MoveBehavior {
    MoveStraight = 0,
    FollowPlayer = 1,
    OrbitPlayer = 2,
    SurroundPlayer = 3,
    StayInfrontOfPlayer = 4,
    ZigZagFollowPlayer = 5,
    StrafeFollowPlayer = 6
}

public enum FaceBehavior {
    FaceForward = 0,
    FacePlayer = 1
}

public enum DodgeBehavior {
    DontDodge = 0,
    DodgeWhenHit = 1,
    DodgePlayerAim = 2
}

public enum AimBehavior {
    Straight = 0,
    AtPlayerCenter = 1,
    InfrontOfPlayer = 2,
    BehindPlayer = 3,
    MatchPlayerVelocity = 4,
    Circle = 5,
    Spread = 6,
    Random = 7
}

public class CreationBehavior : MonoBehaviour {
    private const int REMOVAL_DISTANCE = 850;

    private GameObject explosionFX;
    private Transform playerShip;
    private Transform bridge;
    private Camera playerCamera;

    private Vector3 aimDirection = Vector3.zero;
    private Vector3 dodgeDirection;
    private Vector3 previousPosition;

    private float dodgeTime = 2f;
    private float dodgeTimer = 0f;
    private float zigzagTimer = 0f;
    private float strafeTimer = 0f;
    private float warpRadius = 12f;
    private float distance = 0f;
    private float minScale = 0.5f;
    private float maxScale = 4f;
    private float minSpeed = 3f;
    private float maxSpeed = 40f;
    private float speed;
    private float originalSpeed;
    private float targetSpeed;
    private float speedChangeDuration = 0.5f;
    private float speedChangeTimer = 0f;
    private float spawnDelay = 0.1f;
    private float energyCost = 0f;
    private float speedMultiplier = 1f;

    private bool isSpeedingUp = false;
    private bool hasBridge = false;
    private bool isDodging = false;
    private bool strafeLeft = true;
    private bool hasEnoughEnergy = false;

    private SpawnBehavior spawnBehavior = 0;
    private MoveBehavior moveBehavior = 0;
    private FaceBehavior faceBehavior = 0;
    private DodgeBehavior dodgeBehavior = 0;
    private AimBehavior aimBehavior = 0;

    void Start() {
        if (minSpeed > maxSpeed) {
            (maxSpeed, minSpeed) = (minSpeed, maxSpeed);
        }

        playerShip = GameObject.Find("PlayerShip").transform;
        playerCamera = Camera.main;
        speed = Random.Range(minSpeed, maxSpeed);
        originalSpeed = speed;
        explosionFX = Resources.Load<GameObject>("FX/FX_Explosion");
        previousPosition = transform.position;

        foreach (Transform child in transform) {
            string childName = child.name.ToLower();

            if (childName.Contains("bridge") && !childName.Contains("advanced")) {
                hasBridge = true;
                break;
            }
        }
    }

    void Update() {
        spawnDelay -= Time.deltaTime;
        if (spawnDelay > 0) return;

        CheckAndDestroyShip();
        HandleBoosters();
        CheckForWeapons();

        if (transform.position.z is > 150 or < -150) {
            if (!isSpeedingUp) {
                targetSpeed = speed * 25f;
                speedChangeTimer = 0f;
                isSpeedingUp = true;
            }

            speedChangeTimer += Time.deltaTime;
            speed = Mathf.Lerp(speed, targetSpeed, speedChangeTimer / speedChangeDuration);
            speed = Mathf.Clamp(speed, minSpeed, targetSpeed) * speedMultiplier;
        } else {
            if (isSpeedingUp) {
                targetSpeed = originalSpeed;
                speedChangeTimer = 0f;
                isSpeedingUp = false;
            }

            speedChangeTimer += Time.deltaTime;
            speed = Mathf.Lerp(speed, targetSpeed, speedChangeTimer / speedChangeDuration) * speedMultiplier;
        }

        if (isDodging && hasEnoughEnergy && !IsPlayerCloaked()) {
            Dodge();
            return;
        }

        if (dodgeBehavior == DodgeBehavior.DodgePlayerAim && IsPlayerAimingAtShip()) {
            StartDodge();
            return;
        }

        float currentDistance = Vector3.Distance(transform.position, playerShip.position);

        if (currentDistance <= distance) {
            return;
        }

        if (playerShip != null && hasEnoughEnergy) {
            bridge = playerShip.transform.Find("Bridge1(Clone)");
            if (bridge == null) bridge = playerShip;

            if (moveBehavior == MoveBehavior.OrbitPlayer && !IsPlayerCloaked()) {
                OrbitPlayer();
            } else if (moveBehavior == MoveBehavior.SurroundPlayer && !IsPlayerCloaked()) {
                SurroundPlayer();
            } else if (moveBehavior == MoveBehavior.StayInfrontOfPlayer && !IsPlayerCloaked()) {
                MoveInFrontOfPlayer();
            } else if (moveBehavior == MoveBehavior.ZigZagFollowPlayer && !IsPlayerCloaked()) {
                ZigzagTowardsPlayer();
            } else if (moveBehavior == MoveBehavior.StrafeFollowPlayer && !IsPlayerCloaked()) {
                StrafePlayer();
            } else if (moveBehavior == MoveBehavior.FollowPlayer && !IsPlayerCloaked()) {
                FollowPlayerShip();
            } else if (moveBehavior == MoveBehavior.MoveStraight) {
                MoveToMiddle();
            }

            if (faceBehavior == FaceBehavior.FacePlayer && !IsPlayerCloaked()) {
                FacePlayer();
            } else {
                FaceForawrd();
            }
        }

        if (transform.position.x > REMOVAL_DISTANCE || transform.position.x < -REMOVAL_DISTANCE
            || transform.position.y > REMOVAL_DISTANCE || transform.position.y < -REMOVAL_DISTANCE
            || transform.position.z > REMOVAL_DISTANCE || transform.position.z < -REMOVAL_DISTANCE) {
            Destroy(gameObject);
        }
    }

    public void SetSpawnBehavior(int behavior) => spawnBehavior = (SpawnBehavior)behavior;
    public void SetMoveBehavior(int behavior) => moveBehavior = (MoveBehavior)behavior;
    public void SetFaceBehavior(int behavior) => faceBehavior = (FaceBehavior)behavior;
    public void SetDodgeBehavior(int behavior) => dodgeBehavior = (DodgeBehavior)behavior;
    public void SetAimBehavior(int behavior) => aimBehavior = (AimBehavior)behavior;
    public void SetScaleMinimun(float min) => minScale = Mathf.Clamp(min, 5, 40) / 10;
    public void SetScaleMaximun(float max) => maxScale = Mathf.Clamp(max, 5, 40) / 10;
    public void SetSpeedMinimun(float min) => minSpeed = Mathf.Clamp(min, 3, 40);
    public void SetSpeedMaximun(float max) => maxSpeed = Mathf.Clamp(max, 3, 40);
    public void SetDistance(float dist) => distance = Mathf.Clamp(dist, 0, 100);

    public bool IsSpawnInfront() => spawnBehavior == SpawnBehavior.Infront;
    public float GetDistance() => distance;
    public AimBehavior GetAimBehavior() => aimBehavior;

    public void MoveToSpawnLocation() {
        if (minScale > maxScale) {
            (maxScale, minScale) = (minScale, maxScale);
        }

        float randomXPositon = Random.Range(-100f, 100f);
        float randomYPositon = Random.Range(-100f, 100f);
        float randomScale = Random.Range(minScale, maxScale);

        int direction = IsSpawnInfront() ? 1 : -1;

        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        transform.position = new Vector3(randomXPositon, randomYPositon, 800f * direction);

        Vector3 warpPosition = GameObject.Find("WarpTunnel").transform.Find("joint27").position;
        Vector3 targetPosition = warpPosition + new Vector3(Random.Range(-warpRadius, warpRadius), Random.Range(-warpRadius, warpRadius), 0f);
        aimDirection = (targetPosition - transform.position).normalized;
    }

    private void FollowPlayerShip() {
        Vector3 direction = (playerShip.position - transform.position).normalized;
        transform.position += speed * Time.deltaTime * direction;
    }

    private bool IsPlayerCloaked() {
        if (playerShip == null) return false;

        Transform shipBridge = playerShip.transform.Find("Bridge1(Clone)");

        return shipBridge != null && shipBridge.GetChild(0).GetComponent<Renderer>().material.name.Contains("Cloak");
    }

    private void MoveToMiddle() => transform.position += speed * Time.deltaTime * aimDirection;

    private void OrbitPlayer() {
        float angle = Time.time * speed;
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * distance;
        Vector3 targetPosition = bridge.position + offset;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += speed * Time.deltaTime * direction;
    }

    private void SurroundPlayer() {
        Vector3 directionToPlayer = (bridge.position - transform.position).normalized;
        Vector3 flankDirection = Vector3.Cross(directionToPlayer, Vector3.up).normalized * distance;
        Vector3 targetPosition = bridge.position + flankDirection;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += speed * Time.deltaTime * direction;
    }

    private void FacePlayer() {
        Vector3 direction = (bridge.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
    }

    private void FaceForawrd() {
        Vector3 movementDirection = transform.position - previousPosition;

        if (movementDirection.magnitude > 0.01f) {
            movementDirection.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    private void MoveInFrontOfPlayer() {
        Vector3 targetPosition = bridge.position + (bridge.forward * distance);
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += speed * Time.deltaTime * direction;
    }

    private void ZigzagTowardsPlayer() {
        zigzagTimer += Time.deltaTime * 5f;
        float zigzagOffset = Mathf.Sin(zigzagTimer) * 10f;
        Vector3 direction = (bridge.position - transform.position).normalized;
        Vector3 zigzagDirection = direction + (transform.right * zigzagOffset);
        transform.position += speed * Time.deltaTime * zigzagDirection.normalized;
    }

    private void StrafePlayer() {
        strafeTimer += Time.deltaTime;

        if (strafeTimer >= 4f) {
            strafeLeft = !strafeLeft;
            strafeTimer = 0f;
        }

        Vector3 direction = (bridge.position - transform.position).normalized;
        Vector3 strafeDirection = strafeLeft ? transform.right : -transform.right;
        transform.position += speed * Time.deltaTime * (direction + strafeDirection).normalized;
    }

    private void StartDodge() {
        isDodging = true;
        dodgeTimer = 0f;

        float random = Random.value;

        if (random < 0.25f) {
            dodgeDirection = Vector3.up;
        } else if (random < 0.5f) {
            dodgeDirection = Vector3.down;
        } else if (random < 0.75f) {
            dodgeDirection = Vector3.left;
        } else {
            dodgeDirection = Vector3.right;
        }
    }

    private void Dodge() {
        transform.position += speed * Time.deltaTime * dodgeDirection;
        dodgeTimer += Time.deltaTime;

        if (dodgeTimer >= dodgeTime) {
            isDodging = false;
        }
    }

    private bool IsPlayerAimingAtShip() {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.transform.parent != null) {
                if (hit.transform.parent.transform == transform) {
                    return true;
                }
            }
        }

        return false;
    }

    private void CheckAndDestroyShip() {
        if (transform.childCount == 0) {
            TriggerExplosion();
            Destroy(gameObject);
            return;
        }

        bool hasValidBridge = false;

        foreach (Transform child in transform) {
            string childName = child.name.ToLower();

            if (hasBridge && childName.Contains("bridge") && !childName.Contains("advanced")) {
                hasValidBridge = true;
                break;
            }
        }

        if (!hasValidBridge && hasBridge) {
            foreach (Transform child in transform) {
                if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                    itemBlock.ApplyDamage(itemBlock.Health);
                }
            }
        }
    }

    private void CheckForWeapons() {
        bool hasWeapons = false;

        foreach (Transform child in transform) {
            if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                Item item = itemBlock.GetItem();

                if (item != null) {
                    if (item.Type == ItemType.Weapon) {
                        hasWeapons = true;
                        break;
                    }
                }
            }
        }

        if (!hasWeapons) distance = 0f;
    }

    private void TriggerExplosion() {
        GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);
        explosion.transform.localScale = transform.localScale;
        SoundManager.Instance.PlaySound("Explosion");
        Destroy(explosion, 3f);
    }

    private void HandleBoosters() {
        Vector3 currentPosition = transform.position;
        Vector3 worldVelocity = (currentPosition - previousPosition) / Time.deltaTime;
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

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

        if (currentPosition != previousPosition && hasEnoughEnergy) {
            EnergyStorage.RemoveEnergy(transform, energyCost * Time.deltaTime);
        }

        previousPosition = currentPosition;
    }
}
