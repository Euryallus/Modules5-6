using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Stores information about story items within the game
//
public class item : MonoBehaviour
{
    //
    // Allows designer to input information about items and have it displayed in the Notes menu
    //

    public string itemName;
    public string description;
    public string summary;

    public Sprite icon;
}
