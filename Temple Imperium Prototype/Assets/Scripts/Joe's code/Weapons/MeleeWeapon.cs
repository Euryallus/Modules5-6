using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public MeleeWeaponTemplate m_meleeTemplate { get; private set; }

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
        m_attackIntervalTimer = m_template.GetAttackInterval();

        if (Physics.Raycast(transformHead.position, transformHead.forward, out RaycastHit hitInfo, m_meleeTemplate.GetRange(), ~LayerMask.GetMask("Player")))
        {
            Debug.Log("Attacking with melee weapon, hit " + hitInfo.transform.name);

            GameObject goHit = hitInfo.collider.gameObject;
            if (goHit.CompareTag("Enemy"))
            {
                int damageAmount = Random.Range(m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage() + 1);
                hitInfo.transform.GetComponent<Enemy>().Damage(damageAmount);
                UIManager.instance.ShowEnemyHitPopup(damageAmount, hitInfo.point);
            }
            else if (goHit.CompareTag("ExplodeOnImpact"))
            {
                goHit.GetComponent<ExplodeOnImpact>().Explode();
            }
        }
        else
        {
            Debug.Log("Attacking with melee weapon, hit nothing");
        }

        meleeGameObject.transform.Find("Weapon").GetComponent<Animator>().SetTrigger("Attack");
        SoundEffectPlayer.instance.PlaySoundEffect2D(m_template.m_attackSound, m_template.m_attackSoundVolume, 0.95f, 1.05f);
    }

    public override void AlternateAttack(WeaponAimInfo weaponAimInfo, GameObject weaponGameObject, Transform transformHead) {}
}
