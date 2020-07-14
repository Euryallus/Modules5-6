using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Class for Enemy #2 behaviours, child class of Enemy
//

public class EnemyVariant2 : Enemy
{
    private float bulletSpawnCount = 0;
    [Header("Variant 2 specific variables")]
        [SerializeField]
        private float bulletSpawnRate = 2f;
        [SerializeField]
        GameObject bulletBlueprint;
        [SerializeField]
        public GameObject bullet;
        [SerializeField]
        public float bulletSpeed = 5;

    public EnemyVariant2() : base(20f, 150f, 2.5f, 3.5f, 2)
    {
        // ## CLASS CONSTRUCTOR
        // ## Calls "Enemy" parent constructor with values assigned to variant 2
    }

    public override void Engage()
    {
        //
        // ## ENGAGE BEHAVIOUR (ENEMY #2 OVERRIDE)
        //

        if (canSeePlayer)
        {
            if(Vector3.Distance(player.transform.position, transform.position) > 7.5f)
            {
                //
                // Ranged attack behaviour - if more than 7.5m away, fire bullets at 'bulletSpawnRate' rate
                //

                agent.SetDestination(transform.position);

                if(bulletSpawnCount > bulletSpawnRate)
                {
                    bulletSpawnCount = 0;
                    bullet = Instantiate(bulletBlueprint);
                    bullet.GetComponent<bulletTemp>().setParent(gameObject);
                    bullet.GetComponent<Rigidbody>().velocity = (playerVector.normalized * bulletSpeed);
                    bullet.transform.position = gameObject.transform.position + (playerVector.normalized);
                }
            }
            else 
            {
                // 
                // If out of ranged attack range, initiate melee attack (move towards player until 2m away, melee hit)
                //

                if (playerDist >= 2f)
                {
                    agent.SetDestination(lastKnownPos);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    meleeHit();
                }

               
            }

            // Increase time since last bullet hit, turn to face the player

            bulletSpawnCount += Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
        }
        
        base.Engage(); //call base ENGAGE behaviour from parent class ('Enemy')
    }

    public override void Investigate()
    {
        lookingCount += Time.deltaTime;
    
        if (lookingCount > transitionTime)
        {
            currentState = State.Patrol;
        }
    
        base.Investigate();
    }
}