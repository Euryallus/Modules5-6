using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public WeaponHolder m_weaponHolder { get; private set; }
    public WeaponTemplate m_template { get; private set; }
    public bool m_hideHeldWeapon { get; protected set; }
    public float m_attackIntervalTimer { get; protected set; }

    public Weapon(WeaponHolder weaponHolder, WeaponTemplate template)
    {
        m_weaponHolder = weaponHolder;
        m_template = template;
    }

    protected void SetHideHeldWeapon(bool hideHeldWeapon)
    {
        m_hideHeldWeapon = hideHeldWeapon;
        m_weaponHolder.SetHeldWeaponHidden(hideHeldWeapon);
    }

    public virtual void Update()
    {
        if (m_attackIntervalTimer > 0f)
        {
            m_attackIntervalTimer -= Time.deltaTime;
        }
    }

    public virtual void HeldUpdate()
    {

    }
}
