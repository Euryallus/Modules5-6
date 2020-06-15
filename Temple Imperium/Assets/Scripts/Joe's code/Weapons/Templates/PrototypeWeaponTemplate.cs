using UnityEngine;

[CreateAssetMenu(fileName = "Prototype Weapon Template", menuName = "Weapon Template/Prototype Weapon")]
public class PrototypeWeaponTemplate : WeaponTemplate
{
    #region Properties

    //See tooltips for comments on each of the following priperties:

    [Header("Prototype Weapon Properties")]

    [SerializeField]
    [Tooltip("The offset of the weapon GameObject from its parent when aiming down sights")]
    private Vector3 m_aimDownSightOffset;

    [SerializeField]
    [Tooltip("The prefab beam GameObject to be spawned when this weapon is used")]
    private GameObject m_beamGameObject;

    [SerializeField]
    [Tooltip("Objects within this range can be hit")]
    private float m_range;

    [SerializeField]
    [Tooltip("How frequently damage is dealt when firing at an object (seconds)")]
    private float m_damageInterval;

    [SerializeField]
    [Tooltip("Name of sound effect to be played when this weapon stops firing")]
    private string m_disableSound;

    #endregion

    #region Getters

    //Getters
    //=======

    public Vector3 GetAimDownSightOffset()
    {
        return m_aimDownSightOffset;
    }
    public GameObject GetBeamGameObject()
    {
        return m_beamGameObject;
    }
    public float GetRange()
    {
        return m_range;
    }
    public float GetDamageInterval()
    {
        return m_damageInterval;
    }
    public string GetDisableSound()
    {
        return m_disableSound;
    }

    #endregion
}
