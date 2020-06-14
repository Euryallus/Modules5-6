using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletTemp : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Player"))
        {
            collision.transform.gameObject.GetComponent<playerHealth>().takeDamage(5);
        }

        Destroy(gameObject);
    }
}
