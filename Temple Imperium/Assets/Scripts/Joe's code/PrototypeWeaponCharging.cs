using UnityEngine;

public class PrototypeWeaponCharging : MonoBehaviour
{
    private PrototypeWeapon weapon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            weapon = other.gameObject.GetComponent<WeaponHolder>().GetPrototypeWeapon();
            if(weapon != null)
            {
                weapon.StartCharging();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(weapon != null)
            {
                weapon.StopCharging();
                weapon = null;
            }
        }
    }
}
