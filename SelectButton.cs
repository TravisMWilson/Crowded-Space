using System.Collections;
using TMPro;
using UnityEngine;

public class SelectButton : MonoBehaviour {
    [SerializeField] private float rotationDuration = 0.4f;
    [SerializeField] private float rotationSpeed = 360f;

    private Item displayItem;

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");

        if (Settings.Instance.InCreativeMode()) {
            GameObject tierInput = GameObject.Find("ItemTierInputField");
            if (tierInput.GetComponent<TMP_InputField>().text == string.Empty) {
                PopupManager.Instance.ShowPopup("There is no tier level chosen for this block. In the text field above put a tier you would like then select the block.", PopupType.OK);
                return;
            }
        }

        Inventory.Instance.ItemBlock(displayItem);
        _ = StartCoroutine(SpinButton());
    }

    public void SetDisplayItem(Item item) => displayItem = item;

    private IEnumerator SpinButton() {
        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;

        while (elapsedTime < rotationDuration) {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(0, rotationSpeed, elapsedTime / rotationDuration);
            transform.rotation = initialRotation * Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        transform.rotation = initialRotation * Quaternion.Euler(0, 0, 360f);
    }
}
