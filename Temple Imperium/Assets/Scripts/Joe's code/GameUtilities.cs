using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtilities : MonoBehaviour
{
    public static GameUtilities instance;

    public Color colourPurplePower;
    public Color colourOrangeHeat;
    public Color colourBlueIce;
    public Color colourPinkHeal;

    private void Awake()
    {
        //Ensure that an instance of the Utilities class does not already exist
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
