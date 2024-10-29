using UnityEngine;

public class Shield : MonoBehaviour {
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public Item Item { get; set; }

    private float regenAmount = 1;
    private float regenRate = 1f;
    private float regenTimer = 0f;
    private bool isPlayer = false;

    void Start() {
        regenAmount = Inventory.GetBlockParameter("energyRegenTotal", Item.Tier, Item.GetGrade());
        isPlayer = transform.root.name.Contains("Player");
        if (isPlayer) AmbientManager.Instance.PlayAmbient("Shield", 3);
    }

    void Update() {
        regenTimer += Time.deltaTime;

        if (regenTimer >= regenRate) {
            Health = Mathf.Min(Health + regenAmount, 500);
            regenTimer = 0f;
        }
    }

    void OnDestroy() {
        if (isPlayer) AmbientManager.Instance.StopChannel(3);
    }

    public void RegenShield(float regenAmount) {
        ApplyDamage(-regenAmount);
        if (Health > MaxHealth) Health = MaxHealth;
    }

    public void ApplyDamage(float damage) {
        Health -= isPlayer ? (damage / 10) : damage;

        if (isPlayer) {
            UIController.Instance.UpdateShieldBar(Health, MaxHealth);
        }

        if (Health <= 0) {
            Destroy(gameObject);
        }
    }
}
