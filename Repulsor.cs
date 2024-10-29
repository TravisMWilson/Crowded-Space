using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Repulsor : MonoBehaviour {
    [SerializeField] private float growthDuration = 0.5f;
    [SerializeField] private float maxCooldown = 30f;

    private Item item;
    private GameObject playerShip;
    private GameObject spherePrefab;
    private GameObject currentSphere;
    private bool isGrowing = false;
    private bool isPlayer = false;
    private float cooldown = 0f;
    private float currentGrowthTime = 0f;
    private float maxRadius = 25f;

    void Start() {
        playerShip = GameObject.Find("PlayerShip");
        if (playerShip) isPlayer = transform.parent.name == playerShip.name;
        spherePrefab = Resources.Load<GameObject>("Blocks/Special/Shockwave");

        item = GetComponent<ItemBlock>().GetItem();
        if (item != null) maxRadius = Inventory.GetBlockParameter("pushDistTotal", item.Tier, 1);
    }
    
    void Update() {
        if (gameObject.layer == 5 || SceneManager.GetActiveScene().name != "WarpScene") return;

        if (cooldown < maxCooldown) {
            cooldown += Time.deltaTime;
            if (isPlayer) UIController.Instance.UpdateRepulsorBar(cooldown, maxCooldown);
        }

        float distanceToPlayer = 10000f;

        if (!isPlayer) distanceToPlayer = (transform.position - playerShip.transform.position).magnitude;

        if ((Input.GetKeyDown(KeyCode.Alpha2) && isPlayer) || (distanceToPlayer <= (maxRadius / 2) && !isPlayer)) {
            if (!isGrowing && cooldown >= maxCooldown) {
                currentSphere = Instantiate(spherePrefab, transform.position, Quaternion.identity);
                currentSphere.transform.localScale = Vector3.zero;
                currentSphere.transform.GetChild(0).GetComponent<BillboardFX>().SetCamera(Camera.main.transform);

                isGrowing = true;
                currentGrowthTime = 0f;
                cooldown = 0;
            }
        }
        
        if (isGrowing && currentSphere != null) {
            currentGrowthTime += Time.deltaTime;
            float growthFactor = Mathf.Clamp01(currentGrowthTime / growthDuration);
            float currentRadius = Mathf.Lerp(0f, maxRadius, growthFactor);
            currentSphere.transform.localScale = Vector3.one * currentRadius;

            Image canvasImage = currentSphere.transform.GetChild(0).GetChild(0).GetComponent<Image>();

            if (growthFactor >= 0.75f) {
                float alphaLerpFactor = Mathf.InverseLerp(0.75f, 1f, growthFactor);
                Color newColor = canvasImage.color;
                newColor.a = Mathf.Lerp(1, 0f, alphaLerpFactor);
                canvasImage.color = newColor;
            }

            if (currentGrowthTime >= growthDuration) {
                isGrowing = false;
                Destroy(currentSphere);
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, currentRadius / 2);
            
            foreach (Collider col in colliders) {
                if (col.transform.root != transform.root) {
                    Vector3 pushDirection = col.transform.position - currentSphere.transform.position;
                    float distanceToMove = maxRadius * Time.deltaTime;
                    col.transform.parent.position += pushDirection.normalized * distanceToMove;
                }
            }
        }
    }
}
