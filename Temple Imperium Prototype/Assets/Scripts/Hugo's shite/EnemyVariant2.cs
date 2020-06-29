using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVariant2 : Enemy
{
    private float bulletSpawnCount = 0;
    public float bulletSpawnRate = 2f;

    public GameObject bulletBlueprint;
    public GameObject bullet;
    public float bulletSpeed = 5;

    public EnemyVariant2() : base(20f, 150f, 2.5f, 3.5f)
    {
        
    }

    public override void Engage()
    {
        if (canSeePlayer)
        {
            if(Vector3.Distance(player.transform.position, transform.position) > 7.5f)
            {
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

            bulletSpawnCount += Time.deltaTime;
            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
        }
        
        base.Engage();
    }

    public override void Investigate()
    {
        lookingCount += Time.deltaTime;
        agent.SetDestination(lastKnownPos);

        if (lookingCount > transitionTime)
        {
            currentState = State.Patrol;
        }

        base.Investigate();
    }
}