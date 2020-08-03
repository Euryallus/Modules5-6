using UnityEngine;

//------------------------------------------------------\\
//  When used on a GameObject placed in the scene,      \\
//  this script allows the prototype weapon to be       \\
//  charged when within the object's trigger            \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class PrototypeWeaponCharging : MonoBehaviour
{
    private PrototypeWeapon weapon;                         //The weapon to be charged
    private const string loopSoundId = "starStoneCharge";   //Name of the looping sound played when charging

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //When the player enters the trigger, start charging the weapon and playing the charge sound
            weapon = other.gameObject.GetComponent<WeaponHolder>().GetPrototypeWeapon();
            if(weapon != null)
            {
                weapon.SetCharging(true);
                SoundEffectPlayer.instance.PlayLoopingSoundEffect("Charge Loop", true, transform.position, loopSoundId, 1f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //When the player exits the trigger, stop charging the weapon and stop the looping sound
            if (weapon != null)
            {
                weapon.SetCharging(false);
                weapon = null;
                SoundEffectPlayer.instance.StopLoopingSoundEffect(loopSoundId);
            }
        }
    }
}
