using UnityEngine;

public class PulseEffect : MonoBehaviour {
    [SerializeField] private float pulseSpeed = 2.0f;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;

    private Vector3 initialScale;

    void Start() => initialScale = transform.localScale;

    void Update() {
        float scaleFactor = (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2;
        scaleFactor = Mathf.Lerp(minScale, maxScale, scaleFactor);

        transform.localScale = initialScale * scaleFactor;
    }
}
