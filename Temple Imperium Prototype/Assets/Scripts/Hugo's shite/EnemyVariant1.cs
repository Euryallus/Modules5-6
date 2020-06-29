using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVariant1 : Enemy
{
    

    

    public EnemyVariant1() : base(15f, 170f, 5f, 5f)
    {
    }

    public override void Engage()
    {
        float damageToDo = meleeHitDamage;

        if (canSeePlayer)
        {
            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

            meleeHit();
                
            
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
