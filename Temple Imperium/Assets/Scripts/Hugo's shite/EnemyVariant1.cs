using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVariant1 : Enemy
{
    [SerializeField]
    float meleeDistance = 4f;
    [SerializeField]
    float meleeHitDamage = 5;

    [SerializeField]
    float hitInterval = 1f;
    float hitCount = 0;

   public EnemyVariant1() : base(15f, 170f, 5f, 5f)
   {
        Debug.Log("Variant1 success");
   }

    public override void Engage()
    {
        if (canSeePlayer)
        {
            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

            RaycastHit target;
            if(Physics.Raycast(transform.position, transform.forward, out target, meleeDistance))
            {
                GameObject hitObj = target.transform.gameObject;

                if (hitObj.CompareTag("Player") && hitCount > hitInterval)
                {
                    hitObj.GetComponent<playerHealth>().takeDamage(meleeHitDamage);
                    hitCount = 0;
                }
            }
        }

        if(playerDist >= 3.5f)
        {
            agent.SetDestination(lastKnownPos);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        hitCount += Time.deltaTime;

        base.Engage();
    }

    public override void Investigate()
    {
        base.Investigate();

        currentState = State.Patrol;
    }
}
