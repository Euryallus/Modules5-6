using UnityEngine;

[CreateAssetMenu(fileName = "Grenade Template", menuName = "Weapon Template/Grenade")]
public class GrenadeWeaponTemplate : WeaponTemplate
{
    #region Properties

    //See tooltips for comments on each of the following priperties:

    [Header("Grenade Properties")]

    [SerializeField]
    [Tooltip("The prefab GameObject with physics properties to be spawned when this grenade is thrown")]
    private GameObject m_throwGameObject;

    [SerializeField]
    [Tooltip("Amount of time before the grenade explodes (seconds)")]
    private float m_delay;

    [SerializeField]
    [Tooltip("The size of the area in which this grenade will cause damage upon exploding")]
    private float m_impactRadius;

    [SerializeField]
    [Tooltip("The size of the area where extra damage (fragDamage) can be taken from framgents")]
    private float m_fragRadius;

    [SerializeField]
    [Tooltip("The amount of extra damage caused by fragments")]
    private int m_fragDamage;

    [SerializeField]
    [Tooltip("Name of sound effect to be played when this grenade is thrown")]
    private string m_throwSound;

    [SerializeField]
    [Tooltip("Prefab for particles to be spawned when this grenade explodes")]
    private GameObject m_explosionParticles;

    #endregion

    #region Getters

    //Getters
    //=======

    public GameObject GetThrowGameObject()
    {
        return m_throwGameObject;
    }
    public float GetDelay()
    {
        return m_delay;
    }
    public float GetImpactRadius()
    {
        return m_impactRadius;
    }
    public float GetFragRadius()
    {
        return m_fragRadius;
    }
    public int GetFragDamage()
    {
        return m_fragDamage;
    }
    public string GetThrowSound()
    {
        return m_throwSound;
    }
    public GameObject GetExplosionParticles()
    {
        return m_explosionParticles;
    }

    #endregion
}