﻿using UnityEngine;

//------------------------------------------------------\\
//  AmmoPickup can be added to a GameObject to allow    \\
//  it to be picked up by a player, and supply them     \\
//  with a set amount of ammo for a certain gun.        \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class AmmoPickup : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private int ammo;                   //The amount of ammo to be picked up
    [SerializeField]
    private GunWeaponTemplate gunType;  //The gun type this ammo is used for
    [SerializeField]
    private string pickupSound; //Name of the sound to be played when this item is picked up

    private void OnTriggerEnter(Collider other)
    {
        //When the player collides with this ammo box, pick up some ammo
        //and destroy the box so it cannot be picked up again
        if (other.CompareTag("Player"))
        {
            other.GetComponent<WeaponHolder>().PickupAmmo(ammo, gunType);
            AudioManager.instance.PlaySoundEffect3D(pickupSound, transform.position);
            Destroy(gameObject);
        }
    }
}