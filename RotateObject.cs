using UnityEngine;

public class RotateObject : MonoBehaviour {
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 45, 20);
    void Update() => transform.Rotate(rotationSpeed * Time.deltaTime);
}
