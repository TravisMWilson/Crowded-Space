using UnityEngine;

public class ExitGameButton : MonoBehaviour {
    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");
        Settings.Instance.ExitGame();
    }
}
