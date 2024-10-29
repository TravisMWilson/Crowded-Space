using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeUpdater : MonoBehaviour {
    [SerializeField] private Transform displayIcon;

    private Color USE_SELECTED = new Color(0, 0, 0, 0);

    private Transform creationParent;
    private Transform creativeParent;

    void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;
    void Start() => SetParents();

    public void UpdateBridge() {
        if (displayIcon.GetChild(0) == null) return;

        Material newMaterial = new Material(displayIcon.GetChild(0).GetComponent<Renderer>().material);
        Transform currentParent = Settings.Instance.InCreativeMode() ? creativeParent : creationParent;

        if (newMaterial.name.Contains("Invisible")) {
            newMaterial = new Material(displayIcon.GetChild(0).Find("Hull").GetComponent<Renderer>().material);
        }

        foreach (Transform block in currentParent) {
            if (block.name.Contains("Bridge") && !block.name.Contains("Adv")) {
                Utility.ApplyToAllChildren(block, (Transform child) => {
                    if (child.TryGetComponent<Renderer>(out var renderer)) {
                        if (child.name.Contains("Hull")) {
                            Color color = renderer.material.color;
                            renderer.material = newMaterial;
                            renderer.material.color = color;
                        }
                    }
                });

                ColorPicker.ApplyColorToBlock(block, USE_SELECTED);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SetParents();

    private void SetParents() {
        if (!GameObject.Find("Creation")) return;

        creationParent = ShipCreation.Instance.GetCreationParent();
        creativeParent = ShipCreation.Instance.GetCreativeParent();
    }
}
