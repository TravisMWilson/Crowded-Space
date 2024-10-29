using UnityEngine;

public class NoButton : MonoBehaviour {
    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");
        PopupManager.Instance.OnNoButtonClicked();
    }
}
