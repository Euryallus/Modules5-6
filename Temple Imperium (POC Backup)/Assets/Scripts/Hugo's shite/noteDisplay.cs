using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Manages each note entry in the Notes menu
//

public class noteDisplay : MonoBehaviour
{
    [Header("UI Elements")]
        [SerializeField]
        private Image iconDisplay;
        [SerializeField]
        private Text nameDisplay;
        [SerializeField]
        private Text descriptionDisplay;


    public void Display(item itemPickUp)
    {
        //
        // Sets UI elements on item menu to name & Description of item collected
        //

        nameDisplay.text = itemPickUp.itemName;
        iconDisplay.sprite = itemPickUp.icon;

        descriptionDisplay.text = itemPickUp.description;
    }
}
