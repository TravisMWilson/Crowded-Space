using UnityEngine;
using UnityEngine.SceneManagement;

public class AlphaFadeOnCameraProximity : MonoBehaviour {
    [SerializeField] private string fadeObjectName;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;

    private Transform fadeObject;
    private CanvasGroup canvasGroup;
    private Camera mainCamera;

    void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainHub") {
            fadeObject = GameObject.Find(fadeObjectName).transform;
            mainCamera = Camera.main;

            if (this != null && gameObject != null) canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    void Update() {
        if (canvasGroup == null || mainCamera == null || fadeObject == null) return;

        float distance = Vector3.Distance(mainCamera.transform.position, fadeObject.position);
        float alpha = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        canvasGroup.alpha = alpha;
    }
}
