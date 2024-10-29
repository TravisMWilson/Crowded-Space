using System.Text.RegularExpressions;
using UnityEngine;

public class EnergyStorage : MonoBehaviour {
    public float Energy { get; set; }
    public float MaxEnergyStorage { get; set; }
    public float TotalMaxEnergyStorage { get; set; }

    private Item item;

    void Start() {
        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) MaxEnergyStorage = Inventory.GetBlockParameter("energyTotal", item.Tier, item.GetGrade());
        if (transform.parent != null) TotalMaxEnergyStorage = CalculateTotalMaxEnergy(transform.parent);
    }

    public static float CalculateTotalCurrentEnergy(Transform parent) {
        float totalEnergy = 0;

        foreach (Transform child in parent) {
            if (child.TryGetComponent<EnergyStorage>(out var energyStorage)) {
                totalEnergy += energyStorage.Energy;
            }
        }

        return totalEnergy;
    }

    public static float CalculateTotalMaxEnergy(Transform parent) {
        float totalMaxEnergy = 0;

        foreach (Transform child in parent) {
            if (child.TryGetComponent<EnergyStorage>(out var energyStorage)) {
                totalMaxEnergy += energyStorage.MaxEnergyStorage;
            }
        }

        return totalMaxEnergy;
    }

    public static void RemoveEnergy(Transform parent, float energyDifference) {
        GameObject playerShip = GameObject.Find("PlayerShip");
        bool isPlayer = false;

        if (playerShip != null) {
            isPlayer = parent.name == playerShip.name;
        }

        foreach (Transform child in parent) {
            if (child.TryGetComponent<EnergyStorage>(out var energyStorage)) {
                float availableEnergy = energyStorage.Energy;

                if (energyDifference <= availableEnergy) {
                    energyStorage.Energy -= energyDifference;
                    if (isPlayer) UIController.Instance.UpdateEnergyBar(CalculateTotalCurrentEnergy(parent), energyStorage.TotalMaxEnergyStorage);
                    break;
                }
                else {
                    energyDifference -= availableEnergy;
                    energyStorage.Energy = 0;
                    if (isPlayer) UIController.Instance.UpdateEnergyBar(CalculateTotalCurrentEnergy(parent), energyStorage.TotalMaxEnergyStorage);
                }
            }

            if (energyDifference <= 0) {
                break;
            }
        }
    }
}