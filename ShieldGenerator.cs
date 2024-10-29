using UnityEngine;
using UnityEngine.SceneManagement;

public class ShieldGenerator : MonoBehaviour {
    public float MaxShield { get; set; }

    private GameObject shieldPrefab;
    private Item item;

    private float regenerationTimer = 0f;
    private float regenerationInterval = 1f;
    private float regenerationAmount = 1f;

    private float shieldCooldownTimer = 0f;
    private float shieldCooldownDuration = 10f;
    private bool isCooldownActive = false;
    private bool isPlayer = false;

    void Start() {
        isPlayer = transform.root.name.Contains("Player");
        shieldPrefab = Resources.Load<GameObject>("Blocks/Special/Shield");

        item = GetComponent<ItemBlock>().GetItem();

        if (item != null) {
            MaxShield = Inventory.GetBlockParameter("shieldTotal", item.Tier, item.GetGrade());
            regenerationAmount = Inventory.GetBlockParameter("shieldRegenTotal", item.Tier, item.GetGrade());
        }
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        Transform currentShield = transform.parent.Find("Shield(Clone)");

        if (isCooldownActive) {
            shieldCooldownTimer += Time.deltaTime;

            if (shieldCooldownTimer >= shieldCooldownDuration) {
                isCooldownActive = false;
                shieldCooldownTimer = 0f;
            }
        }

        if (currentShield == null && !isCooldownActive) {
            float currentEnergy = EnergyStorage.CalculateTotalCurrentEnergy(transform.parent);
            float maxEnergy = EnergyStorage.CalculateTotalMaxEnergy(transform.parent);

            if (currentEnergy >= maxEnergy) {
                EnergyStorage.RemoveEnergy(transform.parent, maxEnergy);

                Transform shipTransform = transform.parent;
                Bounds bounds = Utility.GetUnrotatedBounds(shipTransform);
                Bounds centerBound = Utility.GetTransformBounds(shipTransform);
                bounds.Expand(1f);

                float maxShieldHealth = CalculateMaxShieldHealth();

                GameObject shieldInstance = Instantiate(shieldPrefab, centerBound.center, Quaternion.identity, shipTransform);
                shieldInstance.transform.localScale = bounds.size;
                shieldInstance.transform.localRotation = Quaternion.identity;

                if (!isPlayer) {
                    shieldInstance.layer = 7;
                    shieldInstance.tag = "Enemy";
                }

                Shield shield = shieldInstance.GetComponent<Shield>();
                shield.Health = maxShieldHealth;
                shield.MaxHealth = maxShieldHealth;
                shield.Item = GetComponent<ItemBlock>().GetItem();

                isCooldownActive = true;
            }
        } else if (currentShield != null) {
            Shield shield = currentShield.GetComponent<Shield>();

            if (shield.Health < shield.MaxHealth) {
                regenerationTimer += Time.deltaTime;

                if (regenerationTimer >= regenerationInterval) {
                    float regenAmount = regenerationAmount * (regenerationInterval * 60);
                    shield.RegenShield(regenAmount);
                    regenerationTimer = 0f;
                }
            }

            if (shield.Health <= 0 && !isCooldownActive) {
                isCooldownActive = true;
            }
        }
    }

    private float CalculateMaxShieldHealth() {
        float maxHealth = 0;

        foreach (Transform child in transform.parent) {
            if (child.TryGetComponent<ShieldGenerator>(out var shieldGenerator)) {
                maxHealth += shieldGenerator.MaxShield;
            }
        }

        return maxHealth;
    }
}
