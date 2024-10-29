using UnityEngine;

public class ParicleCollider : MonoBehaviour {
    private GameObject itemBlockParent;
    private Item item;

    void Start() {
        itemBlockParent = ItemBlock.FindItemBlockObject(transform);

        if (itemBlockParent != null) {
            item = itemBlockParent.GetComponent<ItemBlock>().GetItem();
        }
    }

    void OnParticleCollision(GameObject other) {
        if ((other.tag.Contains("Enemy") && gameObject.tag.Contains("Player")) || (other.tag.Contains("Player") && gameObject.tag.Contains("Enemy"))) {
            if (gameObject.tag.Contains("Missile")) {
                float radius = 0f;

                if (itemBlockParent.name.Contains("1") || itemBlockParent.name.Contains("4")) {
                    radius = 1.5f;
                } else if (itemBlockParent.name.Contains("2") || itemBlockParent.name.Contains("5")) {
                    radius = 2.5f;
                } else if (itemBlockParent.name.Contains("3") || itemBlockParent.name.Contains("6")) {
                    radius = 3.5f;
                }

                if (other.TryGetComponent<Shield>(out var otherShield)) {
                    otherShield.ApplyDamage(Inventory.GetBlockParameter("dpsMissileTotal", item.Tier, 1));
                } else {
                    Collider[] colliders = Physics.OverlapSphere(other.transform.position, radius);

                    foreach (Collider col in colliders) {
                        if (col.TryGetComponent<ItemBlock>(out var otherItemBlock)) {
                            otherItemBlock.ApplyDamage(Inventory.GetBlockParameter("dpsMissileTotal", item.Tier, 1));
                        }
                    }
                }
            } else {
                if (other.TryGetComponent<ItemBlock>(out var otherItemBlock)) {
                    if (itemBlockParent.name.Contains("Cannon")) {
                        otherItemBlock.ApplyDamage(Inventory.GetBlockParameter("dpsCannonTotal", item.Tier, 1));
                    } else if (itemBlockParent.name.Contains("Minigun")) {
                        otherItemBlock.ApplyDamage(Inventory.GetBlockParameter("dpsMinigunTotal", item.Tier, 1));
                    } else if (itemBlockParent.name.Contains("Railgun")) {
                        otherItemBlock.ApplyDamage(Inventory.GetBlockParameter("dpsRailgunTotal", item.Tier, 1));
                    } else if (itemBlockParent.name.Contains("Laser")) {
                        otherItemBlock.ApplyDamage(Inventory.GetBlockParameter("dpsLaserTotal", item.Tier, 1));
                    }
                } else if (other.TryGetComponent<Shield>(out var otherShield)) {
                    if (itemBlockParent.name.Contains("Cannon")) {
                        otherShield.ApplyDamage(Inventory.GetBlockParameter("dpsCannonTotal", item.Tier, 1));
                    } else if (itemBlockParent.name.Contains("Minigun")) {
                        otherShield.ApplyDamage(Inventory.GetBlockParameter("dpsMinigunTotal", item.Tier, 1));
                    } else if (itemBlockParent.name.Contains("Railgun")) {
                        otherShield.ApplyDamage(Inventory.GetBlockParameter("dpsRailgunTotal", item.Tier, 1));
                    } else if (itemBlockParent.name.Contains("Laser")) {
                        otherShield.ApplyDamage(Inventory.GetBlockParameter("dpsLaserTotal", item.Tier, 1));
                    }
                }
            }
        } else if (gameObject.tag.Contains("Player") && other.transform.parent.tag.Contains("Pickup")) {
            other.transform.root.GetComponent<Pickup>().StopMovement();
            Utility.Instance.LerpToParent(other.transform.root.gameObject, itemBlockParent.transform.parent.gameObject);
        }
    }
}
