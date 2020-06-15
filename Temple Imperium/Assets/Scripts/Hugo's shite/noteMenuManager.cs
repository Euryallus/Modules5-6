using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noteMenuManager : MonoBehaviour
{
    public GameObject[] notes;
    int pointer = 0;

    private void Start()
    {
        for (int i = 0; i < notes.Length; i++)
        {
            notes[i].SetActive(false);
        }

        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void addToMenu(item pickUpItem)
    {
        if (pointer == notes.Length)
        {
            Debug.LogError("Item list Full");
        }
        else
        {
            notes[pointer].SetActive(true);
            notes[pointer].GetComponent<noteDisplay>().Display(pickUpItem);
            pointer += 1;
        }
    }

}
