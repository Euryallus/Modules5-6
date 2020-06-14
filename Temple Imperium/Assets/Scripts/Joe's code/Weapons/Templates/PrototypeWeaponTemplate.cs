using UnityEngine;

[CreateAssetMenu(fileName = "Prototype Weapon Template", menuName = "Weapon Template/Prototype Weapon")]
public class PrototypeWeaponTemplate : WeaponTemplate
{
    #region Properties

    //See tooltips for comments on each of the following priperties:

    [Header("Prototype Weapon Properties")]

    [SerializeField]
    [Tooltip("...")]
    private GameObject m_beamGameObject;

    [SerializeField]
    [Tooltip("...")]
    private float m_range;

    [SerializeField]
    [Tooltip("...")]
    private string m_disableSound;

    #endregion

    #region Getters

    //Getters
    //=======

    public GameObject GetBeamGameObject()
    {
        return m_beamGameObject;
    }
    public float GetRange()
    {
        return m_range;
    }
    public string GetDisableSound()
    {
        return m_disableSound;
    }

    #endregion
}
