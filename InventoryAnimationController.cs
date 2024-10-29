using UnityEngine;

public class InventoryAnimationController : MonoBehaviour {
    [SerializeField] private CanvasGroup background;
    [SerializeField] private RectTransform sidePanel;
    [SerializeField] private RectTransform display;
    [SerializeField] private RectTransform topDecor;
    [SerializeField] private RectTransform bottomDecor;

    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float moveDuration = 1.0f;

    private Vector2 sidePanelStartPos;
    private Vector2 sidePanelEndPos;
    private Vector2 displayStartPos;
    private Vector2 displayEndPos;
    private Vector2 topDecorStartPos;
    private Vector2 topDecorEndPos;
    private Vector2 bottomDecorStartPos;
    private Vector2 bottomDecorEndPos;

    private static bool isVisible = false;

    void Start() {
        sidePanelEndPos = sidePanel.anchoredPosition;
        sidePanelStartPos = new Vector2(-800, sidePanelEndPos.y);

        displayEndPos = display.anchoredPosition;
        displayStartPos = new Vector2(3100, displayEndPos.y);

        topDecorEndPos = topDecor.anchoredPosition;
        topDecorStartPos = new Vector2(topDecorEndPos.x, 450);

        bottomDecorEndPos = bottomDecor.anchoredPosition;
        bottomDecorStartPos = new Vector2(bottomDecorEndPos.x, -400);

        sidePanel.anchoredPosition = sidePanelStartPos;
        display.anchoredPosition = displayStartPos;
        topDecor.anchoredPosition = topDecorStartPos;
        bottomDecor.anchoredPosition = bottomDecorStartPos;
        background.alpha = 0f;
    }

    void OnMouseDown() {
        Utility.Instance.DoIfNotMoving(moveDuration, () => {
            if (isVisible) {
                Utility.Instance.FadeCanvasGroup(background, fadeDuration);
                Utility.Instance.MoveTransform(sidePanel, sidePanelEndPos, sidePanelStartPos, moveDuration);
                Utility.Instance.MoveTransform(display, displayEndPos, displayStartPos, moveDuration);
                Utility.Instance.MoveTransform(topDecor, topDecorEndPos, topDecorStartPos, moveDuration);
                Utility.Instance.MoveTransform(bottomDecor, bottomDecorEndPos, bottomDecorStartPos, moveDuration);
            } else {
                Utility.Instance.FadeCanvasGroup(background, fadeDuration, true);
                Utility.Instance.MoveTransform(sidePanel, sidePanelStartPos, sidePanelEndPos, moveDuration);
                Utility.Instance.MoveTransform(display, displayStartPos, displayEndPos, moveDuration);
                Utility.Instance.MoveTransform(topDecor, topDecorStartPos, topDecorEndPos, moveDuration);
                Utility.Instance.MoveTransform(bottomDecor, bottomDecorStartPos, bottomDecorEndPos, moveDuration);
            }

            isVisible = !isVisible;
        });
    }

    public static bool IsInventoryOpen() => isVisible;
}
