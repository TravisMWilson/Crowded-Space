using UnityEngine;

public class BillboardFX : MonoBehaviour {
    [SerializeField] private Transform camTransform;

    private Quaternion originalRotation;

    void Start() => originalRotation = transform.rotation;
    void Update() => transform.rotation = camTransform.rotation * originalRotation;

    public void SetCamera(Transform cameraTransform) => camTransform = cameraTransform;
}
