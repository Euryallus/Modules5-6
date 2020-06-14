using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    public int m_loadedAmmo { get; private set; }
    public bool m_reloading { get; private set; }
    public GunWeaponTemplate m_gunTemplate { get; private set; }

    private float m_reloadTimer;

    public GunWeapon(WeaponHolder weaponHolder, GunWeaponTemplate template) : base(weaponHolder, template)
    {
        m_loadedAmmo = template.GetMagazineSize();
        m_gunTemplate = template;
    }

    public override void HeldUpdate()
    {
        base.HeldUpdate();

        if (m_loadedAmmo <= 0 && !m_reloading)
        {
            StartReload();
        }

        if (m_reloading)
        {
            m_reloadTimer -= Time.deltaTime;
            if (m_reloadTimer <= 0)
            {
                ReloadDone();
            }
        }
    }

    public override bool ReadyToFire()
    {
        if ( (base.ReadyToFire()) && (m_loadedAmmo > 0) && (!m_reloading) )
        {
            return true;
        }
        return false;
    }

    //Attack is called when the player shoots
    public override void Attack(WeaponAimInfo weaponAimInfo, GameObject gunGameObject, GameObject prefabFireLight, Transform transformHead, bool buttonDown)
    {
        if (!m_gunTemplate.GetContinuousFire() && !buttonDown)
        {
            //If continuous fire is disabled and the mouse button was not pressed on this frame, do not fire
            return;
        }

        m_attackIntervalTimer = m_template.GetAttackInterval();

        GameObject fireParticles = m_gunTemplate.GetFireParticles();
        Transform parentMuzzle = gunGameObject.transform.Find("AimPoint");
        if (fireParticles != null)
        {
            Object.Instantiate(fireParticles, parentMuzzle);
        }
        Object.Instantiate(prefabFireLight, parentMuzzle);

        if (weaponAimInfo.m_raycastHit)
        {
            Debug.Log("Gun fired, hit " + weaponAimInfo.m_hitInfo.transform.name);

            if (weaponAimInfo.m_hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                int damageAmount = Random.Range(m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage() + 1);
                weaponAimInfo.m_hitInfo.transform.GetComponent<Enemy>().Damage(damageAmount);
                UIManager.instance.ShowEnemyHitPopup(damageAmount, weaponAimInfo.m_hitInfo.point);
            }
        }
        else
        {
            Debug.Log("Gun fired, hit nothing");
        }

        //if (Physics.Raycast(transformHead.position, transformHead.forward, out RaycastHit hitInfo, m_gunTemplate.GetRange(), ~LayerMask.GetMask("Player")))
        //{
        //    Debug.Log("Gun fired, hit " + hitInfo.transform.name);

        //    if (hitInfo.collider.gameObject.CompareTag("Enemy"))
        //    {
        //        int damageAmount = Random.Range(m_template.GetMinAttackDamage(), m_template.GetMaxAttackDamage() + 1);
        //        hitInfo.transform.GetComponent<Enemy>().Damage(damageAmount);
        //        UIManager.instance.ShowEnemyHitPopup(damageAmount, hitInfo.point);
        //    }
        //}
        //else
        //{
        //    Debug.Log("Gun fired, hit nothing");
        //}

        if (m_loadedAmmo >= 0)
        {
            m_loadedAmmo--;
        }
        else
        {
            Debug.LogError("Shooting gun with no ammo (" + m_template.GetWeaponName() + ")");
        }

        gunGameObject.transform.Find("Gun").GetComponent<Animator>().SetTrigger("Shoot");
        SoundEffectPlayer.instance.PlaySoundEffect(m_template.GetAttackSound(), true, transformHead.position, 1f, 0.95f, 1.05f);
    }

    private void StartReload()
    {
        if(m_weaponHolder.ammo > 0)
        {
            Debug.Log(m_template.GetWeaponName() + ": Starting reload");
            m_reloadTimer = m_gunTemplate.GetReloadTime();
            m_reloading = true;
        }
    }

    private void ReloadDone()
    {
        Debug.Log(m_template.GetWeaponName() + ": Done reloading");

        m_reloading = false;
        //Set loaded ammo to the gun's magazine size, or the remaining amount of ammo if there is not enough for a full reload
        int reloadAmount = ((GunWeaponTemplate)m_template).GetMagazineSize();
        if(reloadAmount > m_weaponHolder.ammo)
        {
            reloadAmount = m_weaponHolder.ammo;
        }
        m_loadedAmmo = reloadAmount;
        m_weaponHolder.ammo -= reloadAmount;
    }
}
