using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Allows wave progression checks to be made by allowing all 'Enemies' within a scene to be destroyed upon BACKSPACE input
// ## TO BE DELETED
//
public class DEBUGREASONS : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) //checks for input, on Backspace input cycles each 'Enemy' and destroys it
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
        }
    }
}
