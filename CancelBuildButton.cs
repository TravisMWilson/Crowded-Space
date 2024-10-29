using UnityEngine;

public class CancelBuildButton : MonoBehaviour {
    [SerializeField] private CanvasGroup blackScreen;

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");

        PopupManager.Instance.ShowPopup("Are you sure you want to cancel and go back to the station?", PopupType.YesNo, () => {
            Utility.Instance.FadeCanvasGroup(blackScreen, 1, true);

            Utility.Instance.Delay(1, () => {
                ChangeScene.Instance.GoToScene("MainHub");
                Utility.Instance.FadeCanvasGroup(blackScreen, 1, false, 0.5f);
            });

            Utility.Instance.Delay(1.5f, () => UIController.Instance.ShowMain());
        });
    }
}
