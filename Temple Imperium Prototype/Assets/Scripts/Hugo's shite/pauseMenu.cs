﻿using System.Collections;
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
    private static bool showingPause = false;
    private GameObject player;

    //Added by Joe, allows other scripts to check if the pause menu is being shown
    //  e.g. used to prevent weapon inputs when paused
    public static bool GetPauseMenuShowing()
    {
        return showingPause;
    }

    private void Start()
    {
        showingPause = false;
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
            Cursor.visible = showingPause;
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

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Added by Joe:
    //Loads the codex scene on button press
    public void LoadCodexScene()
    {
        SceneManager.LoadScene("CodexScene");
    }
}
