using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVariant2 : Enemy
{
    [SerializeField]
    private float bulletSpawnCount = 0;
    private float bulletSpawnRate = 2f;

    public GameObject bulletBlueprint;
    public GameObject bullet;
    public float bulletSpeed = 5;

    public EnemyVariant2() : base(20f, 150f, 2.5f, 3.5f)
    {
        Debug.Log("Variant2 success");
    }

    public override void Engage()
    {
        if (canSeePlayer)
        {
            agent.SetDestination(transform.position);

            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);

            bulletSpawnCount += Time.deltaTime;

            if(bulletSpawnCount > bulletSpawnRate)
            {
                bulletSpawnCount = 0;
                bullet = Instantiate(bulletBlueprint);
                bullet.GetComponent<Rigidbody>().velocity = (playerVector.normalized * bulletSpeed);
                bullet.transform.position = gameObject.transform.position + (playerVector.normalized);
            }
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