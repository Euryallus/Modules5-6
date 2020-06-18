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

    [Header("-- Heat Effect")]

    [SerializeField]
    [Tooltip("How long the target is set on fire for (seconds)")]
    private int m_fireEffectTime;

    [SerializeField]
    [Tooltip("How much damage is dealt every timeBetweenFireDamage seconds while the target is on fire")]
    private int m_fireDamage;

    [SerializeField]
    [Tooltip("Amount of time between the target taking fire damage (seconds)")]
    private float m_timeBetweenFireDamage;

    [Header("-- Power Effect")]

    [SerializeField]
    [Tooltip("Minimum damage done to the target when hit after charging the weapon")]
    private int m_minPowerDamage;

    [SerializeField]
    [Tooltip("Maximum damage done to the target when hit after charging the weapon")]
    private int m_maxPowerDamage;

    [Header("-- Ice Effect")]

    [SerializeField]
    [Tooltip("Multiplier for the speed of the target entity for slowdown effect")]
    private float m_speedMultiplier;

    [SerializeField]
    [Tooltip("Amount of time (seconds) to apply the speedMultiplier after an entity is hit")]
    private float m_slowdownTime;

    [Header("-- Heal Effect")]

    [SerializeField]
    [Tooltip("Amount of health to restore to the entity holding the weapon when they deal damage")]
    private int m_healthRestoreAmount;


    //Name of sound effect to be looped while this weapon is firing
    [SerializeField] [HideInInspector]
    public string m_firingSound;

    //Volume of sound effect to be looped while this weapon is firing
    [SerializeField] [HideInInspector]
    public float m_firingSoundVolume;

    //Name of sound effect to be played when this weapon stops firing
    [SerializeField] [HideInInspector]
    public string m_disableSound;

    //Volume of sound effect to be played when this weapon stops firing
    [SerializeField] [HideInInspector]
    public float m_disableSoundVolume;

    //Name of sound effect to be played when this weapon fires with power StarStone
    [SerializeField] [HideInInspector]
    public string m_powerSound;

    //Volume of sound effect to be played when this weapon fires with power StarStone
    [SerializeField] [HideInInspector]
    public float m_powerSoundVolume;

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
    public string GetFiringSound()
    {
        return m_firingSound;
    }
    public float GetFiringSoundVolume()
    {
        return m_firingSoundVolume;
    }
    public string GetDisableSound()
    {
        return m_disableSound;
    }
    public float GetDisableSoundVolume()
    {
        return m_disableSoundVolume;
    }
    public int GetFireEffectTime()
    {
        return m_fireEffectTime;
    }
    public int GetFireDamage()
    {
        return m_fireDamage;
    }
    public float GetTimeBetweenFireDamage()
    {
        return m_timeBetweenFireDamage;
    }
    public int GetMinPowerDamage()
    {
        return m_minPowerDamage;
    }
    public int GetMaxPowerDamage()
    {
        return m_maxPowerDamage;
    }
    public float GetSpeedMultiplier()
    {
        return m_speedMultiplier;
    }
    public float GetSlowdownTime()
    {
        return m_slowdownTime;
    }
    public int GetHealthRestoreAmount()
    {
        return m_healthRestoreAmount;
    }

    #endregion
}
