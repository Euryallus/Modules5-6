using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Controls all methods needed to run the main menu (attached to Buttons)
//

public class mainMenu : MonoBehaviour
{
    public void loadScene(string sceneToLoad) // Loads scene whos name has been passed as a parameter (e.g. "MAP" loads the map scene from Build)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void optionsMenu(CanvasGroup options) // Activates or deactivates options based on whether it's been toggled on or not
    {
        if(options.alpha == 1)
        {
            options.alpha = 0; // sets menu to transparent, displables raycast target and interactivity
            options.interactable = false;
            options.blocksRaycasts = false;
        }
        else
        {
            options.alpha = 1; // opposite to comment above
            options.interactable = true;
            options.blocksRaycasts = true;
        }
    }
}
