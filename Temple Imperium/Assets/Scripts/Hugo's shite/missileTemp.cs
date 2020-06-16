using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileTemp : MonoBehaviour
{
    GameObject player;
    [SerializeField]
    GameObject explosion;

    [SerializeField]
    float missileSpeed = 3;

    [SerializeField]
    float missileDamage = 10f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, missileSpeed * Time.deltaTime);

        transform.LookAt(player.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject effect = Instantiate(explosion);
        effect.transform.position = transform.position;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<playerHealth>().takeDamage(missileDamage);
        }

        Destroy(gameObject);
    }
}
