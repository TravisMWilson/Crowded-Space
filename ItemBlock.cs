using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemBlock : MonoBehaviour {
    [SerializeField] private Item item;

    private Color USE_SELECTED = new Color(0, 0, 0, 0);

    public float Health { get; set; }
    public float TotalMaxHealth { get; set; }

    private string saveMaterialName;

    private Material originalMaterial;
    private List<Material> originalMaterials = new List<Material>();
    private List<Transform> childrenWithRenderers = new List<Transform>();

    private GameObject smallExplosionFX;
    private GameObject pickup;

    void Start() {
        smallExplosionFX = Resources.Load<GameObject>("FX/FX_Missiles_Explosion");
        pickup = Resources.Load<GameObject>("Pickup");

        Renderer renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;

        Utility.ApplyToAllChildren(transform, (Transform child) => {
            if (child.TryGetComponent<Renderer>(out var renderer)) {
                if (!renderer.name.Contains("FX") && !renderer.material.name.Contains("Invisible")) {
                    originalMaterials.Add(renderer.material);
                    childrenWithRenderers.Add(child);
                }
            }
        });

        if (item != null) Health = MaxHealth();

        try {
            if (transform.parent != null) {
                TotalMaxHealth = CalculateTotalMaxHealth(transform.parent);
            }
        } catch (NullReferenceException) {
        }
    }

    void OnMouseDown() {
        if (gameObject.layer == 5 && SceneManager.GetActiveScene().name == "ShipCreation") {
            if (Settings.Instance.InDeleteMode()) {
                ToggleDelete();
            } else {
                HandleUIInteraction();
            }
        }
    }

    public static GameObject FindItemBlockObject(Transform childTransform) {
        if (childTransform == null) return null;

        ItemBlock itemBlock = childTransform.GetComponent<ItemBlock>();

        return itemBlock != null ? childTransform.gameObject : FindItemBlockObject(childTransform.parent);
    }

    public static float CalculateTotalCurrentHealth(Transform parent) {
        float totalHealth = 0f;

        foreach (Transform child in parent) {
            if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                totalHealth += itemBlock.Health;
            }
        }

        return totalHealth;
    }

    public static float CalculateTotalMaxHealth(Transform parent) {
        float totalMaxHealth = 0f;

        foreach (Transform child in parent) {
            if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                totalMaxHealth += itemBlock.MaxHealth();
            }
        }

        return totalMaxHealth;
    }

    public float MaxHealth() {
        float maxHealth = item.Tier * 50;

        if (item.Name.Contains("Hull")) {
            maxHealth *= item.GetGrade();
        }

        return maxHealth;
    }

    public void SetItem(Item i) => item = i;
    public Item GetItem() => item;

    public void SwapMaterial(bool restoreMaterial = true, Material newMaterial = null) {
        Renderer renderer = GetComponent<Renderer>();
        if (!renderer.material.name.Contains("Invisible")) renderer.material = restoreMaterial ? originalMaterial : newMaterial;

        for (int i = 0; i < childrenWithRenderers.Count; i++) {
            if (childrenWithRenderers[i].TryGetComponent<Renderer>(out var childRenderer)) {
                childRenderer.material = restoreMaterial ? originalMaterials[i] : newMaterial;
            }
        }
    }

    public void ToggleDelete() {
        if (Settings.Instance.InDeleteMode()) {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer.material.name == "Deleting") {
                RemoveItemFromRemovalList();
            } else {
                AddItemToRemove();
            }
        }
    }

    public Color GetColor() {
        if (transform.childCount == 0) {
            return GetComponent<Renderer>().material.color;
        } else {
            foreach (Transform block in transform) {
                if (block.TryGetComponent<Renderer>(out var renderer)) {
                    if (block.name.Contains("Hull")) {
                        return renderer.material.color;
                    }
                }
            }
        }

        return originalMaterial.color;
    }

    public void RegenHealth(float regenAmount) {
        ApplyDamage(-regenAmount);

        if (item != null) {
            if (Health > MaxHealth()) Health = MaxHealth();
        }
    }

    public void ApplyDamage(float damage) {
        bool isPlayer = transform.root.name.Contains("Player");

        Health -= isPlayer ? (damage / 7) : damage;

        if (isPlayer) {
            UIController.Instance.UpdateHealthBar(CalculateTotalCurrentHealth(transform.parent), TotalMaxHealth);
        }

        if (Health <= 0) {
            if (!isPlayer) {
                if (UnityEngine.Random.Range(0, 100) < 5) {
                    GameObject newPickup = Instantiate(pickup, transform.position, Quaternion.identity);
                    newPickup.GetComponent<Pickup>().SetItem(item);
                }
            }

            TriggerSmallExplosion();
            Destroy(gameObject);
        }
    }

    private void TriggerSmallExplosion() {
        GameObject explosion = Instantiate(smallExplosionFX, transform.position, Quaternion.identity);
        explosion.transform.localScale = transform.localScale;
        explosion.GetComponent<ParticleSystem>().Play();
        SoundManager.Instance.PlaySound("Explosion");
        Destroy(explosion, 5f);
    }

    private void HandleUIInteraction() {
        UpdateDisplayIcon();
        UpdateDisplayItem();
        Utility.Instance.Delay(0.01f, () => UIController.Instance.UpdateDisplayInformation());
    }

    private void UpdateDisplayItem() {
        GameObject selectButton = GameObject.Find("SelectButton");
        if (selectButton == null) return;
        selectButton.GetComponent<SelectButton>().SetDisplayItem(item);
    }

    private void UpdateDisplayIcon() {
        GameObject display = SceneManager.GetActiveScene().name == "MainHub"
            ? GameObject.Find("ShopInventory").transform.Find("Display").gameObject
            : GameObject.Find("Inventory").transform.Find("Display").gameObject;

        if (display == null) return;

        Transform displayItem = display.transform.Find("Item Parent");
        Transform displayIcon = displayItem.transform.Find("DisplayItemIcon");

        Utility.ClearChildObjects(displayIcon);

        GameObject icon = Instantiate(Resources.Load<GameObject>("Blocks/" + item.Type + "/" + item.Icon), displayIcon);
        icon.layer = 5;
        icon.GetComponent<Renderer>().receiveShadows = false;
        icon.GetComponent<ItemBlock>().SetItem(item);

        Utility.ApplyToAllChildren(icon.transform, (Transform child) => {
            child.gameObject.layer = 5;
            if (child.GetComponent<Renderer>() != null) child.GetComponent<Renderer>().receiveShadows = false;
        });

        ColorPicker.ApplyColorToBlock(icon.transform, USE_SELECTED);
    }

    private void RemoveItemFromRemovalList() {
        TintBlock(Color.white);
        Settings.Instance.RemoveItem(item);
    }

    private void AddItemToRemove() {
        Settings.Instance.AddItem(item);
        TintBlock(Color.red, true);
    }

    private void TintBlock(Color color, bool replaceName = false) {
        Renderer renderer = GetComponent<Renderer>();

        renderer.material = new Material(renderer.material);
        color.a = renderer.material.color.a;
        renderer.material.color = color;
        originalMaterial.color = color;

        if (replaceName) {
            saveMaterialName = renderer.material.name;
            renderer.material.name = "Deleting";
        } else {
            renderer.material.name = saveMaterialName;
        }

        Utility.ApplyToAllChildren(transform, (Transform child) => {
            if (child.TryGetComponent<Renderer>(out var renderer)) {
                renderer.material = new Material(renderer.material);
                color.a = renderer.material.color.a;
                renderer.material.color = color;
            }
        });
    }
}
