using UnityEngine;

//------------------------------------------------------\\
//  Increases player health when picked up              \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class HealthPickup : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private int health; //Amount of health to add when this pickup is collected
    [SerializeField]
    private string pickupSound; //Name of the sound to be played when this item is picked up

    private void OnTriggerEnter(Collider other)
    {
        //If the player collides with the pickup, restore a set amount of health
        //  and remove the pickup so it can't be collected again
        if (other.CompareTag("Player"))
        {
            other.GetComponent<playerHealth>().RestoreHealth(health);
            SoundEffectPlayer.instance.PlaySoundEffect3D(pickupSound, transform.position);
            Destroy(gameObject);
        }
    }
}
