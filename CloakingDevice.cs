using UnityEngine;
using UnityEngine.SceneManagement;

public class CloakingDevice : MonoBehaviour {
    private const float MAX_COOLDOWN = 30f;
    private const float ENERGY_COST = 30;

    private GameObject playerShip;

    private float cooldown = 0f;
    private float cloakTimer = 0f;
    private float maxCloakTime = 3f;

    private bool isPlayer = false;
    private bool isInvisible = false;

    void Start() {
        playerShip = GameObject.Find("PlayerShip");

        if (playerShip != null) {
            isPlayer = transform.parent.name == playerShip.name;
        }

        Transform parent = transform.parent;

        foreach (Transform child in parent) {
            if (child.name.Contains("Cloak")) {
                Item childItem = child.GetComponent<ItemBlock>().GetItem();

                if (childItem != null) {
                    maxCloakTime += Inventory.GetBlockParameter("cloakTimeTotal", childItem.Tier, childItem.GetGrade());
                }
            }
        }

        maxCloakTime = Mathf.Clamp(maxCloakTime, 3f, 15f);
    }

    void Update() => CheckCloak();

    public void CheckCloak() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        if (cooldown < MAX_COOLDOWN && isPlayer) {
            cooldown += Time.deltaTime;
            UIController.Instance.UpdateCloakBar(cooldown, MAX_COOLDOWN);
        }

        if (cloakTimer < maxCloakTime && isInvisible) {
            cloakTimer += Time.deltaTime;
        } else if (cloakTimer >= maxCloakTime && isInvisible) {
            CloakShip(false);
        }

        float currentEnergy = EnergyStorage.CalculateTotalCurrentEnergy(transform.parent);

        if (((Input.GetKeyDown(KeyCode.Alpha5) && isPlayer) || !isPlayer) && cooldown >= MAX_COOLDOWN && currentEnergy >= ENERGY_COST) {
            EnergyStorage.RemoveEnergy(transform.parent, ENERGY_COST);
            CloakShip(true);
            cooldown = 0f;
            cloakTimer = 0f;
        }
    }

    private void CloakShip(bool toggle) {
        isInvisible = toggle;

        foreach (Transform child in transform.parent) {
            if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                if (isInvisible) {
                    itemBlock.SwapMaterial(false, new Material(Resources.Load<Material>("Materials/" + (Settings.Instance.InCreativeMode() ? "EnemyCloak" : "PlayerCloak"))));
                } else if (!isInvisible) {
                    itemBlock.SwapMaterial(true);
                }
            }
        }
    }
}
