using System.Collections;
using UnityEngine;

public class TopBarButton : MonoBehaviour {
    [SerializeField] private GameObject button;
    [SerializeField] private RectTransform bar;

    [SerializeField] private float moveAmount = 500f;
    [SerializeField] private float duration = 0.5f;

    private bool isFlipped = false;
    private bool isMoving = false;

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");

        if (isMoving) return;
        isMoving = true;

        float rotationAngle = isFlipped ? 0f : 180f;
        button.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        float newY = isFlipped ? bar.anchoredPosition.y - moveAmount : bar.anchoredPosition.y + moveAmount;
        _ = StartCoroutine(MoveBar(newY));

        isFlipped = !isFlipped;
    }

    private IEnumerator MoveBar(float targetY) {
        Vector2 startPos = bar.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float sineT = Mathf.Sin(t * Mathf.PI * 0.5f);

            bar.anchoredPosition = Vector2.Lerp(startPos, targetPos, sineT);
            yield return null;
        }

        bar.anchoredPosition = targetPos;
        isMoving = false;
    }
}
