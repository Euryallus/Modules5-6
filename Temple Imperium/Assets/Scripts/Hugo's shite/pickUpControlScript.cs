﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpControlScript : MonoBehaviour
{
    public GameObject camera;

    public RaycastHit hit;
    public GameObject hitObject;
    public item pickUpItem;

    public GameObject display;
    public GameObject noteMenu;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        display = GameObject.FindGameObjectWithTag("display");
        noteMenu = GameObject.FindGameObjectWithTag("noteDisplayManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 3f))
        {
           
            hitObject = hit.transform.gameObject;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hitObject.CompareTag("PickUp") && hitObject.GetComponent<item>() == true)
                {
                    pickUpItem = hitObject.GetComponent<item>();

                    Debug.Log(pickUpItem.icon);
                    Debug.Log(pickUpItem.itemName);

                    noteMenu.GetComponent<noteMenuManager>().addToMenu(pickUpItem);
                    display.GetComponent<pickUpDropDown>().displayItemPickUp(pickUpItem.icon, pickUpItem.itemName, pickUpItem.summary);
                    Destroy(hitObject);
                }
            }
        }

        Debug.DrawLine(camera.transform.position, camera.transform.position + (camera.transform.forward * 3));
    }
}