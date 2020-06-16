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
        float damageToDo = meleeHitDamage;

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
                    //star stone effects
                    switch (generator.returnState())
                    {
                        case generatorStates.starStoneActive.Orange:
                            //Burn enemy
                            hitObj.GetComponent<playerHealth>().setOnFire(fireLength, fireDamage, 0.5f);
                            break;

                        case generatorStates.starStoneActive.Blue:

                            hitObj.GetComponent<playerMovement>().slowEffect(slowPercent, slowTime);

                            break;

                        case generatorStates.starStoneActive.Purple:
                            damageToDo *= purpleDamagePercent;
                            break;

                        case generatorStates.starStoneActive.Pink:
                            healTimeElapsed = 0;
                            StartCoroutine(regenHealth(healthRegenLength, timeBetweenHealthRegen, healthRegenAmountPerCall));

                            break;
                    }

                    hitObj.GetComponent<playerHealth>().takeDamage(damageToDo);

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
