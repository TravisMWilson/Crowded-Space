using UnityEngine;
using UnityEngine.SceneManagement;

public class Magnet : MonoBehaviour {
    private const float SEARCH_INTERVAL = 1f;

    private Item item;
    private float searchRadius = 10f;
    private float timer = 0f;

    void Start() {
        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) searchRadius = Inventory.GetBlockParameter("pickupDistTotal", item.Tier, item.GetGrade()) + 10f;
    }

    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        timer += Time.deltaTime;

        if (timer >= SEARCH_INTERVAL) {
            SearchForPickups();
            timer = 0f;
        }
    }

    private void SearchForPickups() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);

        foreach (var hitCollider in hitColliders) {
            if (hitCollider.CompareTag("Pickup")) {
                Utility.Instance.LerpToParent(hitCollider.gameObject, gameObject);
                hitCollider.gameObject.tag = "Collecting";
            }
        }
    }
}
