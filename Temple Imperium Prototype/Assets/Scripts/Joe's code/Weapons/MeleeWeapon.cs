using UnityEngine;

//------------------------------------------------------\\
//  A melee weapon that can be used by an entity with   \\
//  a WeaponHolder that holds a MeleeWeaponTemplate.    \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class MeleeWeapon : Weapon
{
    //Properties
    public MeleeWeaponTemplate m_meleeTemplate { get; private set; }    //The template that defines the weapon's base properties

    //Constructor
    public MeleeWeapon(WeaponHolder weaponHolder, MeleeWeaponTemplate template) : base(weaponHolder, template)
    {
        m_meleeTemplate = template;
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();
    }

    //Attack is called when the weapon is used
    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject meleeGameObject, GameObject prefabAttackLight, Transform transformHead, bool buttonDown)
    {
        //Reset attack interval to prevent continuous melee attacks
        m_attackIntervalTimer = m_template.GetAttackInterval();

        if (weaponAimInfo.m_raycastHit)
        {
            //Weapon hit an object
            Debug.Log("Attacking with melee weapon, hit " + weaponAimInfo.m_hitInfo.transform.name);

            GameObject goHit = weaponAimInfo.m_hitInfo.collider.gameObject;
            //Apply random attack damage (between set min and max) to hit enemies
            if (goHit.CompareTag("Enemy"))
            {
                int damageAmount = Random.Range(m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage() + 1);
                weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>().Damage(damageAmount);
                UIManager.instance.ShowEnemyHitPopup(damageAmount, weaponAimInfo.m_hitInfo.point);
            }
            //Explode any hit objects with the ExplodeOnImpact component
            else if (goHit.CompareTag("ExplodeOnImpact"))
            {
                goHit.GetComponent<ExplodeOnImpact>().Explode();
            }
        }
        else
        {
            Debug.Log("Attacking with melee weapon, hit nothing");
        }

        //Trigger the attack animation and sound
        meleeGameObject.transform.Find("Weapon").GetComponent<Animator>().SetTrigger("Attack");
        AudioManager.instance.PlaySoundEffect2D(m_template.m_attackSound, m_template.m_attackSoundVolume, 0.95f, 1.05f);
    }

    //No alternate attack for melee weapons
    public override void AlternateAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, Transform transformHead) {}
}
