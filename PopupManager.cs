using UnityEngine;
using TMPro;
using System;

public enum PopupType {
    OK,
    YesNo,
    InputOK,
    InputYesNo
}

public class PopupManager : MonoBehaviour {
    public static PopupManager Instance;

    [SerializeField] private GameObject popupWindow;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;
    [SerializeField] private GameObject okButton;

    private Action yesAction;
    private Action noAction;
    private Action okAction;

    void Awake() {
        HidePopup();

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowPopup(string message, PopupType popupType, Action yesCallback = null, Action noCallback = null, Action okCallback = null) {
        if (popupType is PopupType.InputOK or PopupType.InputYesNo) {
            messageText.gameObject.SetActive(false);
            messageInput.gameObject.SetActive(true);
            messageInput.text = message;
        } else {
            messageText.gameObject.SetActive(true);
            messageInput.gameObject.SetActive(false);
            messageText.text = message;
        }

        yesAction = yesCallback;
        noAction = noCallback;
        okAction = okCallback;

        if (popupType is PopupType.OK or PopupType.InputOK) {
            okButton.SetActive(true);
            yesButton.SetActive(false);
            noButton.SetActive(false);
        } else {
            okButton.SetActive(false);
            yesButton.SetActive(true);
            noButton.SetActive(true);
        }

        popupWindow.SetActive(true);
    }

    public void HidePopup() => popupWindow.SetActive(false);

    public void OnYesButtonClicked() {
        yesAction?.Invoke();
        HidePopup();
    }

    public void OnNoButtonClicked() {
        noAction?.Invoke();
        HidePopup();
    }

    public void OnOKButtonClicked() {
        okAction?.Invoke();
        HidePopup();
    }

    public string GetInput() => messageInput.text;
}