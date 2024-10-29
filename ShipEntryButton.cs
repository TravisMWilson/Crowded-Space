using TMPro;
using UnityEngine;

public class ShipEntryButton : MonoBehaviour {
    public void DisplayCreation(TextMeshProUGUI label) {
        SoundManager.Instance.PlaySound("Button");
        UIController.Instance.UpdateHangerDisplays(label.text);
    }
}
