using System.Collections;
using UnityEngine;

public class ColorPickerButton : MonoBehaviour {
    [SerializeField] private Material selected;
    [SerializeField] private Material unselected;
    [SerializeField] private RectTransform colorPickerPanel;
    [SerializeField] private float moveDuration = 1.0f;

    private Vector2 colorPickerPanelStartPos;
    private Vector2 colorPickerPanelEndPos;

    private static bool isVisible = false;
    private bool isMoving = false;

    void Start() {
        colorPickerPanelEndPos = colorPickerPanel.anchoredPosition;
        colorPickerPanelStartPos = new Vector2(500, colorPickerPanelEndPos.y);

        colorPickerPanel.anchoredPosition = colorPickerPanelStartPos;
    }

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");

        if (isMoving) return;

        if (Inventory.Instance.GetSelectedItemBlock() == null) {
            PopupManager.Instance.ShowPopup("You don't have a block selected yet. Open your inventory by pressing the four square icon on the menu at the top of the screen. " +
                "Then select a block from your inventory and press the pulsing button below the big display.", PopupType.OK);
            return;
        }

        isMoving = true;

        _ = isVisible
            ? StartCoroutine(MovePanel(colorPickerPanel, colorPickerPanelEndPos, colorPickerPanelStartPos))
            : StartCoroutine(MovePanel(colorPickerPanel, colorPickerPanelStartPos, colorPickerPanelEndPos));

        isVisible = !isVisible;

        GetComponent<Renderer>().material = isVisible ? selected : unselected;
    }

    private IEnumerator MovePanel(RectTransform panel, Vector2 startPos, Vector2 endPos) {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            float sineT = Mathf.Sin(t * Mathf.PI * 0.5f);

            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, sineT);
            yield return null;
        }

        panel.anchoredPosition = endPos;
        isMoving = false;
    }

    public static bool IsColorPickerOpen() => isVisible;
}
