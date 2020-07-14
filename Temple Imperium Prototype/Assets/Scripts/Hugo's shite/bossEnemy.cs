using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }

    public override void Patrol()
    {
        agent.SetDestination(player.transform.position);
        //base.Patrol();
    }

    public override void Investigate()
    {
        currentState = State.Patrol;
        //base.Investigate();
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
