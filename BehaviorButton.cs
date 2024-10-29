using UnityEngine;

public class BehaviorButton : MonoBehaviour {
    private static bool isVisible = false;

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");
        if (Utility.IsMoving) return;

        if (!Settings.Instance.InCreativeMode()) {
            PopupManager.Instance.ShowPopup("No need to modify the creations behavior, this is your ship and will be under your control.", PopupType.OK);
            return;
        }

        isVisible = !isVisible;
        UIController.Instance.ToggleBehaviorUI(isVisible);
    }

    public static bool IsBehaviorsOpen() => isVisible;
}
