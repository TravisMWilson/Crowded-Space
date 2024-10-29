using UnityEngine;

public class Drone : MonoBehaviour {
    public float flyRadius = 10f;
    public float flySpeed = 5f;
    public float shootRadius = 15f;

    public float bulletSpeed = 20f;
    public GameObject bulletPrefab;

    private Transform ship;

    private void Start() => ship = transform.root;

    private void Update() {
        OrbitAroundShip();
        DetectAndShootEnemies();
    }

    private void OrbitAroundShip() {
        if (ship != null) {
            transform.RotateAround(ship.position, Vector3.up, flySpeed * Time.deltaTime);
        }
    }

    private void DetectAndShootEnemies() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootRadius);

        foreach (Collider collider in hitColliders) {
            if (collider.CompareTag("Enemy")) {
                ShootAtEnemy(collider.transform);
            }
        }
    }

    private void ShootAtEnemy(Transform enemy) {
        Vector3 direction = (enemy.position - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
    }
}
