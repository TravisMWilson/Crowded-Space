using UnityEngine;

public class HideOnScrollMaxMin : MonoBehaviour {
    [SerializeField] private float MinPosYVisible;
    [SerializeField] private float MaxPosYVisible;

    private bool isVisible = true;

    void Update() {
        if ((transform.parent.position.y < MinPosYVisible || transform.parent.position.y > MaxPosYVisible) && isVisible) {
            isVisible = false;
            HideChildren();
        } else if (transform.parent.position.y >= MinPosYVisible && transform.parent.position.y <= MaxPosYVisible && !isVisible) {
            isVisible = true;
            HideChildren(false);
        }
    }

    public void HideChildren(bool toggle = true) {
        foreach (Transform child in transform) {
            if (child.TryGetComponent<ItemBlock>(out var itemBlock)) {
                if (toggle) {
                    itemBlock.SwapMaterial(false, new Material(Resources.Load<Material>("Materials/EnemyCloak")));
                } else if (!toggle) {
                    itemBlock.SwapMaterial(true);
                }
            }
        }
    }
}
