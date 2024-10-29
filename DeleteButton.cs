using UnityEngine;

public class DeleteButton : MonoBehaviour {
    [SerializeField] private Material selected;
    [SerializeField] private Material unselected;

    private void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");

        if (Settings.Instance.InCreativeMode()) {
            PopupManager.Instance.ShowPopup("The delete option is disabled in creative mode. There isn't a reason to delete anything.", PopupType.OK);
            return;
        }

        Settings.Instance.DeleteItemsFromInventory();
        GetComponent<Renderer>().material = Settings.Instance.InDeleteMode() ? selected : unselected;
    }
}