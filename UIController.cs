using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Collections.Generic;

public class UIController : MonoBehaviour {
    public static UIController Instance;

    [SerializeField] private GameObject shipCreation;
    [SerializeField] private GameObject mainHub;
    [SerializeField] private GameObject warpScene;
    [SerializeField] private GameObject playerList;
    [SerializeField] private GameObject creativeList;
    [SerializeField] private GameObject shipPartPrefab;
    [SerializeField] private GameObject shipEntryPrefab;
    [SerializeField] private GameObject selectShipButton;
    [SerializeField] private GameObject crosshairs;

    [SerializeField] private Transform inventory;
    [SerializeField] private Transform shopInventory;
    [SerializeField] private Transform mainMenu;
    [SerializeField] private Transform logo;
    [SerializeField] private Transform skipText;
    [SerializeField] private Transform shipStats;
    [SerializeField] private Transform hangerBackButton;
    [SerializeField] private Transform editShipButton;
    [SerializeField] private Transform shipMenu;
    [SerializeField] private Transform jumpBackButton;
    [SerializeField] private Transform jumpButton;
    [SerializeField] private Transform mapBackButton;
    [SerializeField] private Transform jumpInfo;
    [SerializeField] private Transform currentJumpInfo;
    [SerializeField] private Transform behaviorModule;
    [SerializeField] private Transform warpTopBar;

    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private CanvasGroup whiteScreen;

    [SerializeField] private float moveInventoryButtonDistance = 150.0f;
    [SerializeField] private float moveInventoryDistance = 1100.0f;
    [SerializeField] private float moveMainMenuDistance = 1800.0f;
    [SerializeField] private float moveHangerBackButtonDistance = 150.0f;
    [SerializeField] private float moveEditShipButtonDistance = 200.0f;
    [SerializeField] private float moveShipStatsDistance = 1300.0f;
    [SerializeField] private float moveJumpBackButtonDistance = 150.0f;
    [SerializeField] private float moveJumpInfoDistance = 1400.0f;
    [SerializeField] private float moveCurrentJumpInfoDistance = 1300.0f;
    [SerializeField] private float moveSelectJumpButtonDistance = 80.0f;
    [SerializeField] private float moveJumpTitleDistance = 80.0f;
    [SerializeField] private float moveJumpButtonDistance = 900.0f;
    [SerializeField] private float moveMapBackButtonDistance = 150.0f;
    [SerializeField] private float moveBehaviorTopDecorDistance = 450.0f;
    [SerializeField] private float moveBehaviorBottomDecorDistance = 300.0f;
    [SerializeField] private float scaleDuration = 1.0f;
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float fadeDuration = 1.0f;

    private CameraTrack cameraTrack;

    private Transform inventoryBackbutton;
    private Transform display;
    private Transform shopPanel;
    private Transform jumpTitle;
    private Transform selectJumpButton;
    private Transform jumpBackground;
    private Transform jumpDisplay;
    private Transform hangerDisplay;
    private Transform creationParent;
    private Transform creativeParent;
    private Transform playerShip;
    private Transform shipContentList;
    private Transform displayItem;
    private Transform displayInfo;
    private Transform behaviorMenu;
    private Transform behaviorTopDecor;
    private Transform behaviorBottomDecor;

    private TMP_InputField displayTierInputField;
    private TMP_InputField distanceBehavior;
    private TMP_InputField minScaleBehavior;
    private TMP_InputField maxScaleBehavior;
    private TMP_InputField minSpeedBehavior;
    private TMP_InputField maxSpeedBehavior;

    private TMP_Dropdown spawnBehavior;
    private TMP_Dropdown moveBehavior;
    private TMP_Dropdown faceBehavior;
    private TMP_Dropdown dodgeBehavior;
    private TMP_Dropdown aimBehavior;

    private GameObject energyBar;
    private GameObject healthBar;
    private GameObject shieldBar;
    private GameObject laserBar;
    private GameObject repulsorBar;
    private GameObject teleportBar;
    private GameObject radarBar;
    private GameObject cloakBar;

    private TextMeshProUGUI warpTimeLeft;
    private TextMeshProUGUI displayTier;
    private TextMeshProUGUI displayName;
    private TextMeshProUGUI itemStat1Label;
    private TextMeshProUGUI itemStat1Value;
    private TextMeshProUGUI itemStat2Label;
    private TextMeshProUGUI itemStat2Value;
    private TextMeshProUGUI itemStat3Label;
    private TextMeshProUGUI itemStat3Value;
    private TextMeshProUGUI jumpTitleText;
    private TextMeshProUGUI jumpTime;
    private TextMeshProUGUI currentJumpDificcultyLabel;
    private TextMeshProUGUI currentJumpPowerLabel;
    private TextMeshProUGUI currentJumpTimeLabel;
    private TextMeshProUGUI shipNameLabel;
    private TextMeshProUGUI shipStatNameLabel;
    private TextMeshProUGUI shipStatHealthLabel;
    private TextMeshProUGUI shipStatHealthRegenLabel;
    private TextMeshProUGUI shipStatEnergyLabel;
    private TextMeshProUGUI shipStatEnergyRegenLabel;
    private TextMeshProUGUI shipStatShieldLabel;
    private TextMeshProUGUI shipStatShieldRegenLabel;
    private TextMeshProUGUI shipStatTeleportLabel;
    private TextMeshProUGUI shipStatPushLabel;
    private TextMeshProUGUI shipStatSizeLabel;
    private TextMeshProUGUI shipStatSpecialsLabel;
    private TextMeshProUGUI shipStatCloakTimeLabel;
    private TextMeshProUGUI shipStatRadarDistLabel;
    private TextMeshProUGUI shipStatDPSLabel;
    private TextMeshProUGUI shipStatSpeedLabel;
    private TextMeshProUGUI shipStatJumpSpeedLabel;
    private TextMeshProUGUI shipStatPickupDistLabel;

    private List<string> playerCreationList = new List<string>();
    private List<string> creativeCreationList = new List<string>();
    private List<string> defaultShips = new List<string>();

    private const float MAX_BAR_HEIGHT = 165f;

