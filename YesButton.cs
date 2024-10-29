using UnityEngine;

public class YesButton : MonoBehaviour {
    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");
        PopupManager.Instance.OnYesButtonClicked();
    }
}
