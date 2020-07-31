using UnityEngine;

//------------------------------------------------------\\
//  Template that defines a type of melee weapon that   \\
//  can be edited in the inspector by creating a        \\
//  ScriptableObject and adjusting its properties       \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

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