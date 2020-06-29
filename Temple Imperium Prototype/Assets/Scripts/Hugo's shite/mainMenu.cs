using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public void loadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void optionsMenu(CanvasGroup options)
    {
        if(options.alpha == 1)
        {
            options.alpha = 0;
            options.interactable = false;
            options.blocksRaycasts = false;
        }
        else
        {
            options.alpha = 1;
            options.interactable = true;
            options.blocksRaycasts = true;
        }
    }
}
