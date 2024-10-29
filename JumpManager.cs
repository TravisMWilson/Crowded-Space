using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum DifficultyLevel {
    SuperEasy = 0,
    VeryEasy = 1,
    Easy = 2,
    Medium = 3,
    Difficult = 4,
    Hard = 5,
    VeryHard = 6,
    SuperHard = 7
}

public class JumpManager : MonoBehaviour {
    public static JumpManager Instance;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyParent;

    private List<string> creativeCreationList = new List<string>();
    private List<long> creativeCreationPower = new List<long>();

    private Transform playerShip;
    CanvasGroup whiteScreen;

    private int jumpDifficulty;
    private int maxDifficultyLevel = 7;
    private float jumpTime;
    private long jumpPower;

    private float spawnRate = 1f;
    private float spawnTimer = 1f;
    private bool isTraveling = false;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        string[] jumpInfo = UIController.Instance.GetSelectedJumpInfo().Split(":", System.StringSplitOptions.RemoveEmptyEntries);

        jumpDifficulty = GetDifficultyAsInt(jumpInfo[0]);
        jumpTime = float.Parse(jumpInfo[1]) * 60;
        jumpPower = long.Parse(jumpInfo[2]);

        spawnRate = Mathf.Lerp(3f, 1f, (float)jumpDifficulty / maxDifficultyLevel);

        playerShip = GameObject.Find("PlayerShip").transform;
        whiteScreen = GameObject.Find("WhiteScreen").GetComponent<CanvasGroup>();

        creativeCreationList = UIController.Instance.GetCreativeCreationList();

        List<string> filteredCreativeCreationList = new List<string>();
        List<long> filteredCreativeCreationPower = new List<long>();

        foreach (string save in creativeCreationList) {
            long creationPower = GetPower(save);

            if (creationPower <= jumpPower && creationPower > 0) {
                filteredCreativeCreationList.Add(save);
                filteredCreativeCreationPower.Add(creationPower);
            }
        }

        creativeCreationList = filteredCreativeCreationList;
        creativeCreationPower = filteredCreativeCreationPower;
    }

    void Update() {
        jumpTime -= Time.deltaTime;
        UIController.Instance.UpdateTimeLeft(jumpTime);

        if (jumpTime <= 0 && !isTraveling) {
            ChangeScene.Instance.GoToScene("MainHub");
            Utility.Instance.FadeCanvasGroup(whiteScreen, 1.0f, false, 0.5f);
            Utility.Instance.Delay(0.4f, () => UIController.Instance.ShowMain());
            Cursor.lockState = CursorLockMode.None;
            isTraveling = true;
            return;
        }
        
        if (spawnTimer > 0) {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer < 0) {
                int selectedIndex = GetWeightedRandomIndex();
                SpawnEnemy(selectedIndex);
                spawnTimer = spawnRate;
            }
        }

        if (playerShip.childCount <= 1 && jumpTime > 5f) {
            jumpTime = 5f;
        }

        if (jumpTime <= 5f) {
            whiteScreen.alpha = 1 - (jumpTime / 10);
        }
    }

    private int GetDifficultyAsInt(string difficulty) => Enum.TryParse(difficulty.Replace(" ", ""), true, out DifficultyLevel level) ? (int)level : 0;

    private int GetWeightedRandomIndex() {
        float totalWeight = 0f;
        float[] weights = new float[creativeCreationPower.Count];

        for (int i = 0; i < creativeCreationPower.Count; i++) {
            weights[i] = 1f / Mathf.Max(1, creativeCreationPower[i]);
            totalWeight += weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; i++) {
            cumulativeWeight += weights[i];

            if (randomValue <= cumulativeWeight) {
                return i;
            }
        }

        return 0;
    }

    private void SpawnEnemy(int index) {
        string randomCreation = creativeCreationList[index];

        GameObject enemy = Instantiate(enemyPrefab, enemyParent);
        CreationBehavior enemyBehavior = enemy.GetComponent<CreationBehavior>();
        string creationSave = Load.String(randomCreation);

        if (!string.IsNullOrEmpty(creationSave)) {
            int separatorIndex = creationSave.IndexOf('|');
            creationSave = creationSave[(separatorIndex + 1)..];
            separatorIndex = creationSave.IndexOf('/');
            string behaviorsString = creationSave[..separatorIndex];
            creationSave = creationSave[(separatorIndex + 1)..];
            string[] behaviors = behaviorsString.Split(':', System.StringSplitOptions.RemoveEmptyEntries);

            if (behaviors.Length >= 5) {
                enemyBehavior.SetSpawnBehavior(int.Parse(behaviors[0]));
                enemyBehavior.SetMoveBehavior(int.Parse(behaviors[1]));
                enemyBehavior.SetFaceBehavior(int.Parse(behaviors[2]));
                enemyBehavior.SetDodgeBehavior(int.Parse(behaviors[3]));
                enemyBehavior.SetAimBehavior(int.Parse(behaviors[4]));
                enemyBehavior.SetDistance(float.Parse(behaviors[5]));
                enemyBehavior.SetScaleMinimun(float.Parse(behaviors[6]));
                enemyBehavior.SetScaleMaximun(float.Parse(behaviors[7]));
                enemyBehavior.SetSpeedMinimun(float.Parse(behaviors[8]));
                enemyBehavior.SetSpeedMaximun(float.Parse(behaviors[9]));

                enemyBehavior.MoveToSpawnLocation();

                UIController.Instance.LoadCreation(creationSave, enemy.transform);
            }
        }
    }

    private long GetPower(string save) {
        long power = 0;

        string selectedShipSave = Load.String(save);

        if (!string.IsNullOrEmpty(selectedShipSave)) {
            int separatorIndex = selectedShipSave.IndexOf('/');
            selectedShipSave = selectedShipSave[(separatorIndex + 1)..];
            string[] blocks = selectedShipSave.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string block in blocks) {
                string[] blockProperties = block.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
                long tier = long.Parse(blockProperties[3]);
                string icon = blockProperties[4];
                string name = blockProperties[0];
                int grade = 1;

                if (icon.StartsWith("Hull")) {
                    Match match = Regex.Match(icon, @"^Hull(\d+)");

                    if (match.Success) {
                        grade = int.Parse(match.Groups[1].Value);
                    }
                } else {
                    Match match = Regex.Match(icon, @"\d+$");

                    if (match.Success) {
                        grade = int.Parse(match.Value);
                    }
                }

                power += Inventory.GetBlockPower(name, tier, grade);
            }
        }

        return power;
    }
}
