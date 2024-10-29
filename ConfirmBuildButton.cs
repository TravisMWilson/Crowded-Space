using UnityEngine;

public class ConfirmBuildButton : MonoBehaviour {
    [SerializeField] private CanvasGroup blackScreen;

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");

        PopupManager.Instance.ShowPopup("Confirm your build and go back to the station?", PopupType.YesNo, () => {
            UIController.Instance.SaveCreation();

            Utility.Instance.FadeCanvasGroup(blackScreen, 1, true);

            Utility.Instance.Delay(1, () => {
                ChangeScene.Instance.GoToScene("MainHub");
                Utility.Instance.FadeCanvasGroup(blackScreen, 1, false, 0.5f);
            });

            Utility.Instance.Delay(1.5f, () => {
                UIController.Instance.ShowMain();
                UIController.Instance.UpdateHangerDisplays(UIController.Instance.GetSelectedCreationName());
            });
        });
    }
}
