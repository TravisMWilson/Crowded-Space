using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RepairModule : MonoBehaviour {
    private Item item;
    private float regenAmount = 1;
    private float regenRate = 1f;
    private float regenTimer = 0f;

    void Start() {
        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) regenAmount = Inventory.GetBlockParameter("healthRegenTotal", item.Tier, item.GetGrade());
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenRate) {
            RegenerateHealth();
            regenTimer = 0f;
        }
    }

    private void RegenerateHealth() {
        Transform shipTransform = transform.parent;
        float healthToDistribute = regenAmount * (regenRate * 60);

        foreach (Transform child in shipTransform) {
            if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                float availableSpace = itemBlock.MaxHealth() - itemBlock.Health;

                if (healthToDistribute <= availableSpace) {
                    itemBlock.RegenHealth(healthToDistribute);
                    break;
                } else {
                    itemBlock.RegenHealth(itemBlock.TotalMaxHealth);
                    healthToDistribute -= availableSpace;
                }
            }

            if (healthToDistribute <= 0) {
                break;
            }
        }
    }
}
