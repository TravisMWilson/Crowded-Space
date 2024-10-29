using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public class Inventory : MonoBehaviour {
    public static Dictionary<string, Func<float, float, float>> BlockParameters = new Dictionary<string, Func<float, float, float>> {
        { "healthTotal", (tier, blockGrade) => tier * 50f * blockGrade },
        { "maxSizeTotal", (tier, blockGrade) => tier * 5f * blockGrade },
        { "dpsCannonTotal", (tier, blockGrade) => tier * 8f },
        { "dpsMissileTotal", (tier, blockGrade) => tier * 4f },
        { "dpsRailgunTotal", (tier, blockGrade) => tier * 12f },
        { "speedTotal", (tier, blockGrade) => tier * 1.1f },
        { "energyRegenTotal", (tier, blockGrade) => tier * 0.1f * ((blockGrade / 2f) + 1f) },
        { "energyTotal", (tier, blockGrade) => tier * 50f * blockGrade },
        { "dpsLaserTotal", (tier, blockGrade) => tier * 16f },
        { "pushDistTotal", (tier, blockGrade) => tier * 10f },
        { "dpsMinigunTotal", (tier, blockGrade) => tier * 8f },
        { "maxSpecialsTotal", (tier, blockGrade) => 2f * blockGrade },
        { "cloakTimeTotal", (tier, blockGrade) => tier * 0.2f * blockGrade },
        { "pickupDistTotal", (tier, blockGrade) => tier * 2f * blockGrade },
        { "teleportDistTotal", (tier, blockGrade) => tier * 10f },
        { "radarDistTotal", (tier, blockGrade) => tier * 10f * blockGrade },
        { "healthRegenTotal", (tier, blockGrade) => tier * 5f * ((blockGrade / 10f) + 1f) },
        { "shieldTotal", (tier, blockGrade) => tier * 200f * blockGrade },
        { "shieldRegenTotal", (tier, blockGrade) => tier * 0.5f * ((blockGrade / 10f) + 1f) },
        { "jumpSpeedTotal", (tier, blockGrade) => blockGrade * (tier / 200f) }
    };

    public static Dictionary<string, Func<long, int, long>> BlockPowers = new Dictionary<string, Func<long, int, long>> {
        { "Hull", (tier, blockGrade) => tier * 5 * blockGrade },
        { "Rock", (tier, blockGrade) => tier * 3 },
        { "Bridge", (tier, blockGrade) => tier * 10 * blockGrade },
        { "Cannon", (tier, blockGrade) => tier * 170 },
        { "Missile Launcher", (tier, blockGrade) => tier * 210 },
        { "Railgun", (tier, blockGrade) => tier * 150 },
        { "Engine", (tier, blockGrade) => tier * 15 },
        { "Fusion Core", (tier, blockGrade) => tier * 50 * blockGrade },
        { "Energy Storage", (tier, blockGrade) => tier * 25 * blockGrade },
        { "Laser", (tier, blockGrade) => tier * 350 },
        { "Repulsor", (tier, blockGrade) => tier * 200 },
        { "Minigun", (tier, blockGrade) => tier * 120 },
        { "Advanced Bridge", (tier, blockGrade) => tier * 70 * blockGrade },
        { "Cloaking Device", (tier, blockGrade) => tier * 60 * blockGrade },
        { "Magnet", (tier, blockGrade) => tier * 50 * blockGrade },
        { "Quantum Drive", (tier, blockGrade) => tier * 60 },
        { "Radar", (tier, blockGrade) => tier * 40 * blockGrade },
        { "Repair Module", (tier, blockGrade) => tier * 30 * blockGrade },
        { "Shield Generator", (tier, blockGrade) => tier * 180 * blockGrade },
        { "Turbo Engine", (tier, blockGrade) => tier * 50 * blockGrade }
    };

    public static Dictionary<string, Func<long, int, float>> BlockEnergyCosts = new Dictionary<string, Func<long, int, float>> {
        { "Cannon", (tier, blockGrade) => tier * 0.1f },
        { "Missile Launcher", (tier, blockGrade) => tier * 0.1f },
        { "Railgun", (tier, blockGrade) => tier * 0.1f },
        { "Engine", (tier, blockGrade) => tier * 0.1f },
        { "Laser", (tier, blockGrade) => tier * 1.5f },
        { "Repulsor", (tier, blockGrade) => tier * 4f },
        { "Minigun", (tier, blockGrade) => tier * 0.1f },
        { "Cloaking Device", (tier, blockGrade) => tier * 0.5f * blockGrade },
        { "Magnet", (tier, blockGrade) => tier * 0.1f * blockGrade },
        { "Quantum Drive", (tier, blockGrade) => tier * 3f },
        { "Radar", (tier, blockGrade) => tier * 3f * blockGrade },
        { "Repair Module", (tier, blockGrade) => tier * 0.1f * blockGrade },
        { "Shield Generator", (tier, blockGrade) => tier * 0.1f * blockGrade }
    };

    public static Inventory Instance;

    public List<Item> Items = new List<Item>();
    public List<Item> CreativeItems = new List<Item>();
    public List<Item> CurrentItems = new List<Item>();

    [SerializeField] private GameObject updateBridgeOption;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform bridgeParent;
    [SerializeField] private Transform hullParent;
    [SerializeField] private Transform engineParent;
    [SerializeField] private Transform fusionCoreParent;
    [SerializeField] private Transform energyStorageParent;
    [SerializeField] private Transform weaponParent;
    [SerializeField] private Transform specialParent;

    private Transform inventoryContainer;
    private Item selectedItem = null;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        string strInventory = Load.String("Inventory");
        string strSelectedItem = Load.String("SelectedItem");

        Item item = new Item("Bridge", ItemType.Bridge, "Bridge1");
        _ = Add(item);

        string[] hullVariants = { "Block", "Pipe", "PipeCorner", "Triangle", "Triangle2Corner", "Triangle3Corner", "TriangleEnd2", "TriangleEnd3" };

        for (int i = 1; i < hullVariants.Count(); i++) {
            foreach (var variant in hullVariants) {
                item = new Item("Hull", ItemType.Hull, "Hull" + i + "_" + variant);
                _ = Add(item);
            }
        }

        AddItems("Cannon", ItemType.Weapon, "Weapon_Cannon", 6);
        AddItems("Missile Launcher", ItemType.Weapon, "Weapon_MissileLauncher", 6);
        AddItems("Railgun", ItemType.Weapon, "Weapon_Railgun", 6);
        AddItems("Engine", ItemType.Engine, "Engine", 23);
        AddItems("Fusion Core", ItemType.FusionCore, "FusionCore", 11);
        AddItems("Energy Storage", ItemType.EnergyStorage, "EnergyStorage", 5);
        AddItems("Laser", ItemType.Special, "Special_Laser", 2, SpecialType.Laser);
        AddItems("Repulsor", ItemType.Special, "Special_Repulsor", 2, SpecialType.Repulsor);

        item = new Item("Rock", ItemType.Hull, "Rock");
        _ = Add(item);
        item = new Item("Minigun", ItemType.Weapon, "Weapon_Minigun1");
        _ = Add(item);
        item = new Item("Advanced Bridge", ItemType.Special, "Special_AdvancedBridge1", 1, 1, SpecialType.AdvancedBridge);
        _ = Add(item);
        item = new Item("Cloaking Device", ItemType.Special, "Special_CloakingDevice1", 1, 1, SpecialType.CloakingDevice);
        _ = Add(item);
        item = new Item("Magnet", ItemType.Special, "Special_Magnet1", 1, 1, SpecialType.Magnet);
        _ = Add(item);
        item = new Item("Quantum Drive", ItemType.Special, "Special_QuantumDrive1", 1, 1, SpecialType.QuantumDrive);
        _ = Add(item);
        item = new Item("Radar", ItemType.Special, "Special_Radar1", 1, 1, SpecialType.Radar);
        _ = Add(item);
        item = new Item("Repair Module", ItemType.Special, "Special_RepairModule1", 1, 1, SpecialType.RepairModule);
        _ = Add(item);
        item = new Item("Shield Generator", ItemType.Special, "Special_ShieldGenerator1", 1, 1, SpecialType.ShieldGenerator);
        _ = Add(item);
        item = new Item("Turbo Engine", ItemType.Special, "Special_TurboEngine1", 1, 1, SpecialType.TurboEngine);
        _ = Add(item);

        foreach (Item i in Items) {
            CreativeItems.Add(i);
        }

        Items.Clear();

        foreach (Item i in CreativeItems) {
            i.Slot.SetActive(false);
        }

        LoadItemsFromString(strInventory);

        if (!string.IsNullOrEmpty(strSelectedItem)) {
            Item selectedItem = DeserializeItem(strSelectedItem);
            selectedItem = Items.Find(item => item.Name == selectedItem.Name && item.Type == selectedItem.Type && item.Tier == selectedItem.Tier && item.Icon == selectedItem.Icon);
        }

        CurrentItems = Items;

        if (Items.Count == 0) {
            item = new Item("Hull", ItemType.Hull, "Hull1_Block", 1, 5);
            _ = Add(item);
            item = new Item("Minigun", ItemType.Weapon, "Weapon_Minigun1");
            _ = Add(item);
            item = new Item("Engine", ItemType.Engine, "Engine1");
            _ = Add(item);
            item = new Item("Fusion Core", ItemType.FusionCore, "FusionCore1");
            _ = Add(item);
            item = new Item("Energy Storage", ItemType.EnergyStorage, "EnergyStorage1");
            _ = Add(item);
        }
    }

    //void Update() {
    //    if (Input.GetKeyUp(KeyCode.G)) {
    //        foreach (Item i in CreativeItems) {
    //            _ = Add(i, 1);
    //        }
    //    }
    //}

    void OnDestroy() => SaveItemsToString();

    public static float GetBlockParameter(string key, long tier, int blockGrade) {
        return BlockParameters.TryGetValue(key, out var func)
            ? func(tier, blockGrade)
            : throw new ArgumentException($"No parameter found with the key '{key}'.");
    }

    public static long GetBlockPower(string key, long tier, int blockGrade) {
        return BlockPowers.TryGetValue(key, out var func)
            ? func(tier, blockGrade)
            : throw new ArgumentException($"No power found with the key '{key}'.");
    }

    public static float GetBlockEnergyCost(string key, long tier, int blockGrade) {
        return BlockEnergyCosts.TryGetValue(key, out var func)
            ? func(tier, blockGrade)
            : throw new ArgumentException($"No energy cost found with the key '{key}'.");
    }

    public GameObject GetSelectedItemBlock() => selectedItem != null ? Resources.Load<GameObject>("Blocks/" + selectedItem.Type + "/" + selectedItem.Icon) : null;
    public Item GetSelectedItem() => selectedItem;

    public void ResetItemsUsed() {
        foreach (Item item in CurrentItems) {
            item.ResetQuantityUsed();
        }
    }

    public void SetCurrentItems(bool inCreativeMode) {
        foreach (Item item in CurrentItems) {
            item.Slot.SetActive(false);
        }

        CurrentItems = inCreativeMode ? CreativeItems : Items;

        foreach (Item item in CurrentItems) {
            item.Slot.SetActive(true);
        }
    }

    public void ItemBlock(Item item) {
        if (item == null) return;
        selectedItem = item;

        GameObject selectedItemIcon = GameObject.Find("SelectedItemIcon");
        GameObject selectedItemText = GameObject.Find("SelectedItemText");

        Utility.ClearChildObjects(selectedItemIcon.transform);

        GameObject selectedIcon = Instantiate(GetSelectedItemBlock(), selectedItemIcon.transform);
        selectedIcon.layer = 5;

        Utility.ApplyToAllChildren(selectedIcon.transform, (Transform child) => child.gameObject.layer = 5);

        selectedItemText.GetComponent<TextMeshProUGUI>().text = item.Name;
    }

    public bool Add(Item item, int overrideQuantity = 0) {
        SetInventoryContainer(item.Type);

        if (TryAddExistingItem(item, overrideQuantity)) return true;

        AddNewItemToInventory(item, false, overrideQuantity);

        return true;
    }

    public void Remove(Item item) {
        Item inventoryItem = Items.FirstOrDefault(it => it.Name == item.Name && it.Type == item.Type && it.Tier == item.Tier && it.Icon == item.Icon);
        if (inventoryItem == null) return;

        inventoryItem.Quantity -= item.Quantity;

        TextMeshProUGUI slotQuantity = inventoryItem.Slot.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();
        slotQuantity.text = inventoryItem.Quantity - inventoryItem.QuantityInUse + "/" + inventoryItem.Quantity.ToString();

        if (inventoryItem.Quantity <= 0) {
            Destroy(inventoryItem.Slot);
            _ = Items.Remove(inventoryItem);
        }
    }

    public void ShowTab(GameObject tab) {
        bridgeParent.gameObject.SetActive(false);
        hullParent.gameObject.SetActive(false);
        engineParent.gameObject.SetActive(false);
        fusionCoreParent.gameObject.SetActive(false);
        energyStorageParent.gameObject.SetActive(false);
        weaponParent.gameObject.SetActive(false);
        specialParent.gameObject.SetActive(false);

        tab.SetActive(true);

        GameObject.Find("Scroll View").GetComponent<ScrollRect>().content = tab.GetComponent<RectTransform>();

        if (updateBridgeOption != null) updateBridgeOption.SetActive(hullParent.gameObject.activeSelf);
    }

    public void SortInventory(string sortBy) {
        var sortCriteria = new Dictionary<string, Comparison<Item>> {
            { "Ascending Name", (a, b) => a.Name.CompareTo(b.Name) },
            { "Descending Name", (a, b) => b.Name.CompareTo(a.Name) },
            { "Ascending Tier", (a, b) => a.Tier.CompareTo(b.Tier) },
            { "Descending Tier", (a, b) => b.Tier.CompareTo(a.Tier) },
            { "Ascending Quantity", (a, b) => a.Quantity.CompareTo(b.Quantity) },
            { "Descending Quantity", (a, b) => b.Quantity.CompareTo(a.Quantity) }
        };

        if (sortCriteria.TryGetValue(sortBy, out Comparison<Item> comparison)) {
            CurrentItems.Sort((a, b) => comparison(a, b));

            for (int i = 0; i < CurrentItems.Count; i++) {
                CurrentItems[i].Slot.GetComponent<RectTransform>().SetSiblingIndex(i);
            }
        } else {
            Debug.LogWarning("Sort criteria not found: " + sortBy);
        }
    }

    private void AddItems(string name, ItemType type, string fileName, int count, SpecialType sType = SpecialType.Null) {
        Item item;

        for (int i = 1; i <= count; i++) {
            item = new Item(name, type, fileName + i, 1, 1, sType);
            _ = Add(item);
        }
    }

    private bool TryAddExistingItem(Item item, int overrideQuantity = 0) {
        Item inventoryItem = Items.Find(it => it.Name == item.Name && it.Type == item.Type && it.Tier == item.Tier && it.Icon == item.Icon);
        if (inventoryItem == null) return false;

        if (overrideQuantity == 0) {
            inventoryItem.Quantity += item.Quantity;
        } else {
            inventoryItem.Quantity += overrideQuantity;
        }

        if (inventoryItem.Slot != null) {
            TextMeshProUGUI slotQuantity = inventoryItem.Slot.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();
            slotQuantity.text = inventoryItem.Quantity - inventoryItem.QuantityInUse + "/" + inventoryItem.Quantity;
        }

        return true;
    }

    private void AddNewItemToInventory(Item item, bool slotOnly = false, int overrideQuantity = 0) {
        GameObject slot;
        Transform slotIcon;
        TextMeshProUGUI slotQuantity;
        TextMeshProUGUI slotTier;
        
        slot = Instantiate(slotPrefab, inventoryContainer);

        slotIcon = slot.transform.Find("ItemIcon");
        GameObject itemIcon = Instantiate(Resources.Load<GameObject>("Blocks/" + item.Type + "/" + item.Icon), slotIcon);
        itemIcon.layer = 5;
        itemIcon.GetComponent<Renderer>().receiveShadows = false;

        Utility.ApplyToAllChildren(itemIcon.transform, (Transform child) => {
            child.gameObject.layer = 5;
            if (child.GetComponent<Renderer>() != null) child.GetComponent<Renderer>().receiveShadows = false;
        });

        itemIcon.GetComponent<ItemBlock>().SetItem(item);

        slotTier = slot.transform.Find("ItemTier").gameObject.GetComponent<TextMeshProUGUI>();
        slotQuantity = slot.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();

        slotTier.text = item.Tier.ToString();

        if (overrideQuantity != 0) {
            item.Quantity = overrideQuantity;
        }

        slotQuantity.text = item.Quantity - item.QuantityInUse + "/" + item.Quantity;
        item.Slot = slot;

        if (slotOnly) return;

        Items.Add(item);
    }

    private void LoadItemsFromString(string itemsString) {
        while (!string.IsNullOrEmpty(itemsString)) {
            int index = itemsString.IndexOf(",");
            string nextItem = itemsString[..index];
            itemsString = itemsString[(index + 1)..];

            Item newItem = DeserializeItem(nextItem);
            _ = Add(newItem);
        }
    }

    private Item DeserializeItem(string itemString) {
        string[] itemInfo = itemString.Split(':', System.StringSplitOptions.RemoveEmptyEntries);

        string itemName = itemInfo[0];
        ItemType itemType = Enum.Parse<ItemType>(itemInfo[1]);
        string itemIcon = itemInfo[2];
        long itemTier = long.Parse(itemInfo[3]);
        int itemQuantity = int.Parse(itemInfo[4]);

        return new Item(itemName, itemType, itemIcon, itemTier, itemQuantity);
    }

    private void SaveItemsToString() {
        string save = string.Empty;

        foreach (Item item in Items) {
            save += item.Name + ":" + item.Type + ":" + item.Icon + ":" + item.Tier + ":" + item.Quantity + ",";
        }

        Save.String("Inventory", save);

        if (selectedItem != null) {
            string serializedItem = selectedItem.Name + ":" + selectedItem.Type + ":" + selectedItem.Icon + ":" + selectedItem.Tier + ":" + selectedItem.Quantity;
            Save.String("SelectedItem", serializedItem);
        } else {
            Save.String("SelectedItem", string.Empty);
        }
    }

    private void SetInventoryContainer(ItemType itemType) {
        inventoryContainer = itemType == ItemType.Bridge ? bridgeParent :
        itemType == ItemType.Hull ? hullParent :
            itemType == ItemType.Engine ? engineParent :
            itemType == ItemType.FusionCore ? fusionCoreParent :
            itemType == ItemType.EnergyStorage ? energyStorageParent :
            itemType == ItemType.Weapon ? weaponParent : specialParent;
    }
}