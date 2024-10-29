using UnityEngine;

public class MouseFollower : MonoBehaviour {
    private RectTransform rectTransform;

    void Start() => rectTransform = GetComponent<RectTransform>();
    void Update() => FollowMouse();

    private void FollowMouse() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        rectTransform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
    }
}
