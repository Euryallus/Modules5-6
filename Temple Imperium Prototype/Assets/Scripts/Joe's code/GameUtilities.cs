using UnityEngine;

//------------------------------------------------------\\
//  GameUtilities persists through all scenes and       \\
//  contains useful objects/properties that can be      \\
//  used by any scripts                                 \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class GameUtilities : MonoBehaviour
{
    public static GameUtilities instance;

    //All properties set in inspector:

    //Colours for StarStone abilities
    public Color colourPurplePower;
    public Color colourOrangeHeat;
    public Color colourBlueIce;
    public Color colourPinkHeal;

    //Materials for StarStone abilities:
    public Material materialPower;
    public Material materialHeat;
    public Material materialIce;
    public Material materialHeal;
    //Material for object break effect:
    public Material materialBreak;

    //Particles used for object break effect:
    public GameObject prefabBreakParticles;
    public GameObject prefabDistortionSphere;

    //Bullet hole to spawn when a wall it shot
    public GameObject prefabBulletHole;

    private void Awake()
    {
        //Ensure that an instance of the GameUtilities class does not already exist
        if (instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
