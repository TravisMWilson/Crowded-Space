using UnityEngine;

public class SettingAnimationController : MonoBehaviour {
    [SerializeField] private RectTransform settingPanel;
    [SerializeField] private float moveDuration = 1.0f;

    private Vector2 settingPanelStartPos;
    private Vector2 settingPanelEndPos;

    private static bool isVisible = false;

    void Start() {
        settingPanelEndPos = settingPanel.anchoredPosition;
        settingPanelStartPos = new Vector2(settingPanelEndPos.x, - 1800);

        settingPanel.anchoredPosition = settingPanelStartPos;
    }

    void OnMouseDown() {
        Utility.Instance.DoIfNotMoving(moveDuration, () => {
            if (isVisible) {
                Utility.Instance.MoveTransform(settingPanel, settingPanelEndPos, settingPanelStartPos, moveDuration);
            } else {
                Utility.Instance.MoveTransform(settingPanel, settingPanelStartPos, settingPanelEndPos, moveDuration);
            }

            isVisible = !isVisible;
        });
    }

    public static bool IsSettingsOpen() => isVisible;
}
