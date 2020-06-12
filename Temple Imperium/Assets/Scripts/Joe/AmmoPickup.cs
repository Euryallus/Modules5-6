using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField]
    private int ammo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<WeaponHolder>().PickupAmmo(ammo);
            Destroy(gameObject);
        }
    }
}
