using UnityEngine;

public class PrototypeWeaponCharging : MonoBehaviour
{
    private PrototypeWeapon weapon;

    private const string loopSoundId = "starStoneCharge";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            weapon = other.gameObject.GetComponent<WeaponHolder>().GetPrototypeWeapon();
            if(weapon != null)
            {
                weapon.StartCharging();
                SoundEffectPlayer.instance.PlayLoopingSoundEffect("Charge Loop", true, transform.position, loopSoundId, 1f);
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
                SoundEffectPlayer.instance.StopLoopingSoundEffect(loopSoundId);
            }
        }
    }
}
