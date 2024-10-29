using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionCore : MonoBehaviour {
    private float regenAmount = 1;
    private float regenRate = 1f;
    private float regenTimer = 0f;

    private Item item;
    private GameObject playerShip;
    bool isPlayer = false;

    void Start() {
        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) regenAmount = Inventory.GetBlockParameter("energyRegenTotal", item.Tier, item.GetGrade());
        playerShip = GameObject.Find("PlayerShip");

        if (playerShip != null) {
            isPlayer = transform.parent.name == playerShip.name;
        }
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        regenTimer += Time.deltaTime;

        if (regenTimer >= regenRate) {
            DistributeEnergy();
            regenTimer = 0f;
        }
    }

    private void DistributeEnergy() {
        Transform shipTransform = transform.parent;
        float energyToDistribute = regenAmount * (regenRate * 60);

        foreach (Transform child in shipTransform) {
            if (child.TryGetComponent<EnergyStorage>(out var energyStorage)) {
                float availableSpace = energyStorage.MaxEnergyStorage - energyStorage.Energy;

                if (energyToDistribute <= availableSpace) {
                    energyStorage.Energy += energyToDistribute;
                    break;
                } else {
                    energyStorage.Energy = energyStorage.MaxEnergyStorage;
                    energyToDistribute -= availableSpace;
                }
            }

            if (energyToDistribute <= 0) {
                break;
            }
        }

        if (isPlayer) {
            UIController.Instance.UpdateEnergyBar(EnergyStorage.CalculateTotalCurrentEnergy(transform.parent), EnergyStorage.CalculateTotalMaxEnergy(transform.parent));
        }
    }
}
