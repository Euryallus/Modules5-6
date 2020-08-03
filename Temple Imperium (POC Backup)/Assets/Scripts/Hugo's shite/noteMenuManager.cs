using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Manages which notes are visible on the Notes menu and adds entries according to items collected
//

public class noteMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] notes; //Blank note templates input by designer
    private int pointer = 0;

    private void Start()
    {
        for (int i = 0; i < notes.Length; i++) //initially, all are disabled
        {
            notes[i].SetActive(false);
        }

        gameObject.GetComponent<CanvasGroup>().alpha = 0; //canvas group transparency set to 0
    }

    public void addToMenu(item pickUpItem)
    {
        if (pointer == notes.Length)
        {
            Debug.LogError("Item list Full");
        }
        else
        {
            // If notes list isn't full, activate next blank note and pass 'item' class info to the next template to fill
            notes[pointer].SetActive(true);
            notes[pointer].GetComponent<noteDisplay>().Display(pickUpItem);
            pointer += 1; //pointer increases by 1
        }
    }

}
