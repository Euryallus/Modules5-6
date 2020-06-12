using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVariant1 : Enemy
{
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
        }

        agent.SetDestination(lastKnownPos);

        base.Engage();
    }

    public override void Investigate()
    {
        base.Investigate();

        currentState = State.Patrol;
    }
}
