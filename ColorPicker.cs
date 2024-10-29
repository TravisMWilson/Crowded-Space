using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour {
    private static Color useSelected = new Color(0, 0, 0, 0);
    private static Color selectedColor = Color.white;

    [SerializeField] private Image colorWheelImage;
    [SerializeField] private Transform displayIcon;
    [SerializeField] private Image marker;
    [SerializeField] private Camera uiCamera;

    private Texture2D colorWheelTexture;

    void Start() {
        colorWheelTexture = colorWheelImage.sprite.texture;
        marker.gameObject.SetActive(false);
    }

    void OnMouseDown() {
        selectedColor = GetPixelColor();
        ApplyColorToBlock(displayIcon.GetChild(0), selectedColor);

        marker.rectTransform.anchoredPosition = GetMouseLocalPosition();
        marker.color = selectedColor;
        marker.gameObject.SetActive(true);
    }

    public static void ApplyColorToBlock(Transform block, Color color) {
        if (color == useSelected) color = selectedColor;

        if (block.TryGetComponent<Renderer>(out var blockRenderer)) {
            blockRenderer.material = new Material(blockRenderer.material);
            color.a = blockRenderer.material.color.a;

            if (block.name.Contains("Hull")) blockRenderer.material.color = color;

            Utility.ApplyToAllChildren(block, (Transform child) => {
                if (child.TryGetComponent<Renderer>(out var renderer) && !renderer.material.name.Contains("FX")) {
                    color.a = renderer.material.color.a;

                    if (child.name.Contains("Hull")) renderer.material.color = color;
                }
            });
        }
    }

    private Color GetPixelColor() {
        Vector2 normalizedPosition = GetMouseLocalPosition();

        float rectWidth = colorWheelImage.rectTransform.rect.width;
        float rectHeight = colorWheelImage.rectTransform.rect.height;

        float normalizedX = (normalizedPosition.x + (rectWidth / 2)) / rectWidth * 2048;
        float normalizedY = (normalizedPosition.y + (rectHeight / 2)) / rectHeight * 2048;

        int x = Mathf.RoundToInt(normalizedX);
        int y = Mathf.RoundToInt(normalizedY);

        x = Mathf.Clamp(x, 0, colorWheelTexture.width - 1);
        y = Mathf.Clamp(y, 0, colorWheelTexture.height - 1);

        return colorWheelTexture.GetPixel(x, y);
    }

    private Vector2 GetMouseLocalPosition() {
        _ = RectTransformUtility.ScreenPointToLocalPointInRectangle(colorWheelImage.rectTransform, Input.mousePosition, uiCamera, out Vector2 localPoint);
        return new Vector2(localPoint.x, localPoint.y);
    }
}
