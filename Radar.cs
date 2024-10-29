using UnityEngine;
using UnityEngine.SceneManagement;

public class Radar : MonoBehaviour {
    private const float MAX_COOLDOWN = 30f;

    private Item item;
    private GameObject playerShip;
    private Renderer playerRenderer;
    private float cooldown = 0f;
    private float radarRadius = 0f;
    private bool isPlayer = false;
    private bool searchingForPlayer = false;

    void Start() {
        playerShip = GameObject.Find("PlayerShip");

        if (playerShip) {
            playerRenderer = playerShip.transform.Find("Bridge1(Clone)").GetComponent<Renderer>();
            isPlayer = transform.parent.name == playerShip.name;
        }

        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) radarRadius = Inventory.GetBlockParameter("radarDistTotal", item.Tier, 1);
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        if (cooldown < MAX_COOLDOWN) {
            cooldown += Time.deltaTime;
            if (isPlayer) UIController.Instance.UpdateRadarBar(cooldown, MAX_COOLDOWN);
        }

        if (!isPlayer && playerRenderer != null) searchingForPlayer = playerRenderer.material.name.Contains("Cloak");

        if (((Input.GetKeyDown(KeyCode.Alpha4) && isPlayer) || searchingForPlayer) && cooldown >= MAX_COOLDOWN) {
            SoundManager.Instance.PlaySound("Radar");

            Collider[] colliders = Physics.OverlapSphere(transform.position, radarRadius);

            foreach (Collider col in colliders) {
                if (col.transform.root != transform.root) {
                    if (col.TryGetComponent<ItemBlock>(out var itemBlock)) {
                        itemBlock.SwapMaterial(true);
                    }
                }
            }

            cooldown = 0;
        }
    }
}
