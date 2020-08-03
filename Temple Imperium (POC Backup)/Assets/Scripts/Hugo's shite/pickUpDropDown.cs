using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Controls drop down item display
//

public class pickUpDropDown : MonoBehaviour
{
    [Header("UI Details")]
    [SerializeField]
    private Image displayImage;
    [SerializeField]
    private Text itemName;
    [SerializeField]
    private Text sum;


    private bool fadingIn = false;
    private bool displaying = false;
    private bool fadingOut = false;
    private CanvasGroup group;

    private float displayTime = 0;



    private void Start()
    { 
        //Saves items canvasGroup component for easy access
        group = gameObject.GetComponent<CanvasGroup>();
    }

    public void displayItemPickUp(Sprite icon, string name, string summary)
    {
        //On item pick-up, resets all items needed for display (time elapsed, icon & text to display, etc.)
        fadingIn = true;
        displaying = true;
        displayTime = 0;
        displayImage.sprite = icon;
        itemName.text = name;
        sum.text = summary;

    }

    public void fadeIn() //Increases alpha over 1 second and resets when it reaches 1
    {
        if(group.alpha < 0.75f)
        {
            group.alpha += Time.deltaTime;
        }

        if(group.alpha >= 0.75f)
        {
            fadingIn = false;
            displaying = true;
        }
    }

    public void fadeOut() //Reduces alpha over 1 second and resets when it reaches 0
    {
        if (group.alpha > 0f)
        {
            group.alpha -= Time.deltaTime;
        }

        if(group.alpha == 0)
        {
            fadingOut = false;
            displaying = false;
        }
    }

    private void FixedUpdate()
    {
        if (displaying) //controls whether display is fading in, out, or static (on / off)
        {
            displayTime += Time.deltaTime;
        }

        if (fadingIn)
        {
            fadeIn();
        }

        if (fadingOut)
        {
            fadeOut();
        }

        if(displaying == true && displayTime > 6)
        {
            fadingOut = true;
        }
    }
}
