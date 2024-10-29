using UnityEngine;

public class ColorLerp : MonoBehaviour {
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.red;
    [SerializeField] private float lerpDuration = 2f;

    void Update() {
        float lerpFactor = Mathf.PingPong(Time.time / lerpDuration, 1f);
        Color lerpedColor = Color.Lerp(color1, color2, lerpFactor);
        targetMaterial.color = lerpedColor;
    }
}
