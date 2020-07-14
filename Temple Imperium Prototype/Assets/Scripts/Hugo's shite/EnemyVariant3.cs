using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Class for Enemy #3 behaviours, child class of Enemy
//
public class EnemyVariant3 : Enemy
{
    [Header("Variant 3 specific variables")]
        [SerializeField]
        GameObject missileBlueprint;
        [SerializeField]
        float fireRate = 2;

    private GameObject missile;
    private bool firing = false;
    
    public EnemyVariant3() : base(20f, 130f, 3f, 2f, 3)
    {
        // ## CLASS CONSTRUCTOR
        // ## Calls "Enemy" parent constructor with values assigned to variant 3
    }

    public override void Engage()
    {

        //
        // ## ENGAGE BEHAVIOUR (ENEMY #3 OVERRIDE)
        //

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

        base.Engage(); //call base ENGAGE behaviour from parent class ('Enemy')
    }

    private IEnumerator fire()
    {
        while (canSeePlayer) //repeated every X seconds while player is visible - only resets when the cooldown has ended and the player is no longer visible
        {
            missile = Instantiate(missileBlueprint); //instantiates missile from prefab, assigns it's parent and position
            missile.GetComponent<missileTemp>().setParent(gameObject);
            missile.transform.position = transform.position + transform.forward * 1.5f;

            yield return new WaitForSeconds(fireRate);
        }

        firing = false;
    }


}
