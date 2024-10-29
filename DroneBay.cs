using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneBay : MonoBehaviour {
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxDrones = 3;

    private int currentDroneCount = 0;

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        if (Input.GetKeyDown(KeyCode.J) && currentDroneCount < maxDrones) {
            SpawnDrone();
        }
    }

    private void SpawnDrone() {
        _ = Instantiate(dronePrefab, spawnPoint.position, Quaternion.identity, transform.root);
        currentDroneCount++;
    }
}
