using UnityEngine;

[CreateAssetMenu(fileName = "Prototype Weapon Template", menuName = "Weapon Template/Prototype Weapon")]
public class PrototypeWeaponTemplate : WeaponTemplate
{
    //See tooltips for comments on each of the following priperties:

    [Header("Prototype Weapon Properties")]

    [SerializeField]
    [Tooltip("...")]
    private float m_example;
}
