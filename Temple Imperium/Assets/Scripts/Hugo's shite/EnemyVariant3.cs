using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVariant3 : Enemy
{
    [SerializeField]
    GameObject missileBlueprint;
    GameObject missile;

    [SerializeField]
    float fireRate = 2;

    bool firing = false;
    
    public EnemyVariant3() : base(20f, 130f, 3f, 2f)
    {

    }

    public override void Engage()
    {
        if (canSeePlayer)
        {
            if (firing == false)
            {
                StartCoroutine(fire());
                firing = true;
            }

            agent.SetDestination(transform.position);

            Quaternion lookRotation = Quaternion.LookRotation(playerVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
        }

        base.Engage();
    }

    private IEnumerator fire()
    {
        while (canSeePlayer)
        {
            missile = Instantiate(missileBlueprint);
            missile.transform.position = transform.position + transform.forward * 1.5f;

            yield return new WaitForSeconds(fireRate);
        }

        firing = false;
    }


}
