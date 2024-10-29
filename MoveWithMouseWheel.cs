using UnityEngine;

public class MoveWithMouseWheel : MonoBehaviour {
    [SerializeField] private float moveSpeed;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    void Update() {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel != 0) {
            Vector3 newPosition = transform.position;
            newPosition.y -= mouseWheel * moveSpeed;
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            transform.position = newPosition;
        }
    }
}
