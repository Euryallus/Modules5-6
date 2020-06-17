using UnityEngine;
public class WeaponTemplate : ScriptableObject
{
    #region Properties

    //Weapon properties
    //[SerializeField] used to show in editor. [Tooltip] is shown on mouse hover in inspector.
    //See tooltips for comments on each of the following properties:

    [Header("Weapon Properties (See tooltips for more info)")]

    [SerializeField]
    [Tooltip("Name used to identify this weapon in-game")]
    private string m_weaponName;

    [SerializeField]
    [Tooltip("The prefab GameObject to be used when holding this weapon")]
    private GameObject m_gameObject;

    [SerializeField]
    [Tooltip("The offset of the weapon GameObject from its parent")]
    private Vector3 m_visualOffset;

    [SerializeField]
    [Tooltip("Minimum damage given to the enemy/player being attacked when they are hit")]
    private int m_minAttackDamage;

    [SerializeField]
    [Tooltip("Maximum damage given to the enemy/player being attacked when they are hit")]
    private int m_maxAttackDamage;

    [SerializeField]
    [Tooltip("Minimum time between each usage of this weapon (seconds)")]
    private float m_attackInterval;

    //Set in custom editor:

    //Name of sound effect to be played when this weapon is used
    [SerializeField] [HideInInspector]
    public string m_attackSound;

    //Volume of sound effect to be played when this weapon is used
    [SerializeField] [HideInInspector]
    public float m_attackSoundVolume;

    #endregion

    #region Getters

    //Getters
    //=======

    public string GetWeaponName()
    {
        return m_weaponName;
    }
    public GameObject GetGameObject()
    {
        return m_gameObject;
    }
    public Vector3 GetVisualOffset()
    {
        return m_visualOffset;
    }
    public int GetMinAttackDamage()
    {
        return m_minAttackDamage;
    }
    public int GetMaxAttackDamage()
    {
        return m_maxAttackDamage;
    }
    public float GetAttackInterval()
    {
        return m_attackInterval;
    }
    public string GetAttackSound()
    {
        return m_attackSound;
    }
    public float GetAttackSoundVolume()
    {
        return m_attackSoundVolume;
    }

    #endregion
}
