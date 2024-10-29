using UnityEngine;
using UnityEngine.SceneManagement;

public class TurretController : MonoBehaviour {
    [SerializeField] private Transform turretBase;
    [SerializeField] private Transform turretGun;
    [SerializeField] private ParticleSystem[] firingEffects;

    [SerializeField] private float maxCooldown = 30f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float maxRayDistance = 1000f;
    [SerializeField] private int enemyShootingDistance = 100;

    private GameObject playerShip;
    private AudioSource weaponSound;
    private Item item;

    private float cooldown = 0f;
    private float laserTimer = 3f;
    private float laserFireTime = 3f;
    private float energyCostPerShot;

    private bool fireLaser = false;
    private bool isPlayer = false;
    private bool isLaser = false;

    private AimBehavior aimBehavior;

    void Start() {
        foreach (ParticleSystem shot in firingEffects) {
            shot.Stop();
        }

        playerShip = GameObject.Find("PlayerShip");

        if (playerShip != null) {
            isPlayer = transform.parent.parent.name == playerShip.name;
            isLaser = transform.parent.name.Contains("Laser");
            item = transform.parent.GetComponent<ItemBlock>().GetItem();
            weaponSound = transform.parent.GetComponent<AudioSource>();
            weaponSound.mute = false;

            if (item != null) {
                if (isLaser) {
                    energyCostPerShot = Inventory.BlockEnergyCosts["Laser"](item.Tier, item.GetGrade());
                } else if (transform.parent.name.Contains("Missile")) {
                    energyCostPerShot = Inventory.BlockEnergyCosts["Missile Launcher"](item.Tier, item.GetGrade());
                } else if (transform.parent.name.Contains("Railgun")) {
                    energyCostPerShot = Inventory.BlockEnergyCosts["Railgun"](item.Tier, item.GetGrade());
                } else if (transform.parent.name.Contains("Minigun")) {
                    energyCostPerShot = Inventory.BlockEnergyCosts["Minigun"](item.Tier, item.GetGrade());
                } else if (transform.parent.name.Contains("Cannon")) {
                    energyCostPerShot = Inventory.BlockEnergyCosts["Cannon"](item.Tier, item.GetGrade());
                }
            }

            if (!isPlayer) {
                aimBehavior = transform.parent.parent.GetComponent<CreationBehavior>().GetAimBehavior();
                ConfigureEnemyShotTags();
            }
        }
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        AimTurret();
        HandleFiring();
    }

    private bool IsPlayerCloaked() {
        if (playerShip == null) return false;

        Transform bridge = playerShip.transform.Find("Bridge1(Clone)");
        if (bridge == null) return false;

        return bridge.GetChild(0).GetComponent<Renderer>().material.name.Contains("Cloak");
    }

    private void ConfigureEnemyShotTags() {
        foreach (ParticleSystem shot in firingEffects) {
            if (transform.parent.name.Contains("Missile")) {
                shot.transform.tag = "EnemyMissile";
                var collision = shot.collision;
                collision.collidesWith = LayerMask.GetMask("Player");
            } else {
                shot.transform.GetChild(0).tag = "Enemy";
                var collision = shot.transform.GetChild(0).GetComponent<ParticleSystem>().collision;
                collision.collidesWith = LayerMask.GetMask("Player");
            }
        }
    }

    private void HandleFiring() {
        Transform playerBridge = playerShip.transform.Find("Bridge1(Clone)");
        float distanceToPlayer = 10000f;

        if (cooldown < maxCooldown && isLaser) {
            cooldown += Time.deltaTime;
            if (isPlayer) UIController.Instance.UpdateLaserBar(cooldown, maxCooldown);
        }

        if (laserTimer >= laserFireTime && isLaser && fireLaser) {
            StopWeapon();
        }

        if (!isPlayer && playerBridge != null) {
            distanceToPlayer = (transform.position - playerBridge.position).magnitude;
        }

        if (((Input.GetKeyDown(KeyCode.Alpha1) && isPlayer) || (!isPlayer && !IsPlayerCloaked())) && isLaser && cooldown >= maxCooldown) {
            fireLaser = true;
            laserTimer = 0f;
            cooldown = 0f;
        }

        if (((Input.GetMouseButton(0) && isPlayer) || (distanceToPlayer < enemyShootingDistance && !IsPlayerCloaked())) && (fireLaser || !isLaser)) {
            if (EnergyStorage.CalculateTotalCurrentEnergy(transform.parent.parent) >= energyCostPerShot) {
                EnergyStorage.RemoveEnergy(transform.parent.parent, energyCostPerShot * Time.deltaTime);
                FireWeapon();
            } else {
                StopWeapon();
            }
        } else {
            StopWeapon();
        }
    }

