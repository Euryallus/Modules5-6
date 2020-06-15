using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletTemp : MonoBehaviour
{
    [SerializeField]
    float bulletDamage = 5;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Player"))
        {
            collision.transform.gameObject.GetComponent<playerHealth>().takeDamage(bulletDamage);
        }

        Destroy(gameObject);
    }
}
