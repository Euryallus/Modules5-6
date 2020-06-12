using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noteDisplay : MonoBehaviour
{
    public Image iconDisplay;
    public Text nameDisplay;
    public Text descriptionDisplay;
    void Start()
    {
       //iconDisplay = gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
       //nameDisplay = gameObject.transform.GetChild(2).gameObject.GetComponent<Text>();
       //descriptionDisplay = gameObject.transform.GetChild(3).gameObject.GetComponent<Text>();
    }

    public void Display(item itemPickUp)
    {
        nameDisplay.text = itemPickUp.itemName;
        iconDisplay.sprite = itemPickUp.icon;

        descriptionDisplay.text = itemPickUp.description;
    }
}
