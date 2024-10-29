using UnityEngine;

public class CloseMessageButton : MonoBehaviour {
    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");
        PopupManager.Instance.OnOKButtonClicked();
    }
}
