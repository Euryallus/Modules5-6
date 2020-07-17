using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Class for Enemy #1 behaviours, child class of Enemy
//
public class EnemyVariant1 : Enemy
{
    public EnemyVariant1() : base(15f, 170f, 5f, 5f, 1)
    {
        // ## CLASS CONSTRUCTOR
        // ## Calls "Enemy" parent constructor with values assigned to variant 1
    }

    public override void Engage()
    {
        //
        // ## ENGAGE BEHAVIOUR (ENEMY #1 OVERRIDE)
        //

        if (canSeePlayer)
        {
            // If player is visible, turn to face player
            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

            //Attempt to melee hit (base class 'Enemy' contains meleeHit method)
            meleeHit();

        }

        if(playerDist >= 2f) 
        {
            //if Player is more than X m away, move towards player
            agent.SetDestination(lastKnownPos);
        }
        else
        {
            //if within X m, stop moving towards player
            agent.SetDestination(transform.position);
        }

        hitCount += Time.deltaTime; //increases "time since last hit" counter

        base.Engage(); //calls base ENGAGE behaviour outlined in base class 'Enemy'
    }

}
