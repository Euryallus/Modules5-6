using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField]
    private int ammo;
    [SerializeField]
    private GunWeaponTemplate gunType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<WeaponHolder>().PickupAmmo(ammo, gunType);
            Destroy(gameObject);
        }
    }
}