    private void FireWeapon() {
        foreach (ParticleSystem shot in firingEffects) {
            if (!shot.isPlaying) {
                shot.Play();
                if (isPlayer) weaponSound.Play();
                Utility.ApplyToAllChildren(shot.transform, (Transform child) => child.GetComponent<ParticleSystem>().Play());
            }
        }

        if (isLaser && fireLaser && laserTimer < laserFireTime) {
            laserTimer += Time.deltaTime;
        }
    }

    private void StopWeapon() {
        foreach (ParticleSystem shot in firingEffects) {
            if (shot.isPlaying) {
                shot.Stop();
                if (isPlayer) weaponSound.Stop();
                Utility.ApplyToAllChildren(shot.transform, (Transform child) => child.GetComponent<ParticleSystem>().Stop());
            }
        }

        if (laserTimer >= laserFireTime) {
            fireLaser = false;
        }
    }

    private void AimTurret() {
        Vector3 targetPoint = isPlayer ? GetPlayerAimingPoint() : SetAimTarget();

        bool isLayer5 = gameObject.layer == 5;

        AimBase(turretBase, targetPoint, isLayer5);
        AimGun(turretGun, targetPoint, turretBase, isLayer5);
    }

    private Vector3 GetPlayerAimingPoint() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask ignoreLayers = LayerMask.GetMask("IgnoreRaycast", "Player", "Default");
        ignoreLayers = ~ignoreLayers;
        return Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, ignoreLayers) ? hit.point : ray.GetPoint(maxRayDistance);
    }

    private Vector3 SetAimTarget() {
        Transform playerBridge = playerShip.transform.Find("Bridge1(Clone)");
        if (playerBridge == null) return playerShip.transform.position;

        return aimBehavior switch {
            AimBehavior.Straight => transform.parent.position + (transform.parent.forward * 10f),
            AimBehavior.AtPlayerCenter => playerBridge.position,
            AimBehavior.InfrontOfPlayer => playerBridge.position + (playerBridge.forward * 10f),
            AimBehavior.BehindPlayer => playerBridge.position - (playerBridge.forward * 10f),
            AimBehavior.MatchPlayerVelocity => playerBridge.position + playerShip.GetComponent<ShipController>().GetVelocity(),
            AimBehavior.Circle => GetCircleAimPoint(),
            AimBehavior.Spread => playerBridge.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)),
            AimBehavior.Random => new Vector3(Random.Range(-maxRayDistance, maxRayDistance), Random.Range(-maxRayDistance, maxRayDistance), Random.Range(-maxRayDistance, maxRayDistance)),
            _ => playerBridge.position,
        };
    }

    private Vector3 GetCircleAimPoint() {
        Transform playerBridge = playerShip.transform.Find("Bridge1(Clone)");
        float angle = Time.time * rotationSpeed;
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * 10f;
        return playerBridge.position + offset;
    }

    private void AimBase(Transform baseTransform, Vector3 targetPoint, bool towardsCamera) {
        Vector3 directionToFace = towardsCamera ? baseTransform.position - targetPoint : targetPoint - baseTransform.position;
        Vector3 localDirection = transform.InverseTransformDirection(directionToFace);

        float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg * (Time.deltaTime * rotationSpeed);

        baseTransform.Rotate(0, angle, 0);
    }

    private void AimGun(Transform gunTransform, Vector3 targetPoint, Transform baseTransform, bool towardsCamera) {
        Vector3 directionToFace = towardsCamera ? gunTransform.position - targetPoint : targetPoint - gunTransform.position;
        Vector3 localDirection = baseTransform.InverseTransformDirection(directionToFace);

        Quaternion targetRotation = Quaternion.LookRotation(localDirection);
        Quaternion xRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, 0);

        gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, xRotation, Time.deltaTime * rotationSpeed);
    }
}