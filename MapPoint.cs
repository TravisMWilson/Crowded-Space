using TMPro;
using UnityEngine;

public class MapPoint : MonoBehaviour {
    private TextMeshProUGUI jumpPowerLevel;
    private string jumpDifficulty;

    void Start() {
        jumpPowerLevel = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        jumpDifficulty = Utility.AddSpacesBeforeCapitals(transform.GetChild(0).name);
    }

    void OnMouseDown() => UIController.Instance.SelectMapPoint(jumpDifficulty, long.Parse(jumpPowerLevel.text));
}
