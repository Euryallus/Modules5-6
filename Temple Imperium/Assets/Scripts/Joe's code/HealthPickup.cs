using UnityEngine;

//------------------------------------------------------\\
//  Increases player health when picked up              \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class HealthPickup : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private int health; //Amount of health to add when this pickup is collected

    private void OnTriggerEnter(Collider other)
    {
        //If the player collides with the pickup, restore a set amount of health
        //  and remove the pickup so it can't be collected again
        if (other.CompareTag("Player"))
        {
            other.GetComponent<playerHealth>().RestoreHealth(health);
            Destroy(gameObject);
        }
    }
}
