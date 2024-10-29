using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {
    public static Settings Instance;

    [SerializeField] private bool inDeleteMode = false;
    [SerializeField] private bool inCreativeMode = false;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject tierLabel;
    [SerializeField] private GameObject tierInput;

    private List<Item> removeItems = new List<Item>();

    private bool isSortAcending = true;
    private string sortedByLast;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void DeleteItemsFromInventory() {
        if (inDeleteMode && !inCreativeMode) {
            foreach (Item item in removeItems) {
                Inventory.Instance.Remove(item);
            }

            removeItems.Clear();
            Inventory.Instance.CurrentItems = Inventory.Instance.Items;
        }

        inDeleteMode = !inDeleteMode;
    }

    public void SortInventoryBy(string sortBy) {
        if (sortedByLast == sortBy) {
            isSortAcending = !isSortAcending;
        }

        if (!isSortAcending) {
            sortedByLast = sortBy;
            Inventory.Instance.SortInventory("Ascending " + sortBy);
        } else {
            sortedByLast = sortBy;
            Inventory.Instance.SortInventory("Descending " + sortBy);
        }
    }

    public void ChangeCreativeMode() => inCreativeMode = !inCreativeMode;

    public void SetupCreative() {
        Inventory.Instance.SetCurrentItems(inCreativeMode);
        ShipCreation.Instance.ShowInstansiationParent(inCreativeMode);
        tierLabel.SetActive(!inCreativeMode);
        tierInput.SetActive(inCreativeMode);
    }

    public bool InDeleteMode() => inDeleteMode;
    public bool InCreativeMode() => inCreativeMode;
    public void RemoveItem(Item item) => removeItems.Remove(item);
    public void AddItem(Item item) => removeItems.Add(item);

    public void ExitGame() {
        PopupManager.Instance.ShowPopup("Are you sure you want to exit the game?", PopupType.YesNo, () => {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });
    }
}
