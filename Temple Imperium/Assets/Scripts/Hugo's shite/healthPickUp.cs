using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPickUp : MonoBehaviour
{
    [SerializeField]
    int healthOnPickUp = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<playerHealth>().RestoreHealth(healthOnPickUp);
            Destroy(gameObject);
        }
    }
}