    private string currentCreationSelected = string.Empty;
    private string currentShipSelected = string.Empty;
    private string newJumpDifficulty = "";
    private string selectedJumpDifficulty = "Medium";
    private float newJumpTime = 0f;
    private float selectedJumpTime = 10f;
    private long newJumpPower = 0;
    private long selectedJumpPower = 100;
    private bool isMoving = false;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        currentShipSelected = Load.String("CurrentShipSelected");
        newJumpTime = string.IsNullOrEmpty(Load.String("JumpTime")) ? 0f : float.Parse(Load.String("JumpTime"));
        newJumpPower = string.IsNullOrEmpty(Load.String("JumpPower")) ? 0 : long.Parse(Load.String("JumpPower"));
        newJumpDifficulty = string.IsNullOrEmpty(Load.String("JumpDifficulty")) ? "" : Load.String("JumpDifficulty");
        string playerCreationListSave = Load.String("PlayerCreationListSave");
        string creativeCreationListSave = Load.String("CreativeCreationListSave");
        List<string> playerSaves = new List<string>(playerCreationListSave.Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries));
        List<string> creativeSaves = new List<string>(creativeCreationListSave.Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries));

        LoadShipsFromFile();

        if (playerSaves.Count == 0) {
            Save.String("Starter", StringEncoder.DecodeString(GetShip(0)));
            playerSaves.Add("Starter");
        }
        
        if (creativeSaves.Count == 0) {
            Save.String("Small Asteroid", StringEncoder.DecodeString(GetShip(1)));
            creativeSaves.Add("Small Asteroid");
            Save.String("Medium Asteroid", StringEncoder.DecodeString(GetShip(2)));
            creativeSaves.Add("Medium Asteroid");
            Save.String("Large Asteroid", StringEncoder.DecodeString(GetShip(3)));
            creativeSaves.Add("Large Asteroid");
            Save.String("Pirate", StringEncoder.DecodeString(GetShip(4)));
            creativeSaves.Add("Pirate");
            Save.String("Brigade", StringEncoder.DecodeString(GetShip(5)));
            creativeSaves.Add("Brigade");
            Save.String("Ram", StringEncoder.DecodeString(GetShip(6)));
            creativeSaves.Add("Ram");
            Save.String("Comet", StringEncoder.DecodeString(GetShip(7)));
            creativeSaves.Add("Comet");
            Save.String("Rocket", StringEncoder.DecodeString(GetShip(8)));
            creativeSaves.Add("Rocket");
        }

        foreach (string save in playerSaves) {
            playerCreationList.Add(save);
        }

        foreach (string save in creativeSaves) {
            creativeCreationList.Add(save);
        }
    }

    void Start() {
        transform.SetPositionAndRotation(new Vector3(0, 500, 0), transform.rotation);

        inventoryBackbutton = shopInventory.Find("BackButton");
        shopPanel = shopInventory.Find("ShopPanel");

        ReassignInventory(true);

        behaviorTopDecor = behaviorModule.Find("BehaviorTopDecor");
        behaviorBottomDecor = behaviorModule.Find("BehaviorBottomDecor");
        behaviorMenu = behaviorModule.Find("BehaviorMenu");
        spawnBehavior = behaviorMenu.Find("SpawnDropdown").GetComponent<TMP_Dropdown>();
        moveBehavior = behaviorMenu.Find("MoveDropdown").GetComponent<TMP_Dropdown>();
        faceBehavior = behaviorMenu.Find("FaceDropdown").GetComponent<TMP_Dropdown>();
        dodgeBehavior = behaviorMenu.Find("DodgeDropdown").GetComponent<TMP_Dropdown>();
        aimBehavior = behaviorMenu.Find("AimDropdown").GetComponent<TMP_Dropdown>();
        distanceBehavior = behaviorMenu.Find("DistanceInputField").GetComponent<TMP_InputField>();
        minScaleBehavior = behaviorMenu.Find("MinScaleInputField").GetComponent<TMP_InputField>();
        maxScaleBehavior = behaviorMenu.Find("MaxScaleInputField").GetComponent<TMP_InputField>();
        minSpeedBehavior = behaviorMenu.Find("MinSpeedInputField").GetComponent<TMP_InputField>();
        maxSpeedBehavior = behaviorMenu.Find("MaxSpeedInputField").GetComponent<TMP_InputField>();

        behaviorMenu.gameObject.SetActive(false);

        energyBar = warpTopBar.Find("EnergyBar").gameObject;
        healthBar = warpTopBar.Find("HealthBar").gameObject;
        shieldBar = warpTopBar.Find("ShieldBar").gameObject;
        laserBar = warpTopBar.Find("LaserBar").gameObject;
        repulsorBar = warpTopBar.Find("RepulsorBar").gameObject;
        teleportBar = warpTopBar.Find("TeleportBar").gameObject;
        radarBar = warpTopBar.Find("RadarBar").gameObject;
        cloakBar = warpTopBar.Find("CloakBar").gameObject;
        warpTimeLeft = warpTopBar.Find("JumpTime").GetComponent<TextMeshProUGUI>();

        jumpTitle = jumpInfo.Find("JumpTitle");
        jumpTitleText = jumpInfo.Find("JumpTitle").GetComponent<TextMeshProUGUI>();
        jumpTime = jumpInfo.Find("Background").Find("JumpTime").GetComponent<TextMeshProUGUI>();
        selectJumpButton = jumpInfo.Find("SelectJumpButton");
        jumpBackground = jumpInfo.Find("Background");

        currentJumpDificcultyLabel = currentJumpInfo.Find("DifficultyLabel").GetComponent<TextMeshProUGUI>();
        currentJumpPowerLabel = currentJumpInfo.Find("PowerLabel").GetComponent<TextMeshProUGUI>();
        currentJumpTimeLabel = currentJumpInfo.Find("TimeLabel").GetComponent<TextMeshProUGUI>();

        shipNameLabel = shipMenu.Find("ShipNameLabel").GetComponent<TextMeshProUGUI>();
        shipContentList = shipMenu.Find("ShipContents").Find("Viewport").Find("Contents");

        shipStatNameLabel = shipStats.Find("ShipStatNameLabel").GetComponent<TextMeshProUGUI>();
        shipStatHealthLabel = shipStats.Find("ShipHealthLabel").GetComponent<TextMeshProUGUI>();
        shipStatHealthRegenLabel = shipStats.Find("ShipHealthRegenLabel").GetComponent<TextMeshProUGUI>();
        shipStatEnergyLabel = shipStats.Find("ShipEnergyLabel").GetComponent<TextMeshProUGUI>();
        shipStatEnergyRegenLabel = shipStats.Find("ShipEnergyRegenLabel").GetComponent<TextMeshProUGUI>();
        shipStatShieldLabel = shipStats.Find("ShipShieldLabel").GetComponent<TextMeshProUGUI>();
        shipStatShieldRegenLabel = shipStats.Find("ShipShieldRegenLabel").GetComponent<TextMeshProUGUI>();
        shipStatTeleportLabel = shipStats.Find("ShipTeleLabel").GetComponent<TextMeshProUGUI>();
        shipStatPushLabel = shipStats.Find("ShipPushLabel").GetComponent<TextMeshProUGUI>();
        shipStatSizeLabel = shipStats.Find("ShipSizeLabel").GetComponent<TextMeshProUGUI>();
        shipStatSpecialsLabel = shipStats.Find("ShipSpecialsLabel").GetComponent<TextMeshProUGUI>();
        shipStatCloakTimeLabel = shipStats.Find("ShipCloakLabel").GetComponent<TextMeshProUGUI>();
        shipStatRadarDistLabel = shipStats.Find("ShipRadarLabel").GetComponent<TextMeshProUGUI>();
        shipStatDPSLabel = shipStats.Find("ShipDPSLabel").GetComponent<TextMeshProUGUI>();
        shipStatSpeedLabel = shipStats.Find("ShipSpeedLabel").GetComponent<TextMeshProUGUI>();
        shipStatJumpSpeedLabel = shipStats.Find("ShipJumpSpeedLabel").GetComponent<TextMeshProUGUI>();
        shipStatPickupDistLabel = shipStats.Find("ShipPickupLabel").GetComponent<TextMeshProUGUI>();

        Utility.Instance.MoveTransform(behaviorTopDecor, moveBehaviorTopDecorDistance, 0, 0);
        Utility.Instance.MoveTransform(behaviorBottomDecor, -moveBehaviorBottomDecorDistance, 0, 0);
        Utility.Instance.FadeCanvasGroup(behaviorMenu.GetComponent<CanvasGroup>(), 0);

        Utility.Instance.MoveTransform(inventoryBackbutton, -moveInventoryButtonDistance, 0, 0);
        Utility.Instance.MoveTransform(shopPanel, moveInventoryDistance, 0, 0);
        Utility.Instance.ScaleTransform(display, false, 0);

        Utility.Instance.MoveTransform(shipStats, 0, moveShipStatsDistance, 0);
        Utility.Instance.MoveTransform(hangerBackButton, -moveHangerBackButtonDistance, 0, 0);
        Utility.Instance.MoveTransform(editShipButton, moveEditShipButtonDistance, 0, 0);
        Utility.Instance.ScaleTransform(shipMenu, false, 0);

        Utility.Instance.MoveTransform(jumpBackButton, -moveJumpBackButtonDistance, 0, 0);
        Utility.Instance.MoveTransform(jumpButton, 0, moveJumpButtonDistance, 0);

        Utility.Instance.MoveTransform(mapBackButton, -moveMapBackButtonDistance, 0, 0);
        Utility.Instance.MoveTransform(jumpInfo, 0, moveJumpInfoDistance, 0);
        Utility.Instance.MoveTransform(currentJumpInfo, 0, moveCurrentJumpInfoDistance, 0);
        Utility.Instance.ScaleTransform(jumpBackground, false, 0);
        Utility.Instance.MoveTransform(jumpTitle, -moveJumpTitleDistance, 0, 0);
        Utility.Instance.MoveTransform(selectJumpButton, moveSelectJumpButtonDistance, 0, 0);

        ShowPlayerList();

        if (currentShipSelected != string.Empty) {
            SelectJumpLocation();

            string selectedShipSave = Load.String(currentShipSelected);

            if (!string.IsNullOrEmpty(selectedShipSave)) {
                int separatorIndex = selectedShipSave.IndexOf('/');
                selectedShipSave = selectedShipSave[(separatorIndex + 1)..];
                LoadCreation(selectedShipSave, jumpDisplay);
            }
        }

        AmbientManager.Instance.PlayAmbient("MenuAmbience");
    }

    void Update() {
        if (SceneManager.GetActiveScene().name != "WarpScene") return;

        if (Input.GetMouseButtonDown(1)) {
            crosshairs.SetActive(true);
        } else if (Input.GetMouseButtonUp(1)) {
            crosshairs.SetActive(false);
        }
    }

    void OnDestroy() {
        string playerCreationListSave = string.Empty;
        string creativeCreationListSave = string.Empty;

        foreach (string save in playerCreationList) {
            playerCreationListSave += save + ":";
        }

        foreach (string save in creativeCreationList) {
            if (save != string.Empty) creativeCreationListSave += save + ":";
        }

        Save.String("PlayerCreationListSave", playerCreationListSave);
        Save.String("CreativeCreationListSave", creativeCreationListSave);
        Save.String("CurrentShipSelected", currentShipSelected);
        Save.String("JumpTime", selectedJumpTime.ToString());
        Save.String("JumpPower", selectedJumpPower.ToString());
        Save.String("JumpDifficulty", selectedJumpDifficulty);
    }

    public List<string> GetCreativeCreationList() => creativeCreationList;

    public void ShowMain() {
        Utility.Instance.DoIfNotMoving(moveDuration, () => {
            Utility.Instance.MoveTransform(mainMenu, 0, -moveMainMenuDistance, moveDuration);
            Utility.Instance.ScaleTransform(logo, true, scaleDuration);
        });

        AmbientManager.Instance.PlayAmbient("MenuAmbience");
    }

    public void UIUpdateOnArrived(string currentPath) {
        skipText.gameObject.SetActive(false);

        if (currentPath.Contains("ToMain")) {
            ShowMain();
        } else if (currentPath == "MainToStore") {
            Utility.Instance.DoIfNotMoving(moveDuration, () => {
                Utility.Instance.MoveTransform(inventoryBackbutton, moveInventoryButtonDistance, 0, moveDuration);
                Utility.Instance.MoveTransform(shopPanel, -moveInventoryDistance, 0, moveDuration);
                Utility.Instance.ScaleTransform(display, true, scaleDuration);
            });
        } else if (currentPath == "MainToCreation") {
            AmbientManager.Instance.PlayAmbient("Hanger");

            Utility.Instance.DoIfNotMoving(moveDuration, () => {
                selectShipButton.SetActive(!Settings.Instance.InCreativeMode());
                Utility.Instance.MoveTransform(shipStats, 0, -moveShipStatsDistance, moveDuration);
                Utility.Instance.MoveTransform(hangerBackButton, moveHangerBackButtonDistance, 0, moveDuration);
                Utility.Instance.MoveTransform(editShipButton, -moveEditShipButtonDistance, 0, moveDuration);
                Utility.Instance.ScaleTransform(shipMenu, true, scaleDuration);
            });
        } else if (currentPath == "CreationToShip") {
            AmbientManager.Instance.StopChannel(2);

            Utility.Instance.DoIfNotMoving(moveDuration, () => {
                ChangeScene.Instance.GoToScene("ShipCreation");
                Utility.Instance.FadeCanvasGroup(blackScreen, 1);
            });
        } else if (currentPath == "MainToJump") {
            AmbientManager.Instance.PlayAmbient("Portal");

            Utility.Instance.DoIfNotMoving(moveDuration, () => {
                UpdateShipStats(currentShipSelected, jumpDisplay);
                selectShipButton.SetActive(false);
                Utility.Instance.MoveTransform(jumpBackButton, moveJumpBackButtonDistance, 0, moveDuration);
                Utility.Instance.MoveTransform(jumpButton, 0, -moveJumpButtonDistance, moveDuration);
                Utility.Instance.MoveTransform(shipStats, 0, -moveShipStatsDistance, moveDuration);
                Utility.Instance.MoveTransform(currentJumpInfo, 0, -moveCurrentJumpInfoDistance, moveDuration);
            });
        } else if (currentPath == "JumpToWarp") {
            AmbientManager.Instance.StopChannel(2);

            Utility.Instance.DoIfNotMoving(2, () => {
                ChangeScene.Instance.GoToScene("WarpScene");
                Utility.Instance.FadeCanvasGroup(whiteScreen, 2);
                AmbientManager.Instance.PlayAmbient("WarpSpace");
            });
        } else if (currentPath == "MainToMap") {
            AmbientManager.Instance.PlayAmbient("Hologram");

            Utility.Instance.DoIfNotMoving(2, () => {
                UpdateMapPoints();
                Utility.Instance.MoveTransform(mapBackButton, moveMapBackButtonDistance, 0, moveDuration);
                Utility.Instance.MoveTransform(jumpInfo, 0, -moveJumpInfoDistance, moveDuration);
                Utility.Instance.MoveTransform(currentJumpInfo, 0, -moveCurrentJumpInfoDistance, moveDuration);
                Utility.Instance.ScaleTransform(jumpBackground, true, scaleDuration, true, 1);
                Utility.Instance.MoveTransform(jumpTitle, moveJumpTitleDistance, 0, moveDuration, 1);
                Utility.Instance.MoveTransform(selectJumpButton, -moveSelectJumpButtonDistance, 0, moveDuration, 1);
            });
        }
    }

    public bool UIFinishedMoving(string currentPath) {
        if (isMoving) {
            float buttonOffset = 50f;

            if ((currentPath == "StoreToMain" && inventoryBackbutton.localPosition.y <= -moveInventoryButtonDistance + buttonOffset)
                || (currentPath.Contains("MainTo") && mainMenu.localPosition.x >= moveMainMenuDistance)
                || (currentPath == "CreationToMain" && shipStats.localPosition.x >= moveShipStatsDistance)
                || (currentPath == "CreationToShip" && shipStats.localPosition.x >= moveShipStatsDistance)
                || (currentPath == "JumpToMain" && jumpBackButton.localPosition.y <= -moveJumpBackButtonDistance + buttonOffset)
                || (currentPath == "JumpToWarp" && jumpBackButton.localPosition.y <= -moveJumpBackButtonDistance + buttonOffset)
                || (currentPath == "MapToMain" && jumpInfo.localPosition.x >= moveJumpInfoDistance)) {
                isMoving = false;

                if (currentPath is not "CreationToShip" and not "JumpToWarp") {
                    skipText.gameObject.SetActive(true);
                }

                return true;
            }
        }

        return false;
    }

    public void MoveUI(string pathName) {
        switch (pathName) {
            case "StoreToMain":
                HideStoreUI();
                break;
            case "JumpToMain":
                HideJumpUI();
                break;
            case "JumpToWarp":
                HideJumpUI();
                break;
            case "CreationToMain":
                HideCreationUI();
                break;
            case "CreationToShip":
                HideCreationUI(true);
                break;
            case "MapToMain":
                HideMapUI();
                break;
            case "BridgeToMain":
                break;
            default:
                if (pathName.Contains("MainTo")) HideMainUI();
                break;
        }

        isMoving = true;
    }

    public void SwitchCameraTrack(string path) {
        SoundManager.Instance.PlaySound("Button");
        if (!cameraTrack) return;
        cameraTrack.MovePath(path);
    }

    public void ExitGame() {
        SoundManager.Instance.PlaySound("Button");

        PopupManager.Instance.ShowPopup("Are you sure you want to exit the game?", PopupType.YesNo, () => {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });
    }

    public void ExportCreation() {
        SoundManager.Instance.PlaySound("Button");
        PopupManager.Instance.ShowPopup(StringEncoder.EncodeString(Load.String(currentCreationSelected)), PopupType.InputOK);
    }

    public void ImportCreation() {
        SoundManager.Instance.PlaySound("Button");

        PopupManager.Instance.ShowPopup("Enter Code...\n\nKeep in mind this will override the creation you currently have selected: " + currentCreationSelected, PopupType.InputYesNo, () => {
            string importedCode = StringEncoder.DecodeString(PopupManager.Instance.GetInput());
            int separatorIndex = importedCode.IndexOf('|');
            string boolString = importedCode[..separatorIndex];
            bool inCreativeMode = bool.Parse(boolString);

            if (inCreativeMode != Settings.Instance.InCreativeMode()) importedCode = !inCreativeMode + importedCode[separatorIndex..];

            if (!Settings.Instance.InCreativeMode() && inCreativeMode) {
                PopupManager.Instance.ShowPopup("You can't import a creative creation to your player list.", PopupType.InputOK);
            } else {
                Save.String(currentCreationSelected, importedCode);
                UpdateHangerDisplays(currentCreationSelected);
            }
        });
    }

    public void GoToShipCreation() => SwitchCameraTrack("CreationToShip");

    public void GoToWarpScene() {
        if (currentShipSelected != string.Empty) {
            if (selectedJumpDifficulty != string.Empty) {
                SwitchCameraTrack("JumpToWarp");
            } else {
                PopupManager.Instance.ShowPopup("You need to select a jump in the map room first.", PopupType.OK);
            }
        } else {
            PopupManager.Instance.ShowPopup("You need to select a ship in the hanger first.", PopupType.OK);
        }
    }

    public string GetSelectedCreationName() => currentCreationSelected;
    public string GetSelectedJumpInfo() => selectedJumpDifficulty + ":" + selectedJumpTime + ":" + selectedJumpPower;

    public void ShowPlayerList() {
        SoundManager.Instance.PlaySound("Button");
        ShowList(playerCreationList, false);
    }

    public void ShowCreativeList() {
        SoundManager.Instance.PlaySound("Button");
        ShowList(creativeCreationList, true);
    }

    public void NewCreation() {
        SoundManager.Instance.PlaySound("Button");

        PopupManager.Instance.ShowPopup("What would you like to name your new creation?", PopupType.InputYesNo, () => {
            string inputText = PopupManager.Instance.GetInput();
            string creationName = inputText.Replace("\r", "").Replace("\n", "");
            string checkLoad = Load.String(inputText);

            if (string.IsNullOrEmpty(checkLoad)) {
                AddCreationToList(creationName);
            } else if (checkLoad.StartsWith("true") && Settings.Instance.InCreativeMode()) {
                AddCreationToList(creationName);
            } else if (checkLoad.StartsWith("false")) {
                AddCreationToList(creationName);
            } else {
                PopupManager.Instance.ShowPopup("You can't name your ship that.", PopupType.OK);
            }
        });
    }

    private void AddCreationToList(string creationName) {
        if (Settings.Instance.InCreativeMode()) {
            creativeCreationList.Add(creationName);
            ShowCreativeList();
        } else {
            playerCreationList.Add(creationName);
            ShowPlayerList();
        }
    }

    public void DeleteCreation() {
        SoundManager.Instance.PlaySound("Button");

        PopupManager.Instance.ShowPopup("Are you sure you would like to delete this creation?", PopupType.YesNo, () => {
            if (Settings.Instance.InCreativeMode()) {
                _ = creativeCreationList.Remove(currentCreationSelected);
                ShowCreativeList();
            } else {
                _ = playerCreationList.Remove(currentCreationSelected);
                ShowPlayerList();
            }
        });
    }

    public void SelectCreation() {
        currentShipSelected = shipNameLabel.text;
        string selectedShipSave = Load.String(currentShipSelected);

        if (!string.IsNullOrEmpty(selectedShipSave)) {
            int separatorIndex = selectedShipSave.IndexOf('/');
            selectedShipSave = selectedShipSave[(separatorIndex + 1)..];
            LoadCreation(selectedShipSave, jumpDisplay);
        }

        newJumpDifficulty = string.Empty;
        newJumpPower = 0;
        newJumpTime = 0f;
        SelectJumpLocation();
    }

    public long GetSelectedShipPower() {
        long creationTotalPower = 0;

        foreach (Transform block in jumpDisplay) {
            Item blockItem = block.gameObject.GetComponent<ItemBlock>().GetItem();
            creationTotalPower += blockItem.Tier * 10;
        }

        return creationTotalPower;
    }

    public string GetShip(int index) => index >= 0 && index < defaultShips.Count ? defaultShips[index] : null;

    public void SelectJumpLocation() {
        if (currentShipSelected != string.Empty) {
            SoundManager.Instance.PlaySound("Button");
            selectedJumpTime = newJumpTime;
            selectedJumpPower = newJumpPower;
            selectedJumpDifficulty = newJumpDifficulty;

            currentJumpTimeLabel.text = Utility.ConvertMinutesToTimeFormat(newJumpTime);
            currentJumpPowerLabel.text = newJumpPower.ToString();
            currentJumpDificcultyLabel.text = newJumpDifficulty;
        } else {
            PopupManager.Instance.ShowPopup("You need to select a ship in the hanger first.", PopupType.OK);
        }
    }

    public void SelectMapPoint(string difficulty, long jumpPowerLevel) {
        SoundManager.Instance.PlaySound("Button");
        if (currentShipSelected == null) PopupManager.Instance.ShowPopup("You need to select a ship in the hanger before you can select a location to travel to. How would we know how long it's going to take without seeing your engines first?", PopupType.OK);
        newJumpTime = CalculateJumpTime(jumpPowerLevel, GetSelectedShipPower());
        newJumpPower = jumpPowerLevel;
        newJumpDifficulty = difficulty;

        jumpTitleText.text = difficulty + "\n" + jumpPowerLevel;
        jumpTime.text = "Jump Time | " + Utility.ConvertMinutesToTimeFormat(newJumpTime);
    }

    public float CalculateJumpTime(long jumpPowerLevel, long shipPower) {
        float baseTime = 10f;
        float maxTime = 15f;
        float minTime = 3f;
        float scale = 1.2f;

        float powerDifference = Mathf.Abs(jumpPowerLevel - shipPower);
        float adjustedDifference = Mathf.Log10(powerDifference + 1) * scale;
        float timeAdjustment = adjustedDifference * 0.5f;
        float newTime = baseTime + (jumpPowerLevel > shipPower ? timeAdjustment : -timeAdjustment);

        float turboMultiplier = 1f;

        foreach (Transform block in jumpDisplay) {
            if (block.name.Contains("Turbo")) {
                Item blockItem = block.gameObject.GetComponent<ItemBlock>().GetItem();

                if (blockItem != null) {
                    turboMultiplier += Inventory.GetBlockParameter("jumpSpeedTotal", blockItem.Tier, 1);
                }
            }
        }

        float newJumpTime = newTime / turboMultiplier;

        return Mathf.Clamp(newJumpTime, minTime, maxTime);
    }

    public void UpdateHangerDisplays(string creationName) {
        Utility.ClearChildObjects(hangerDisplay);

        Utility.Instance.Delay(0.1f, () => {
            editShipButton.gameObject.SetActive(true);

            shipNameLabel.text = creationName;
            currentCreationSelected = creationName;
            string creationSave = Load.String(creationName);

            if (!string.IsNullOrEmpty(creationSave)) {
                int separatorIndex = creationSave.IndexOf('/');
                creationSave = creationSave[(separatorIndex + 1)..];
                LoadCreation(creationSave, hangerDisplay);
            }

            Dictionary<Item, int> blockList = new Dictionary<Item, int>();

            foreach (Transform block in hangerDisplay) {
                Item blockItem = block.gameObject.GetComponent<ItemBlock>().GetItem();

                if (blockList.ContainsKey(blockItem)) {
                    blockList[blockItem]++;
                } else {
                    blockList[blockItem] = 1;
                }
            }

            Utility.ClearChildObjects(shipContentList);

            bool inCreativeMode = Settings.Instance.InCreativeMode();

            if (!inCreativeMode) {
                Inventory.Instance.ResetItemsUsed();
            }

            foreach (KeyValuePair<Item, int> block in blockList) {
                GameObject part = Instantiate(shipPartPrefab, shipContentList);
                part.transform.Find("PartNameLabel").GetComponent<TextMeshProUGUI>().text = block.Key.Name + " - Tier " + block.Key.Tier;
                part.transform.Find("PartQuantityLabel").GetComponent<TextMeshProUGUI>().text = "x " + block.Value;

                if (!inCreativeMode) {
                    Item selectedItem = Inventory.Instance.Items.Find(item => item.Name == block.Key.Name && item.Type == block.Key.Type && item.Tier == block.Key.Tier && item.Icon == block.Key.Icon);
                    selectedItem?.ChangeQuantityUsed(block.Value);
                }

                GameObject partIcon = Instantiate(Resources.Load<GameObject>("Blocks/" + block.Key.Type + "/" + block.Key.Icon), part.transform.Find("PartIcon"));
                partIcon.layer = 5;
                Utility.ApplyToAllChildren(partIcon.transform, (Transform child) => child.gameObject.layer = 5);
            }

            UpdateShipStats(creationName, hangerDisplay);
        });
    }

    public void UpdateDisplayInformation() {
        bool isShop = false;

        if (SceneManager.GetActiveScene().name == "MainHub") isShop = true;

        ReassignInventory(isShop);

        if (displayItem.Find("DisplayItemIcon") == null) return;
        if (displayItem.Find("DisplayItemIcon").GetChild(0).TryGetComponent<ItemBlock>(out var itemBlock)) {
            Item item = itemBlock.GetItem();
            long tier;

            if (item == null) return;

            if (Settings.Instance.InCreativeMode()) {
                displayTierInputField = displayInfo.Find("ItemTierInputField").GetComponent<TMP_InputField>();
                _ = long.TryParse(displayTierInputField.text, out long parseLong);
                tier = parseLong;
            } else {
                displayTier = displayInfo.Find("ItemTier").GetComponent<TextMeshProUGUI>();
                displayTier.text = "Tier " + item.Tier;
                tier = item.Tier;
            }

            int blockGrade = item.GetGrade();

            displayName.text = item.Name;

            itemStat1Label.text = "Health";
            itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, 1).ToString();
            itemStat2Label.text = string.Empty;
            itemStat2Value.text = string.Empty;
            itemStat3Label.text = string.Empty;
            itemStat3Value.text = string.Empty;

            switch (item.Name) {
                case "Hull":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    break;
                case "Bridge":
                    itemStat2Label.text = "Max Size";
                    itemStat2Value.text = Inventory.GetBlockParameter("maxSizeTotal", tier, blockGrade).ToString();
                    break;
                case "Cannon":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Damage";
                    itemStat2Value.text = Inventory.GetBlockParameter("dpsCannonTotal", tier, blockGrade).ToString();
                    break;
                case "Missile Launcher":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Damage";
                    itemStat2Value.text = Inventory.GetBlockParameter("dpsMissileTotal", tier, blockGrade).ToString();
                    break;
                case "Railgun":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Damage";
                    itemStat2Value.text = Inventory.GetBlockParameter("dpsRailgunTotal", tier, blockGrade).ToString();
                    break;
                case "Engine":
                    itemStat2Label.text = "Speed";
                    itemStat2Value.text = Inventory.GetBlockParameter("speedTotal", tier, blockGrade).ToString();
                    break;
                case "Fusion Core":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Energy Regen";
                    itemStat2Value.text = Inventory.GetBlockParameter("energyRegenTotal", tier, blockGrade).ToString();
                    break;
                case "Energy Storage":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Energy Storage";
                    itemStat2Value.text = Inventory.GetBlockParameter("energyTotal", tier, blockGrade).ToString();
                    break;
                case "Laser":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Damage";
                    itemStat2Value.text = Inventory.GetBlockParameter("dpsLaserTotal", tier, blockGrade).ToString();
                    break;
                case "Repulsor":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Push Distance";
                    itemStat2Value.text = Inventory.GetBlockParameter("pushDistTotal", tier, blockGrade).ToString();
                    break;
                case "Minigun":
                    displayName.text = item.Name + " G" + blockGrade + " | Grade " + blockGrade;
                    itemStat1Value.text = Inventory.GetBlockParameter("healthTotal", tier, blockGrade).ToString();
                    itemStat2Label.text = "Damage";
                    itemStat2Value.text = Inventory.GetBlockParameter("dpsMinigunTotal", tier, blockGrade).ToString();
                    break;
                case "Advanced Bridge":
                    itemStat2Label.text = "Max Specials";
                    itemStat2Value.text = Inventory.GetBlockParameter("maxSpecialsTotal", tier, blockGrade).ToString();
                    break;
                case "Cloaking Device":
                    itemStat2Label.text = "Cloak Time";
                    itemStat2Value.text = Mathf.Clamp(Inventory.GetBlockParameter("cloakTimeTotal", tier, blockGrade), 3f, 15f).ToString();
                    break;
                case "Magnet":
                    itemStat2Label.text = "Pickup Distance";
                    itemStat2Value.text = (Inventory.GetBlockParameter("pickupDistTotal", tier, blockGrade) + 10).ToString();
                    break;
                case "Quantum Drive":
                    itemStat2Label.text = "Tele Distance";
                    itemStat2Value.text = Inventory.GetBlockParameter("teleportDistTotal", tier, blockGrade).ToString();
                    break;
                case "Radar":
                    itemStat2Label.text = "Radar Distance";
                    itemStat2Value.text = Inventory.GetBlockParameter("radarDistTotal", tier, blockGrade).ToString();
                    break;
                case "Repair Module":
                    itemStat2Label.text = "Health Regen";
                    itemStat2Value.text = Inventory.GetBlockParameter("healthRegenTotal", tier, blockGrade).ToString();
                    break;
                case "Shield Generator":
                    itemStat2Label.text = "Shield Health";
                    itemStat2Value.text = Inventory.GetBlockParameter("shieldTotal", tier, blockGrade).ToString();
                    itemStat3Label.text = "Shield Regen";
                    itemStat3Value.text = Inventory.GetBlockParameter("shieldRegenTotal", tier, blockGrade).ToString();
                    break;
                case "Turbo Engine":
                    itemStat2Label.text = "Jump Speed";
                    itemStat2Value.text = Inventory.GetBlockParameter("jumpSpeedTotal", tier, blockGrade).ToString();
                    break;
            }
        }

        ReassignInventory(true);
    }

    public void ToggleBehaviorUI(bool showUI) {
        if (Utility.IsMoving) return;
        Utility.IsMoving = true;

        int direction = showUI ? -1 : 1;

        behaviorMenu.gameObject.SetActive(true);

        Utility.Instance.MoveTransform(behaviorTopDecor, moveBehaviorTopDecorDistance * direction, 0, moveDuration);
        Utility.Instance.MoveTransform(behaviorBottomDecor, -moveBehaviorBottomDecorDistance * direction, 0, moveDuration);
        Utility.Instance.FadeCanvasGroup(behaviorMenu.GetComponent<CanvasGroup>(), fadeDuration, showUI);
        Utility.Instance.Delay(moveDuration, () => {
            Utility.IsMoving = false;
            behaviorMenu.gameObject.SetActive(showUI);
        });
    }

    public void SaveCreation() {
        Transform currentParent = Settings.Instance.InCreativeMode() ? creativeParent : creationParent;
        string saveString = Settings.Instance.InCreativeMode().ToString() + "|";
        saveString += spawnBehavior.value + ":" + moveBehavior.value + ":" + faceBehavior.value + ":" + dodgeBehavior.value + ":" + aimBehavior.value + ":" + distanceBehavior.text + ":" + minScaleBehavior.text + ":" + maxScaleBehavior.text + ":" + minSpeedBehavior.text + ":" + maxSpeedBehavior.text + "/";

        foreach (Transform child in currentParent) {
            ItemBlock childItemBlock = child.GetComponent<ItemBlock>();
            Item childItem = childItemBlock.GetItem();

            if (childItem != null) {
                saveString += childItem.Name + ":" + childItem.Type.ToString() + ":" + childItem.SpecialType.ToString() + ":" + childItem.Tier + ":" + childItem.Icon + ":" + childItemBlock.GetColor().ToString() + ":" + child.position + ":" + child.rotation + "/";
            } else {
                saveString += "Bridge:Bridge:Null:1:Bridge1:" + childItemBlock.GetColor().ToString() + ":" + child.position + ":" + child.rotation + "/";
            }
        }

        Save.String(currentCreationSelected, saveString);
    }

    public void LoadCreation(string creationSave, Transform parent, bool enemyShip = true) {
        foreach (Transform child in parent) {
            if (!child.name.Contains("Camera")) Destroy(child.gameObject);
        }

        string[] blocks = creationSave.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string block in blocks) {
            string[] blockProperties = block.Split(':', System.StringSplitOptions.RemoveEmptyEntries);

            string name = blockProperties[0];
            ItemType type = Enum.Parse<ItemType>(blockProperties[1]);
            SpecialType specialType = Enum.Parse<SpecialType>(blockProperties[2]);
            long tier = long.Parse(blockProperties[3]);
            string icon = blockProperties[4];
            Color color = Utility.ParseColorFromRGBAString(blockProperties[5]);
            Vector3 position = Utility.ParseVector3FromString(blockProperties[6]);
            Quaternion rotation = Utility.ParseQuaternionFromString(blockProperties[7]);

            Item item = Inventory.Instance.CreativeItems.Find(item => item.Name == name && item.Type == type && item.Icon == icon);

            if (item == null) {
                item = new Item(name, type, icon, tier, 1, specialType);
                _ = Inventory.Instance.Add(item);
                item = Inventory.Instance.Items.Find(item => item.Name == name && item.Type == type && item.Tier == tier && item.Icon == icon);
                item.Slot.SetActive(false);
                Inventory.Instance.CreativeItems.Add(item);
                _ = Inventory.Instance.Items.Remove(item);
            }

            GameObject newBlock = Instantiate(Resources.Load<GameObject>("Blocks/" + type + "/" + icon), parent);
            newBlock.transform.SetLocalPositionAndRotation(position, rotation);
            ColorPicker.ApplyColorToBlock(newBlock.transform, color);
            newBlock.GetComponent<ItemBlock>().SetItem(item);

            if (SceneManager.GetActiveScene().name == "WarpScene") {
                if (enemyShip) {
                    newBlock.layer = 7;
                    newBlock.tag = "Enemy";
                } else {
                    newBlock.layer = 6;
                    newBlock.tag = "Player";
                }
            }
        }
    }

    public void UpdateTimeLeft(float timeLeft) => warpTimeLeft.text = Utility.ConvertSecondsToMinuteTimeFormat(timeLeft);

    public void UpdateLaserBar(float cooldown, float maxCooldown) {
        RectTransform barRect = laserBar.GetComponent<RectTransform>();
        ResizeBar(barRect, cooldown, maxCooldown);
        Image barImage = laserBar.GetComponent<Image>();
        barImage.color = ColorBar(cooldown, maxCooldown);
    }

    public void UpdateRepulsorBar(float cooldown, float maxCooldown) {
        RectTransform barRect = repulsorBar.GetComponent<RectTransform>();
        ResizeBar(barRect, cooldown, maxCooldown);
        Image barImage = repulsorBar.GetComponent<Image>();
        barImage.color = ColorBar(cooldown, maxCooldown);
    }

    public void UpdateTeleportBar(float cooldown, float maxCooldown) {
        RectTransform barRect = teleportBar.GetComponent<RectTransform>();
        ResizeBar(barRect, cooldown, maxCooldown);
        Image barImage = teleportBar.GetComponent<Image>();
        barImage.color = ColorBar(cooldown, maxCooldown);
    }

    public void UpdateRadarBar(float cooldown, float maxCooldown) {
        RectTransform barRect = radarBar.GetComponent<RectTransform>();
        ResizeBar(barRect, cooldown, maxCooldown);
        Image barImage = radarBar.GetComponent<Image>();
        barImage.color = ColorBar(cooldown, maxCooldown);
    }

    public void UpdateCloakBar(float cooldown, float maxCooldown) {
        RectTransform barRect = cloakBar.GetComponent<RectTransform>();
        ResizeBar(barRect, cooldown, maxCooldown);
        Image barImage = cloakBar.GetComponent<Image>();
        barImage.color = ColorBar(cooldown, maxCooldown);
    }

    public void UpdateEnergyBar(float energy, float maxEnergy) {
        RectTransform barRect = energyBar.GetComponent<RectTransform>();
        ResizeBar(barRect, energy, maxEnergy);
    }

    public void UpdateHealthBar(float health, float maxHealth) {
        RectTransform barRect = healthBar.GetComponent<RectTransform>();
        ResizeBar(barRect, health, maxHealth);
    }

    public void UpdateShieldBar(float power, float maxPower) {
        RectTransform barRect = shieldBar.GetComponent<RectTransform>();
        ResizeBar(barRect, power, maxPower);
    }

    private void ReassignInventory(bool isShop) {
        display = isShop ? shopInventory.Find("Display") : inventory.Find("Display");
        displayItem = display.transform.Find("Item Parent");
        displayInfo = displayItem.transform.Find("InfoBackground");
        displayName = displayInfo.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemStat1Label = displayInfo.Find("ItemStat1Label").GetComponent<TextMeshProUGUI>();
        itemStat1Value = displayInfo.Find("ItemStat1Value").GetComponent<TextMeshProUGUI>();
        itemStat2Label = displayInfo.Find("ItemStat2Label").GetComponent<TextMeshProUGUI>();
        itemStat2Value = displayInfo.Find("ItemStat2Value").GetComponent<TextMeshProUGUI>();
        itemStat3Label = displayInfo.Find("ItemStat3Label").GetComponent<TextMeshProUGUI>();
        itemStat3Value = displayInfo.Find("ItemStat3Value").GetComponent<TextMeshProUGUI>();
    }

    private Color ColorBar(float cooldown, float maxCooldown) {
        Color newColor = Color.red;

        if (cooldown >= maxCooldown) {
            newColor = new Color(65, 149, 124);
            SoundManager.Instance.PlaySound("Charged");
        }

        return newColor;
    }

    private void ResizeBar(RectTransform bar, float cooldown, float maxCooldown) {
        float cooldownPercentage = cooldown / maxCooldown;
        float newHeight = MAX_BAR_HEIGHT * cooldownPercentage;
        Vector2 newSize = new Vector2(bar.sizeDelta.x, newHeight);
        bar.sizeDelta = newSize;
    }

    private void UpdateMapPoints() {
        if (currentShipSelected == null) return;

        GameObject mapPoints = GameObject.Find("MapPoints");
        long shipPower = GetSelectedShipPower();

        if (mapPoints != null) {
            foreach (Transform point in mapPoints.transform) {
                if (point.name == "Point") {
                    Transform pointDifficulty = point.GetChild(0);
                    TextMeshProUGUI pointText = pointDifficulty.GetChild(0).GetComponent<TextMeshProUGUI>();

                    float multiplier = 0.2f;

                    if (pointDifficulty.name == "SuperEasy") {
                        multiplier = UnityEngine.Random.Range(0.2f, 0.6f);
                    } else if (pointDifficulty.name == "VeryEasy") {
                        multiplier = UnityEngine.Random.Range(0.6f, 1.0f);
                    } else if (pointDifficulty.name == "Easy") {
                        multiplier = UnityEngine.Random.Range(1.0f, 1.4f);
                    } else if (pointDifficulty.name == "Medium") {
                        multiplier = UnityEngine.Random.Range(1.4f, 1.8f);
                    } else if (pointDifficulty.name == "Difficult") {
                        multiplier = UnityEngine.Random.Range(1.8f, 2.2f);
                    } else if (pointDifficulty.name == "Hard") {
                        multiplier = UnityEngine.Random.Range(2.2f, 2.6f);
                    } else if (pointDifficulty.name == "VeryHard") {
                        multiplier = UnityEngine.Random.Range(2.6f, 3.0f);
                    } else if (pointDifficulty.name == "SuperHard") {
                        multiplier = UnityEngine.Random.Range(3.0f, 3.4f);
                    }

                    pointText.text = Mathf.Ceil(shipPower * multiplier).ToString();
                }
            }
        }
    }

    private void ShowList(List<string> creationList, bool isCreativeMode) {
        playerList.SetActive(!isCreativeMode);
        creativeList.SetActive(isCreativeMode);
        selectShipButton.SetActive(!isCreativeMode);

        Utility.ClearChildObjects(playerList.transform);
        Utility.ClearChildObjects(creativeList.transform);
        Utility.ClearChildObjects(shipContentList);
        Utility.ClearChildObjects(hangerDisplay);

        foreach (string creation in creationList) {
            GameObject entry = Instantiate(shipEntryPrefab, isCreativeMode ? creativeList.transform : playerList.transform);
            entry.transform.Find("ShipNameLabel").GetComponent<TextMeshProUGUI>().text = creation;
        }

        if (Settings.Instance.InCreativeMode() != isCreativeMode) Settings.Instance.ChangeCreativeMode();

        editShipButton.gameObject.SetActive(false);
        currentCreationSelected = string.Empty;
        shipNameLabel.text = string.Empty;
        shipStatNameLabel.text = string.Empty;

        if (!isCreativeMode) {
            GameObject.Find("ShipList").GetComponent<ScrollRect>().content = playerList.GetComponent<RectTransform>();
        } else {
            GameObject.Find("ShipList").GetComponent<ScrollRect>().content = creativeList.GetComponent<RectTransform>();
        }

        UpdateShipStats(string.Empty, null);
    }

    private void LoadShipsFromFile() {
        TextAsset shipDataFile = Resources.Load<TextAsset>("DefaultShips");

        if (shipDataFile != null) {
            string[] shipLines = shipDataFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in shipLines) {
                defaultShips.Add(line.Trim());
            }
        }
    }

    private void HideStoreUI() {
        Utility.Instance.DoIfNotMoving(moveDuration, () => {
            Utility.Instance.MoveTransform(inventoryBackbutton, -moveInventoryButtonDistance, 0, moveDuration);
            Utility.Instance.MoveTransform(shopPanel, moveInventoryDistance, 0, moveDuration);
            Utility.Instance.ScaleTransform(display, false, scaleDuration, false);
        });
    }

    private void HideCreationUI(bool useBlackScreen = false) {
        Utility.Instance.DoIfNotMoving(3, () => {
            Utility.Instance.MoveTransform(shipStats, 0, moveShipStatsDistance, moveDuration);
            Utility.Instance.MoveTransform(hangerBackButton, -moveHangerBackButtonDistance, 0, moveDuration);
            Utility.Instance.MoveTransform(editShipButton, moveEditShipButtonDistance, 0, moveDuration);
            Utility.Instance.ScaleTransform(shipMenu, false, scaleDuration);

            if (useBlackScreen) {
                Utility.Instance.FadeCanvasGroup(blackScreen, 2, true, moveDuration);
            }
        });
    }

    private void HideMapUI() {
        Utility.Instance.DoIfNotMoving(2, () => {
            Utility.Instance.MoveTransform(mapBackButton, -moveMapBackButtonDistance, 0, moveDuration, 1);
            Utility.Instance.MoveTransform(jumpInfo, 0, moveJumpInfoDistance, moveDuration, 1);
            Utility.Instance.MoveTransform(currentJumpInfo, 0, moveCurrentJumpInfoDistance, moveDuration);
            Utility.Instance.ScaleTransform(jumpBackground, false, scaleDuration, true);
            Utility.Instance.MoveTransform(jumpTitle, -moveJumpTitleDistance, 0, moveDuration);
            Utility.Instance.MoveTransform(selectJumpButton, moveSelectJumpButtonDistance, 0, moveDuration);
        });
    }

    private void HideJumpUI() {
        Utility.Instance.DoIfNotMoving(moveDuration + 4, () => {
            Utility.Instance.MoveTransform(jumpBackButton, -moveJumpBackButtonDistance, 0, moveDuration);
            Utility.Instance.MoveTransform(jumpButton, 0, moveJumpButtonDistance, moveDuration);
            Utility.Instance.MoveTransform(shipStats, 0, moveShipStatsDistance, moveDuration);
            Utility.Instance.MoveTransform(currentJumpInfo, 0, moveCurrentJumpInfoDistance, moveDuration);
        });
    }

    private void HideMainUI() {
        Utility.Instance.DoIfNotMoving(moveDuration, () => {
            Utility.Instance.MoveTransform(mainMenu, 0, moveMainMenuDistance, moveDuration);
            Utility.Instance.ScaleTransform(logo, false, scaleDuration, false);
        });
    }

    private void UpdateVariables() {
        if (GameObject.Find("CameraPaths")) cameraTrack = GameObject.Find("CameraPaths").GetComponent<CameraTrack>();
        if (GameObject.Find("JumpDisplay")) jumpDisplay = GameObject.Find("JumpDisplay").transform;
        if (GameObject.Find("HangerDisplay")) hangerDisplay = GameObject.Find("HangerDisplay").transform;
        if (GameObject.Find("PlayerShip")) playerShip = GameObject.Find("PlayerShip").transform;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        transform.SetPositionAndRotation(new Vector3(0, 500, 0), transform.rotation);

        UpdateVariables();

        if (scene.name == "MainHub") {
            shipCreation.SetActive(false);
            mainHub.SetActive(true);
            warpScene.SetActive(false);

            string creationSave = Load.String(currentCreationSelected);

            if (!string.IsNullOrEmpty(creationSave)) {
                int separatorIndex = creationSave.IndexOf('/');
                creationSave = creationSave[(separatorIndex + 1)..];
                LoadCreation(creationSave, hangerDisplay);
            }

            string selectedShipSave = Load.String(currentShipSelected);

            if (!string.IsNullOrEmpty(selectedShipSave)) {
                int separatorIndex = selectedShipSave.IndexOf('/');
                selectedShipSave = selectedShipSave[(separatorIndex + 1)..];
                LoadCreation(selectedShipSave, jumpDisplay);
            }
        } else if (scene.name == "ShipCreation") {
            ToggleBehaviorUI(false);
            shipCreation.SetActive(true);
            mainHub.SetActive(false);
            warpScene.SetActive(false);

            string creationSave = Load.String(currentCreationSelected);
            bool creativeMode = Settings.Instance.InCreativeMode();

            if (string.IsNullOrEmpty(creationSave)) {
                ShipCreation.Instance.ShowInstansiationParent(creativeMode);
            } else {
                int separatorIndex = creationSave.IndexOf('|');
                creationSave = creationSave[(separatorIndex + 1)..];
                separatorIndex = creationSave.IndexOf('/');
                string behaviorsString = creationSave[..separatorIndex];
                string[] behaviors = behaviorsString.Split(':', System.StringSplitOptions.RemoveEmptyEntries);

                spawnBehavior.value = int.Parse(behaviors[0]);
                moveBehavior.value = int.Parse(behaviors[1]);
                faceBehavior.value = int.Parse(behaviors[2]);
                dodgeBehavior.value = int.Parse(behaviors[3]);
                aimBehavior.value = int.Parse(behaviors[4]);
                distanceBehavior.text = behaviors[5];
                minScaleBehavior.text = behaviors[6];
                maxScaleBehavior.text = behaviors[7];
                minSpeedBehavior.text = behaviors[8];
                maxSpeedBehavior.text = behaviors[9];

                creationSave = creationSave[(separatorIndex + 1)..];

                if (creationParent == null || creativeParent == null) {
                    ShipCreation.Instance.ShowInstansiationParent(creativeMode);
                    creationParent = ShipCreation.Instance.GetCreationParent();
                    creativeParent = ShipCreation.Instance.GetCreativeParent();
                }

                Transform currentParent = creativeMode ? creativeParent : creationParent;
                LoadCreation(creationSave, currentParent);
                ShipCreation.Instance.ShowInstansiationParent(creativeMode);
                Bounds bounds = Utility.GetTransformBounds(currentParent);
                GameObject.Find("PlayerFloating").transform.position = bounds.center - (bounds.size / 1.5f);
            }
        } else if (scene.name == "WarpScene") {
            shipCreation.SetActive(false);
            mainHub.SetActive(false);
            warpScene.SetActive(true);

            string selectedShipSave = Load.String(currentShipSelected);

            if (!string.IsNullOrEmpty(selectedShipSave)) {
                int separatorIndex = selectedShipSave.IndexOf('/');
                selectedShipSave = selectedShipSave[(separatorIndex + 1)..];
                LoadCreation(selectedShipSave, playerShip, false);
            }
        }
    }

    private void UpdateShipStats(string shipName, Transform parentDisplay) {
        float healthTotal = 0;
        float energyTotal = 0;
        float shieldTotal = 0;
        float teleportDistTotal = 0;
        float pushDistTotal = 0;
        float maxSizeTotal = 20;
        float maxSpecialsTotal = 1;
        float radarDistTotal = 0;
        float dpsTotal = 0;
        float pickupDistTotal = 0;
        float cloakTimeTotal = 0;
        float healthRegenTotal = 0;
        float energyRegenTotal = 0;
        float shieldRegenTotal = 0;
        float speedTotal = 0;
        float jumpSpeedTotal = 0;
        long creationTotalPower = 0;

        if (parentDisplay != null) {
            foreach (Transform block in parentDisplay) {
                Item blockItem = block.gameObject.GetComponent<ItemBlock>().GetItem();
                long tier = blockItem.Tier;
                int blockGrade = blockItem.GetGrade();

                healthTotal += tier * 50 * blockGrade;
                creationTotalPower += Inventory.GetBlockPower(blockItem.Name, tier, blockGrade);

                switch (blockItem.Name) {
                    case "Bridge":
                        maxSizeTotal += Inventory.GetBlockParameter("maxSizeTotal", tier, blockGrade);
                        break;
                    case "Cannon":
                        dpsTotal += Inventory.GetBlockParameter("dpsCannonTotal", tier, blockGrade);
                        break;
                    case "Missile Launcher":
                        dpsTotal += Inventory.GetBlockParameter("dpsMissileTotal", tier, blockGrade);
                        break;
                    case "Railgun":
                        dpsTotal += Inventory.GetBlockParameter("dpsRailgunTotal", tier, blockGrade);
                        break;
                    case "Engine":
                        speedTotal += Inventory.GetBlockParameter("speedTotal", tier, blockGrade);
                        break;
                    case "Fusion Core":
                        energyRegenTotal += Inventory.GetBlockParameter("energyRegenTotal", tier, blockGrade);
                        break;
                    case "Energy Storage":
                        energyTotal += Inventory.GetBlockParameter("energyTotal", tier, blockGrade);
                        break;
                    case "Laser":
                        dpsTotal += Inventory.GetBlockParameter("dpsLaserTotal", tier, blockGrade);
                        break;
                    case "Repulsor":
                        pushDistTotal += Inventory.GetBlockParameter("pushDistTotal", tier, blockGrade);
                        break;
                    case "Minigun":
                        dpsTotal += Inventory.GetBlockParameter("dpsMinigunTotal", tier, blockGrade);
                        break;
                    case "Advanced Bridge":
                        maxSpecialsTotal += Inventory.GetBlockParameter("maxSpecialsTotal", tier, blockGrade);
                        break;
                    case "Cloaking Device":
                        cloakTimeTotal += Inventory.GetBlockParameter("cloakTimeTotal", tier, blockGrade);
                        break;
                    case "Magnet":
                        if (pickupDistTotal == 0) pickupDistTotal = 10;
                        pickupDistTotal += Inventory.GetBlockParameter("pickupDistTotal", tier, blockGrade);
                        break;
                    case "Quantum Drive":
                        teleportDistTotal += Inventory.GetBlockParameter("teleportDistTotal", tier, blockGrade);
                        break;
                    case "Radar":
                        radarDistTotal += Inventory.GetBlockParameter("radarDistTotal", tier, blockGrade);
                        break;
                    case "Repair Module":
                        healthRegenTotal += Inventory.GetBlockParameter("healthRegenTotal", tier, blockGrade);
                        break;
                    case "Shield Generator":
                        shieldTotal += Inventory.GetBlockParameter("shieldTotal", tier, blockGrade);
                        shieldRegenTotal += Inventory.GetBlockParameter("shieldRegenTotal", tier, blockGrade);
                        break;
                    case "Turbo Engine":
                        jumpSpeedTotal += Inventory.GetBlockParameter("jumpSpeedTotal", tier, blockGrade);
                        break;
                }
            }
        }

        cloakTimeTotal = Mathf.Clamp(cloakTimeTotal, 3f, 15f);

        shipStatNameLabel.text = shipName + " | Power - " + creationTotalPower;
        shipStatHealthLabel.text = "Health - " + healthTotal;
        shipStatHealthRegenLabel.text = "H Regen - " + healthRegenTotal;
        shipStatEnergyLabel.text = "Energy - " + energyTotal;
        shipStatEnergyRegenLabel.text = "E Regen - " + energyRegenTotal;
        shipStatShieldLabel.text = "Shield - " + shieldTotal;
        shipStatShieldRegenLabel.text = "S Regen - " + shieldRegenTotal;
        shipStatTeleportLabel.text = "Tele Dist - " + teleportDistTotal;
        shipStatPushLabel.text = "Push Dist - " + pushDistTotal;
        shipStatSizeLabel.text = "Max Size - " + maxSizeTotal;
        shipStatSpecialsLabel.text = "Max Specials - " + maxSpecialsTotal;
        shipStatCloakTimeLabel.text = "Cloak Time - " + cloakTimeTotal;
        shipStatRadarDistLabel.text = "Radar Dist - " + radarDistTotal;
        shipStatDPSLabel.text = "DPS - " + dpsTotal;
        shipStatSpeedLabel.text = "Speed - " + speedTotal;
        shipStatJumpSpeedLabel.text = "Jump Speed - " + jumpSpeedTotal;
        shipStatPickupDistLabel.text = "Pickup Dist - " + pickupDistTotal;
    }
}
