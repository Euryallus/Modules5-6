using UnityEngine;

[CreateAssetMenu(fileName = "Melee Weapon Template", menuName = "Weapon Template/Melee Weapon")]
public class MeleeWeaponTemplate : WeaponTemplate
{
    #region Properties

    //See tooltips for comments on each of the following priperties:

    [Header("Melee Weapon Properties")]
    
    [SerializeField] [Tooltip("Maximum distance from the target where this weapon will be effective")]
    private float m_range;

    #endregion

    #region Getters

    //Getters
    //=======

    public float GetRange()
    {
        return m_range;
    }

    #endregion
}