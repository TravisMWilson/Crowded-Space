using UnityEngine;
using UnityEngine.SceneManagement;

public class QuantumDrive : MonoBehaviour {
    private const float MAX_COOLDOWN = 30f;

    private Item item;
    private GameObject playerShip;
    private GameObject hologramPrefab;
    private GameObject hologramInstance;

    private float cooldown = 0f;
    private float holdTime = 0f;
    private float teleportDistance = 0f;
    private float maxTeleportDistance = 50f;
    private bool isHolding = false;
    private bool isPlayer = false;

    void Start() {
        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) maxTeleportDistance = Inventory.GetBlockParameter("teleportDistTotal", item.Tier, 1);
        playerShip = GameObject.Find("PlayerShip");
        if (playerShip) isPlayer = transform.parent.name == playerShip.name;
        hologramPrefab = Resources.Load<GameObject>("Blocks/Special/Hologram");
        teleportDistance = Random.Range(5f, maxTeleportDistance);
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        if (cooldown < MAX_COOLDOWN) {
            cooldown += Time.deltaTime;
            if (isPlayer) UIController.Instance.UpdateTeleportBar(cooldown, MAX_COOLDOWN);
        }

        Transform shipTransform = transform.root;

        float maxHoldTime = 0.5f;
        float distanceToPlayer = 0f;

        if (!isPlayer) distanceToPlayer = (transform.position - playerShip.transform.position).magnitude;

        if (((Input.GetKey(KeyCode.Alpha3) && isPlayer) || (distanceToPlayer > teleportDistance && holdTime > teleportDistance && !isPlayer)) && cooldown >= MAX_COOLDOWN) {
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 0f, maxHoldTime);
            isHolding = true;

            float teleportDistance = holdTime / maxHoldTime * maxTeleportDistance;
            Vector3 targetPosition = shipTransform.position + (shipTransform.forward * teleportDistance);
            Quaternion targetRotation = shipTransform.rotation;

            if (hologramInstance == null) {
                hologramInstance = Instantiate(hologramPrefab, targetPosition, targetRotation);
            } else {
                hologramInstance.transform.SetPositionAndRotation(targetPosition, targetRotation);
            }
        } else if (isHolding) {
            float teleportDistance = holdTime / maxHoldTime * maxTeleportDistance;
            shipTransform.position += shipTransform.forward * teleportDistance;

            if (hologramInstance != null) {
                Destroy(hologramInstance);
            }

            cooldown = 0f;
            holdTime = 0f;
            isHolding = false;
        }
    }
}
