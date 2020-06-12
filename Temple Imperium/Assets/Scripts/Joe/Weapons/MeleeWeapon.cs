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

    public void Attack(GameObject meleeGameObject, Transform transformHead)
    {
        m_attackIntervalTimer = m_template.GetAttackInterval();

        if (Physics.Raycast(transformHead.position, transformHead.forward, out RaycastHit hitInfo, m_meleeTemplate.GetRange()))
        {
            Debug.Log("Attacking with melee weapon, hit " + hitInfo.transform.name);

            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                int damageAmount = Random.Range(m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage() + 1);
                hitInfo.transform.GetComponent<BasicEnemy>().Damage(damageAmount);
                UIManager.instance.ShowEnemyHitPopup(damageAmount, hitInfo.point);
            }
        }
        else
        {
            Debug.Log("Attacking with melee weapon, hit nothing");
        }

        meleeGameObject.transform.Find("Weapon").GetComponent<Animator>().SetTrigger("Attack");
        SoundEffectPlayer.instance.PlaySoundEffect(m_template.GetAttackSound(), true, transformHead.position, 1f, 0.95f, 1.05f);
    }
}
