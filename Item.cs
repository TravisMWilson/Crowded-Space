using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public enum ItemType {
    Bridge = 0,
    Hull = 1,
    Engine = 2,
    FusionCore = 3,
    EnergyStorage = 4,
    Weapon = 5,
    Special = 6
}

public enum WeaponType {
    Cannon = 0,
    Railgun = 1,
    MissileLauncher = 2
}

public enum SpecialType {
    Null = 0,
    TurboEngine = 1,
    RepairModule = 2,
    Magnet = 3,
    Laser = 4,
    Radar = 5,
    Repulsor = 6,
    ShieldGenerator = 7,
    QuantumDrive = 8,
    DroneBay = 9,
    CloakingDevice = 10,
    AdvancedBridge = 11
}

public class Item {
    public string Name { get; set; }
    public string Icon { get; set; }

    public ItemType Type { get; set; }
    public SpecialType SpecialType { get; set; }
    public long Tier { get; set; }

    public int Quantity { get; set; }
    public int QuantityInUse { get; set; }

    public GameObject Slot { get; set; }

    public Item(string name, ItemType type, string icon, long tier = 1, int quantity = 1, SpecialType specialType = 0) {
        Name = name;
        Icon = icon;

        Type = type;
        SpecialType = specialType;
        Tier = tier;

        Quantity = quantity;
    }

    public void ChangeQuantityUsed(int quantity) {
        QuantityInUse += quantity;
        UpdateQuantityUsed();
    }

    public void ResetQuantityUsed() {
        QuantityInUse = 0;
        UpdateQuantityUsed();
    }

    public int GetGrade() {
        if (Icon.StartsWith("Hull")) {
            Match match = Regex.Match(Icon, @"^Hull(\d+)");

            if (match.Success) {
                return int.Parse(match.Groups[1].Value);
            }
        } else {
            Match match = Regex.Match(Icon, @"\d+$");

            if (match.Success) {
                return int.Parse(match.Value);
            }
        }

        return 1;
    }

    private void UpdateQuantityUsed() {
        TextMeshProUGUI slotQuantity = Slot.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();
        slotQuantity.text = Quantity - QuantityInUse + "/" + Quantity;
    }
}
