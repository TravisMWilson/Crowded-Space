using UnityEngine;

public class BoosterScaleController : MonoBehaviour {
    public float ShipSpeed = 2;

    private const float SCALE_MULTIPLIER = 0.01f;

    void Update() {
        foreach (Transform booster in transform) {
            if (booster.name == "Booster") {
                Vector3 newScale = booster.localScale;
                newScale.y = ShipSpeed * SCALE_MULTIPLIER;
                booster.localScale = newScale;
            }
        }
    }
}
