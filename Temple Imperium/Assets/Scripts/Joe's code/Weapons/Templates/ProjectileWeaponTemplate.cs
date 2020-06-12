using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Weapon Template", menuName = "Weapon Template/Projectile Weapon")]
public class ProjectileWeaponTemplate : WeaponTemplate
{
    //Projectile weapons can be thrown and will explode on impact
    //See tooltips for comments on each of the following priperties:

    [Header("Projectile Weapon Properties")]

    [SerializeField] [Tooltip("How far this projectile can be thrown")]
    private float m_range;

    [SerializeField] [Tooltip("The area that will be affected by this projectile's blast after it lands")]
    private float m_damageArea;
}