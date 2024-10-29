using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DistanceDisplay : MonoBehaviour {
    private const float FEET_PER_UNIT = 16f;
    private const float METERS_PER_FOOT = 0.3048f;
    private const float METERS_PER_KILOMETER = 1000f;

    [SerializeField] private TextMeshProUGUI distanceTextPrefab;
    [SerializeField] private Image dotPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform playerShip;
    [SerializeField] private Transform enemiesParent;

    private Camera mainCamera;

    private Color orangeColor = new Color(1f, 0.5f, 0f);
    private Color redColor = new Color(1f, 0.135f, 0f);
    private Color blueColor = new Color(0f, 0.165f, 1f);

    private Color darkerOrangeColor = new Color(0.6f, 0.3f, 0f);
    private Color darkerRedColor = new Color(0.6f, 0.08f, 0f);
    private Color darkerBlueColor = new Color(0.065f, 0.165f, 0.5f);

    private Dictionary<Transform, (TextMeshProUGUI, Image)> enemyUIDictionary = new Dictionary<Transform, (TextMeshProUGUI, Image)>();

    void Start() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
    }

    void Update() {
        if (playerShip.childCount == 1) {
            DestroyAllDistanceIndicators();
            enabled = false;
            return;
        }

        List<Transform> currentEnemies = new List<Transform>();
        foreach (Transform enemy in enemiesParent) {
            currentEnemies.Add(enemy);

            if (!enemyUIDictionary.ContainsKey(enemy)) {
                TextMeshProUGUI tempDistanceTextInstance = Instantiate(distanceTextPrefab, canvas.transform);
                Image tempDotInstance = Instantiate(dotPrefab, canvas.transform);
                enemyUIDictionary[enemy] = (tempDistanceTextInstance, tempDotInstance);
            }

            var (distanceTextInstance, dotInstance) = enemyUIDictionary[enemy];

            UpdateDistanceText(enemy, distanceTextInstance);

            if (IsObjectVisible(enemy)) {
                UpdateTextPosition(enemy, distanceTextInstance);
                dotInstance.gameObject.SetActive(false);
                distanceTextInstance.gameObject.SetActive(true);
            } else {
                PositionTextAtScreenEdge(enemy, distanceTextInstance, dotInstance);
                distanceTextInstance.gameObject.SetActive(true);
                dotInstance.gameObject.SetActive(true);
            }
        }

        List<Transform> enemiesToRemove = new List<Transform>();
        foreach (var enemy in enemyUIDictionary.Keys) {
            if (!currentEnemies.Contains(enemy)) {
                enemiesToRemove.Add(enemy);
                Destroy(enemyUIDictionary[enemy].Item1.gameObject);
                Destroy(enemyUIDictionary[enemy].Item2.gameObject);
            }
        }

        foreach (var enemy in enemiesToRemove) {
            _ = enemyUIDictionary.Remove(enemy);
        }
    }

    private void DestroyAllDistanceIndicators() {
        foreach (var enemy in enemyUIDictionary.Keys) {
            Destroy(enemyUIDictionary[enemy].Item1.gameObject);
            Destroy(enemyUIDictionary[enemy].Item2.gameObject);
        }

        enemyUIDictionary.Clear();
    }

    private bool IsObjectVisible(Transform obj) {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }

    private void UpdateDistanceText(Transform enemy, TextMeshProUGUI textInstance) {
        float distanceMagnitude = Vector3.Distance(enemy.position, playerShip.position);
        float distanceInFeet = distanceMagnitude * FEET_PER_UNIT;
        float distanceInMeters = distanceInFeet * METERS_PER_FOOT;

        bool isBehind = Vector3.Dot(playerShip.forward, enemy.position - playerShip.position) < 0;

        if (distanceInMeters < METERS_PER_KILOMETER) {
            int roundedDistance = Mathf.RoundToInt(distanceInMeters / 10f) * 10;
            textInstance.text = roundedDistance.ToString() + " m";

            Transform shipBridge = enemy.transform.Find("Bridge1(Clone)");

            if (shipBridge != null) {
                textInstance.color = isBehind ? darkerBlueColor : blueColor;
            } else {
                textInstance.color = isBehind ? darkerRedColor : redColor;
            }
        } else {
            float distanceInKilometers = distanceInMeters / METERS_PER_KILOMETER;
            int roundedDistance = Mathf.RoundToInt(distanceInKilometers);
            textInstance.text = roundedDistance.ToString() + " km";
            textInstance.color = isBehind ? darkerOrangeColor : orangeColor;
        }

        textInstance.fontSize = isBehind ? 8f : 15f;
    }

    private void UpdateTextPosition(Transform enemy, TextMeshProUGUI textInstance) {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(enemy.position);
        textInstance.transform.position = screenPosition;
    }

    private void PositionTextAtScreenEdge(Transform enemy, TextMeshProUGUI textInstance, Image dotInstance) {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(enemy.position);

        Vector3 directionToEnemy = enemy.position - playerShip.position;
        bool isBehind = Vector3.Dot(playerShip.forward, directionToEnemy) < 0;

        screenPosition.x = Mathf.Clamp(screenPosition.x, 0f, 1f);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0f, 1f);

        Vector3 edgePosition = mainCamera.ViewportToScreenPoint(screenPosition);

        float textOffset = 40f;
        float dotOffset = 20f;

        Vector3 dotPosition = edgePosition;

        if (screenPosition.x == 0) {
            dotPosition.x += dotOffset;
        } else if (screenPosition.x == 1) {
            dotPosition.x -= dotOffset;
        }

        if (screenPosition.y == 0) {
            dotPosition.y += dotOffset;
        } else if (screenPosition.y == 1) {
            dotPosition.y -= dotOffset;
        }

        if (isBehind) {
            dotPosition.x = Screen.width - dotPosition.x;
            dotPosition.y = Screen.height - dotPosition.y;
        }

        dotInstance.transform.position = dotPosition;

        Vector3 textPosition = edgePosition;

        if (screenPosition.x == 0) {
            textPosition.x += textOffset;
        } else if (screenPosition.x == 1) {
            textPosition.x -= textOffset;
        }

        if (screenPosition.y == 0) {
            textPosition.y += textOffset;
        } else if (screenPosition.y == 1) {
            textPosition.y -= textOffset;
        }

        if (isBehind) {
            textPosition.x = Screen.width - textPosition.x;
            textPosition.y = Screen.height - textPosition.y;
        }

        textInstance.transform.position = textPosition;

        textInstance.fontSize = isBehind ? 8f : 15f;

        Transform shipBridge = enemy.transform.Find("Bridge1(Clone)");

        if (shipBridge != null) {
            textInstance.color = isBehind ? darkerBlueColor : blueColor;
            dotInstance.color = isBehind ? darkerBlueColor : blueColor;
        } else {
            textInstance.color = isBehind ? darkerRedColor : redColor;
            dotInstance.color = isBehind ? darkerRedColor : redColor;
        }
    }
}
