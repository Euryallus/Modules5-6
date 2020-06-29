using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    CanvasGroup group;
    bool showingPause = false;

    private void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showingPause = !showingPause;
            Time.timeScale = showingPause ? 0 : 1;
            group.alpha = showingPause ? 1 : 0;

            group.interactable = showingPause ? true : false;
            group.blocksRaycasts = showingPause ? true : false;

            Cursor.lockState = showingPause? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void backToMainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
