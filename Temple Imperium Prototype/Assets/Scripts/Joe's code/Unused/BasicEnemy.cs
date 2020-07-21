using UnityEngine;

//------------------------------------------------------\\
//  This script is a basic proof of concept enemy that  \\
//  was used for testing purposes and later replaced by \\
//  the Enemy.cs script.                                \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class BasicEnemy : MonoBehaviour
{
    //Set in inspector
    [SerializeField]
    private int startHealth;    //Amount of health on level start

    private int health;         //Current amount of health

    // Start is called before the first frame update
    void Start()
    {
        //Set the start health when the enemy is first spawned
        health = startHealth;
    }

    public void Damage(int hitPoints)
    {
        //Removes health from the enemy and kills them
        //  if health reaches 0
        health -= hitPoints;
        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Removes this enemy from the scene
        Destroy(gameObject);
    }
}
