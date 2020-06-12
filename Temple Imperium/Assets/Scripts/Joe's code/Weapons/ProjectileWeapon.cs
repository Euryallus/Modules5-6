using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public ProjectileWeaponTemplate m_projectileTemplate { get; private set; }

    public ProjectileWeapon(WeaponHolder weaponHolder, ProjectileWeaponTemplate template) : base(weaponHolder, template)
    {
        m_projectileTemplate = template;
    }
}
