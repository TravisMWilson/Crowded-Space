using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipCreation : MonoBehaviour {
    public static ShipCreation Instance;

    private const float GRID_SIZE = 1f;
    private Color USE_SELECTED = new Color(0, 0, 0, 0);

    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private Material hologramMaterial;
    [SerializeField] private GameObject shipRequirements;
    [SerializeField] private TextMeshProUGUI shipSizeText;
    [SerializeField] private Material green;
    [SerializeField] private Material red;

    private GameObject currentHologram;
    private Transform creationParent;
    private Transform creativeParent;

    private int buildMode = 0;
    private bool placingBlock = false;
    private bool meetsRequirements = false;
    private long maxShipSize = 25;
    private long maxSpecials = 1;
    private long specialCount = 0;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update() {
        if (SceneManager.GetActiveScene().name != "ShipCreation") return;

        if (InventoryAnimationController.IsInventoryOpen() || SettingAnimationController.IsSettingsOpen() || BehaviorButton.IsBehaviorsOpen()) return;

        if (Input.GetMouseButtonDown(0)) {
            PlaceOrModifyBlock();
        }

        HandleHologram();

        if (Input.GetKeyDown(KeyCode.P)) {
            ChangeBuildMode(0);
        } else if (Input.GetKeyDown(KeyCode.X)) {
            ChangeBuildMode(1);
        } else if (Input.GetKeyDown(KeyCode.R)) {
            ChangeBuildMode(2);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "ShipCreation") {
            Settings.Instance.SetupCreative();
        }
    }

    public void ChangeBuildMode(int mode) => buildMode = mode;
    public bool DoesShipMeetRequirements() => meetsRequirements;
    public Transform GetCreationParent() => creationParent;
    public Transform GetCreativeParent() => creativeParent;

    public void ShowInstansiationParent(bool inCreativeMode) {
        if (GameObject.Find("Creation") != null) creationParent = GameObject.Find("Creation").transform;
        if (GameObject.Find("CreativeCreation") != null) creativeParent = GameObject.Find("CreativeCreation").transform;

        creationParent.gameObject.SetActive(!inCreativeMode);
        creativeParent.gameObject.SetActive(inCreativeMode);
        shipRequirements.SetActive(!inCreativeMode);

        UpdateMaxShipSize();
        CheckShipRequirements();
    }

    private void PlaceOrModifyBlock() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, blockLayer)) {
            if ((ignoreLayer.value & (1 << hit.transform.gameObject.layer)) == 0) {
                GameObject blockWithProperties = hit.transform.GetComponent<BlockProperties>() != null
                    ? hit.transform.gameObject
                    : hit.transform.GetComponentInParent<BlockProperties>().gameObject;

                if (blockWithProperties != null) {
                    if (Inventory.Instance.GetSelectedItemBlock() == null) {
                        PopupManager.Instance.ShowPopup("You don't have a block selected yet. Open your inventory by pressing the four square icon on the menu at the top of the screen. " +
                            "Then select a block from your inventory and press the pulsing button below the big display.", PopupType.OK);
                        return;
                    }

                    Vector3 normal = hit.normal;
                    Vector3 newPosition = blockWithProperties.transform.position + (normal * GRID_SIZE);

                    newPosition.x = Mathf.Round(newPosition.x / GRID_SIZE) * GRID_SIZE;
                    newPosition.y = Mathf.Round(newPosition.y / GRID_SIZE) * GRID_SIZE;
                    newPosition.z = Mathf.Round(newPosition.z / GRID_SIZE) * GRID_SIZE;

                    if (buildMode == 0) {
                        BlockProperties blockProperties = blockWithProperties.GetComponent<BlockProperties>();

                        if (CanPlaceBlockOnSide(hit.normal, blockProperties) && !placingBlock) {
                            if (!Settings.Instance.InCreativeMode()) {
                                Item item = Inventory.Instance.GetSelectedItem();

                                if (item.QuantityInUse < item.Quantity) {
                                    if ((item.Type == ItemType.Special && specialCount != maxSpecials) || item.Type != ItemType.Special) {
                                        item.ChangeQuantityUsed(1);
                                        PlaceBlockWithAnimation(newPosition, normal, hit.point, blockProperties.transform);
                                    } else {
                                        PopupManager.Instance.ShowPopup("You have already placed all the specials you can. Place advanced bridges to place more specials.", PopupType.OK);
                                    }
                                } else {
                                    PopupManager.Instance.ShowPopup("You've already placed all " + item.Quantity + " of your " + item.Name, PopupType.OK);
                                }
                            } else {
                                PlaceBlockWithAnimation(newPosition, normal, hit.point, blockProperties.transform);
                            }
                        }
                    } else if (buildMode == 1) {
                        if (blockWithProperties.transform.localPosition != Vector3.zero) {
                            if (!Settings.Instance.InCreativeMode()) {
                                ItemBlock blockItemBlock = blockWithProperties.GetComponent<ItemBlock>();
                                Item blockItem = blockItemBlock.GetItem();
                                Item inventoryItem = Inventory.Instance.Items.Find(it => it.Name == blockItem.Name && it.Type == blockItem.Type && it.Tier == blockItem.Tier && it.Icon == blockItem.Icon);

                                inventoryItem.ChangeQuantityUsed(-1);
                                blockItemBlock.SetItem(null);
                                blockWithProperties.transform.SetParent(null);
                            }

                            DeleteBlockWithAnimation(blockWithProperties);
                        } else if (Settings.Instance.InCreativeMode()) {
                            PlaceBlockWithAnimation(Vector3.zero, Vector3.zero, Vector3.zero, blockWithProperties.transform);
                            DeleteBlockWithAnimation(blockWithProperties);
                        }
                    } else if (buildMode == 2 && blockWithProperties.name != "Bridge") {
                        RotateBlock(blockWithProperties.transform, hit.normal);
                    }

                    CheckShipRequirements();
                    UpdateMaxShipSize();
                }
            }
        }
    }

    private void UpdateMaxShipSize() {
        if (shipRequirements == null || !shipRequirements.activeSelf) return;

        maxSpecials = 1;
        maxShipSize = 20;
        specialCount = 0;

        foreach (Transform block in creationParent) {
            if (block.name.Contains("AdvancedBridge")) {
                maxSpecials += 2;
                specialCount++;
            } else if (block.name.Contains("Bridge")) {
                Item blockItem = block.gameObject.GetComponent<ItemBlock>().GetItem();
                long tier = 1;

                if (blockItem != null) tier = blockItem.Tier;

                maxShipSize += tier * 5;
            } else {
                if (block.gameObject.TryGetComponent<ItemBlock>(out var itemBlock)) {
                    Item blockItem = itemBlock.GetItem();
                    if (blockItem != null) {
                        if (blockItem.Type == ItemType.Special) specialCount++;
                    }
                }
            }
        }

        shipSizeText.text = "Specials\n" + specialCount + "/" + maxSpecials + "\nShip Size\n" + creationParent.childCount + "/" + maxShipSize;
    }

    private void CheckShipRequirements() {
        if (shipRequirements == null || !shipRequirements.activeSelf) return;

        Transform requirements = shipRequirements.transform.Find("Requirements");
        Transform bridge = requirements.Find("Bridge");
        Transform engine = requirements.Find("Engine");
        Transform fusionCore = requirements.Find("FusionCore");
        Transform energyStorage = requirements.Find("EnergyStorage");

        UpdateRequirement(bridge, false);
        UpdateRequirement(engine, false);
        UpdateRequirement(fusionCore, false);
        UpdateRequirement(energyStorage, false);

        meetsRequirements = false;

        bool hasBridge = false;
        bool hasEngine = false;
        bool hasFusionCore = false;
        bool hasEnergyStorage = false;

        foreach (Transform block in creationParent) {
            if (block.name.Contains("Bridge")) {
                UpdateRequirement(bridge, true);
                hasBridge = true;
            } else if (block.name.Contains("Engine")) {
                UpdateRequirement(engine, true);
                hasEngine = true;
            } else if (block.name.Contains("FusionCore")) {
                UpdateRequirement(fusionCore, true);
                hasFusionCore = true;
            } else if (block.name.Contains("EnergyStorage")) {
                UpdateRequirement(energyStorage, true);
                hasEnergyStorage = true;
            }
        }

        if (hasBridge && hasEngine && hasFusionCore && hasEnergyStorage) meetsRequirements = true;
    }

    private void UpdateRequirement(Transform requirement, bool isMet) {
        foreach (Transform child in requirement) {
            child.gameObject.GetComponent<Renderer>().material = isMet ? green : red;
        }
    }

    private void HandleHologram() {
        if (Inventory.Instance.GetSelectedItemBlock() != null && !placingBlock && buildMode == 0) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, blockLayer)) {
                if ((ignoreLayer.value & (1 << hit.transform.gameObject.layer)) == 0) {
                    GameObject blockWithProperties = hit.transform.GetComponent<BlockProperties>() != null
                        ? hit.transform.gameObject
                        : hit.transform.GetComponentInParent<BlockProperties>().gameObject;

                    BlockProperties blockProperties = blockWithProperties.GetComponent<BlockProperties>();

                    Vector3 normal = hit.normal;
                    Vector3 hologramPosition = hit.transform.position + (normal * GRID_SIZE);

                    hologramPosition.x = Mathf.Round(hologramPosition.x / GRID_SIZE) * GRID_SIZE;
                    hologramPosition.y = Mathf.Round(hologramPosition.y / GRID_SIZE) * GRID_SIZE;
                    hologramPosition.z = Mathf.Round(hologramPosition.z / GRID_SIZE) * GRID_SIZE;

                    if (blockProperties != null && CanPlaceBlockOnSide(hit.normal, blockProperties)) {
                        if (creationParent.childCount >= maxShipSize && !Settings.Instance.InCreativeMode()) return;

                        if (currentHologram == null) {
                            currentHologram = Instantiate(Inventory.Instance.GetSelectedItemBlock(), hologramPosition, Quaternion.identity);
                            currentHologram.layer = 2;

                            Utility.ApplyToAllChildren(currentHologram.transform, (Transform child) => child.gameObject.layer = 2);

                            Renderer[] childRenderers = currentHologram.GetComponentsInChildren<Renderer>();
                            foreach (Renderer renderer in childRenderers) {
                                renderer.material = hologramMaterial;
                            }
                        } else {
                            currentHologram.transform.position = hologramPosition;
                        }

                        PlaceBlockWithCorrectRotation(currentHologram.transform, normal, hit.point, blockWithProperties.transform);
                    } else if (currentHologram != null) {
                        Destroy(currentHologram);
                    }
                } else if (currentHologram != null) {
                    Destroy(currentHologram);
                }
            } else if (currentHologram != null) {
                Destroy(currentHologram);
            }
        } else if (currentHologram != null) {
            Destroy(currentHologram);
        }
    }

    private void PlaceBlockWithAnimation(Vector3 targetPosition, Vector3 hitNormal, Vector3 hitPoint, Transform clickedBlockTransform) {
        if (creationParent.childCount >= maxShipSize && !Settings.Instance.InCreativeMode()) {
            PopupManager.Instance.ShowPopup("Your ship is at it's max size. Add more bridges if you would like to increase its size.", PopupType.OK);
            return;
        }

        if (specialCount >= maxSpecials && !Settings.Instance.InCreativeMode()) {
            PopupManager.Instance.ShowPopup("Your ship has all the specials it can handle already. Add more advanced bridges if you would like to have more.", PopupType.OK);
            return;
        }

        SoundManager.Instance.PlaySound("Hit");

        Item item = Inventory.Instance.GetSelectedItem();
        GameObject tierInput = GameObject.Find("ItemTierInputField");
        if (Settings.Instance.InCreativeMode()) item.Tier = long.Parse(tierInput.GetComponent<TMP_InputField>().text);

        GameObject block = Inventory.Instance.GetSelectedItemBlock();
        block.layer = 0;
        Utility.ApplyToAllChildren(block.transform, (Transform child) => child.gameObject.layer = 2);

        Vector3 spawnPosition = CalculateSpawnPosition(hitNormal);
        Transform currentParent = Settings.Instance.InCreativeMode() ? creativeParent : creationParent;
        GameObject instantiatedBlock = Instantiate(block, spawnPosition, Quaternion.identity, currentParent);
        instantiatedBlock.GetComponent<ItemBlock>().SetItem(item);
        ColorPicker.ApplyColorToBlock(instantiatedBlock.transform, USE_SELECTED);

        PlaceBlockWithCorrectRotation(instantiatedBlock.transform, hitNormal, hitPoint, clickedBlockTransform);
        _ = StartCoroutine(MoveBlockToPosition(instantiatedBlock, targetPosition, 0.5f));
    }

    private void PlaceBlockWithCorrectRotation(Transform blockTransform, Vector3 hitNormal, Vector3 hitPoint, Transform clickedBlockTransform) {
        Quaternion alignToSurface = Quaternion.FromToRotation(Vector3.up, hitNormal);
        blockTransform.localRotation = alignToSurface;

        Vector3 centerPosition = hitPoint - clickedBlockTransform.position;

        if (hitNormal == Vector3.forward || hitNormal == Vector3.back) {
            RotateBlockToEdge(blockTransform, hitNormal, centerPosition.x, centerPosition.y);
        } else if (hitNormal == Vector3.up || hitNormal == Vector3.down) {
            RotateBlockToEdge(blockTransform, hitNormal, centerPosition.x, centerPosition.z);
        } else if (hitNormal == Vector3.right || hitNormal == Vector3.left) {
            RotateBlockToEdge(blockTransform, hitNormal, centerPosition.y, centerPosition.z);
        }
    }

    private void RotateBlockToEdge(Transform blockTransform, Vector3 hitNormal, float hitPointAxis1, float hitPointAxis2) {
        float rotationAngle;

        if (Mathf.Abs(hitPointAxis1) > MathF.Abs(hitPointAxis2)) {
            float rotationDirection1 = hitNormal == Vector3.right ? 270f : 90f;
            float rotationDirection2 = hitNormal == Vector3.right ? 90f : 270f;
            rotationAngle = hitPointAxis1 > 0 ? rotationDirection1 : rotationDirection2;
        } else {
            float rotationDirection1 = hitNormal == Vector3.forward || hitNormal == Vector3.down ? 180f : 0f;
            float rotationDirection2 = hitNormal == Vector3.forward || hitNormal == Vector3.down ? 0f : 180f;
            rotationAngle = hitPointAxis2 > 0 ? rotationDirection1 : rotationDirection2;
        }

        blockTransform.Rotate(hitNormal, rotationAngle, Space.World);
    }

    private Vector3 CalculateSpawnPosition(Vector3 surfaceNormal) {
        Vector3 cameraForward = Camera.main.transform.forward;

        float randomOffsetX = UnityEngine.Random.Range(-1f, 1f);
        float randomOffsetY = UnityEngine.Random.Range(-1f, 1f);
        float randomOffsetZ = UnityEngine.Random.Range(-1f, 1f);

        Vector3 randomDirection = new Vector3(
            cameraForward.x + randomOffsetX,
            cameraForward.y + randomOffsetY,
            cameraForward.z + randomOffsetZ
        ).normalized;

        Vector3 spawnDirection = -surfaceNormal.normalized - randomDirection;
        float spawnDistance = 10f;
        Vector3 spawnPosition = Camera.main.transform.position + (spawnDirection * spawnDistance);

        return spawnPosition;
    }

    private System.Collections.IEnumerator MoveBlockToPosition(GameObject block, Vector3 targetPosition, float duration) {
        Vector3 startPosition = block.transform.position;

        placingBlock = true;
        block.layer = 2;

        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            block.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = targetPosition;

        block.layer = 0;
        placingBlock = false;
    }

    private void DeleteBlockWithAnimation(GameObject block) {
        SoundManager.Instance.PlaySound("Woosh");
        Vector3 escapePosition = CalculateSpawnPosition(block.transform.position);
        _ = StartCoroutine(MoveBlockAndDestroy(block, escapePosition, 0.5f));
    }

    private System.Collections.IEnumerator MoveBlockAndDestroy(GameObject block, Vector3 targetPosition, float duration) {
        block.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);

        Destroy(block.GetComponent<BoxCollider>());

        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            block.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration), Quaternion.Slerp(startRotation, Quaternion.identity, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(block);
    }

    private void RotateBlock(Transform blockTransform, Vector3 hitNormal) {
        BlockProperties blockProperties = blockTransform.GetComponent<BlockProperties>();
        if (!blockProperties.CanRotate()) return;

        Vector3 rotationAxis = hitNormal == Vector3.up || hitNormal == Vector3.down
            ? Vector3.up
            : hitNormal == Vector3.left || hitNormal == Vector3.right
                ? Vector3.right
                : Vector3.forward;

        blockTransform.Rotate(rotationAxis, 90f, Space.World);
    }

    private bool CanPlaceBlockOnSide(Vector3 hitNormal, BlockProperties properties) {
        if (properties.IsSideAllowed("any")) return false;

        Vector3 localHitNormal = properties.transform.InverseTransformDirection(hitNormal);

        return (localHitNormal == Vector3.up && properties.IsSideAllowed("top"))
            || (localHitNormal == Vector3.down && properties.IsSideAllowed("bottom"))
            || (localHitNormal == Vector3.left && properties.IsSideAllowed("left"))
            || (localHitNormal == Vector3.right && properties.IsSideAllowed("right"))
            || (localHitNormal == Vector3.forward && properties.IsSideAllowed("front"))
            || (localHitNormal == Vector3.back && properties.IsSideAllowed("back"));
    }
}
