using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Prototype phase
// ## Purpose: Boss enemy variant
//
public class bossEnemy : Enemy
{
    private bool firing = false;
    private GameObject missile;
    [SerializeField]
    private GameObject missileBlueprint;

    [SerializeField]
    private float fireRate = 3;
    public bossEnemy() : base(40f, 180, 4, 1.5f, 4)
    {
        // ## CLASS CONSTRUCTOR
        // ## Calls "Enemy" parent constructor with values assigned to the boss
    }

    public override void Patrol()
    {
        agent.SetDestination(player.transform.position);
    }

    public override void Investigate()
    {
        currentState = State.Patrol;
    }

    public override void Engage()
    {
        if (canSeePlayer)
        {
            if (firing == false)
            {
                //
                // ## Fires missile at player and starts the cooldown for the next missile
                //

                StartCoroutine(fire());
                firing = true;
            }

            agent.SetDestination(transform.position); //when player is visible, stop moving and turn to look at player

            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
        }
        else
        {
            if(agent.CalculatePath(player.transform.position, agent.path) == true)
            {
                agent.SetDestination(player.transform.position);
            }
        }

        //base.Engage(); //call base ENGAGE behaviour from parent class ('Enemy')
    }

    private IEnumerator fire()
    {
        while (canSeePlayer) //repeated every X seconds while player is visible - only resets when the cooldown has ended and the player is no longer visible
        {
            missile = Instantiate(missileBlueprint); //instantiates missile from prefab, assigns it's parent and position
            missile.GetComponent<missileTemp>().setParent(gameObject);
            missile.transform.position = transform.position + transform.forward * 3f;

            yield return new WaitForSeconds(fireRate);
        }

        firing = false;
    }

}
