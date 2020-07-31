using System;
using UnityEngine;

//------------------------------------------------------\\
//  A struct containing info about an achievement,      \\  
//  used to define achievements in the editor           \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

[Serializable]
public struct Achievement
{
    //Set in inspector:
    [SerializeField]
    private string m_id;        //Unique id for this achievement, used to identify it in code
    [SerializeField]
    private string m_uiName;    //Name for this achievement displayed to the player in the game's UI
    [SerializeField]
    private string m_uiDescription; //Description for this achievement displayed to the player in the game's UI
    [SerializeField]
    private Sprite m_uiSprite;      //Sprite for this achievement displayed to the player as an image in the game's UI

    //Properties:
    public string id { get { return m_id; } set { m_id = value; } }
    public string uiName { get { return m_uiName; } set { m_uiName = value; } }
    public string uiDescription { get { return m_uiDescription; } set { m_uiDescription = value; } }
    public Sprite uiSprite { get { return m_uiSprite; } set { m_uiSprite = value; } }
}
