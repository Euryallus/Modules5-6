using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pickUpDropDown : MonoBehaviour
{
    public Image displayImage;
    public Text itemName;
    public Text sum;


    bool fadingIn = false;
    bool displaying = false;
    bool fadingOut = false;

    public float displayTime = 0;

    CanvasGroup group;

    private void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>();
    }

    public void displayItemPickUp(Sprite icon, string name, string summary)
    {
        fadingIn = true;
        displaying = true;
        displayTime = 0;
        displayImage.sprite = icon;
        itemName.text = name;
        sum.text = summary;

    }

    public void fadeIn()
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

    public void fadeOut()
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
        if (displaying)
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
