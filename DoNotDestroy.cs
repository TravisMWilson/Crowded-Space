using UnityEngine;

public class DoNotDestroy : MonoBehaviour {
    public static DoNotDestroy Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
