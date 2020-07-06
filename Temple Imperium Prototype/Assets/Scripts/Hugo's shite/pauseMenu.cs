using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Controls all methods needed to run the pause menu
//

public class pauseMenu : MonoBehaviour
{
    private CanvasGroup group;
    private bool showingPause = false;
    private GameObject player;

    private void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>(); //assigns canvas group component for later use
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && player.GetComponent<playerHealth>().isDead() == false) //on Escape input;
        {
            showingPause = !showingPause; //value of showingPause is inverted

            //alters menu attributes according to showingPause value
            Time.timeScale = showingPause ? 0 : 1; 
            group.alpha = showingPause ? 1 : 0;

            group.interactable = showingPause ? true : false;
            group.blocksRaycasts = showingPause ? true : false;

            Cursor.lockState = showingPause? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

    public void quitGame()
    {
        //Exits application in built application (doesn't function in Editor)
        Application.Quit();
    }

    public void backToMainMenu()
    {
        //Loads mainMenu scene
        SceneManager.LoadScene("mainMenu");
    }
}
