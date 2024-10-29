using UnityEngine;

public class Pickup : MonoBehaviour {
    [SerializeField] private Item item;

    private Vector3 aimDirection;
    private float speed;
    private bool movingForward = true;

    void Start() {
        speed = Random.Range(5f, 25f);
        aimDirection = (new Vector3(0, 0, -1000) - transform.position).normalized;
    }

    void Update() {
        if (movingForward) transform.position += speed * Time.deltaTime * aimDirection;

        if (transform.position.z < -550) {
            Destroy(gameObject);
        }
    }

    public void SetItem(Item i) => item = i;
    public void StopMovement() => movingForward = false;

    public void CollectPickup() {
        _ = Inventory.Instance.Add(item, 1);
        Destroy(gameObject);
    }
}
