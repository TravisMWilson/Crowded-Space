using UnityEngine;

public class SwapCreativeButton : MonoBehaviour {
    [SerializeField] private Material green;
    [SerializeField] private Material red;

    void OnMouseDown() {
        SoundManager.Instance.PlaySound("Button");
        Settings.Instance.ChangeCreativeMode();
        GetComponent<Renderer>().material = Settings.Instance.InCreativeMode() ? green : red;
    }
}
