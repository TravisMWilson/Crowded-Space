using UnityEngine;

public class ChangeBuildMode : MonoBehaviour {
    [SerializeField] private int buildMode;
    void OnMouseDown() => ShipCreation.Instance.ChangeBuildMode(buildMode);
}
