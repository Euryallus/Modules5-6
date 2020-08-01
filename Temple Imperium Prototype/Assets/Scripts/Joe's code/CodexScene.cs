using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//------------------------------------------------------\\
//  CodexScene displays UI with information about       \\
//  all weapon and enemy types in the game              \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class CodexScene : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private List<GameObject> infoGameObjects;   //All UI panel GameObjects containing info about weapons/enemies
    [SerializeField]
    private TextMeshProUGUI textEnemy1Button;   //UI text for each enemy type's button -
    [SerializeField]                            //  button text is changed
    private TextMeshProUGUI textEnemy2Button;   //  based on whether the
    [SerializeField]                            //  corresponding enemy
    private TextMeshProUGUI textEnemy3Button;   //  has been killed
    [SerializeField]                            //  by the player
    private TextMeshProUGUI textBossButton;     //  -------------

    void Start()
    {
        //Show the first info panel by default
        ShowInfoGameObjectAtIndex(0);

        //For each enemy type, if they have never been killed by the player,
        //  change the corresponding button text to '???' to show their
        //  codex entry has not been unlocked
        SaveLoadManager slm = SaveLoadManager.instance;
        if(slm.LoadIntFromPlayerPrefs("PlayerKilledEnemy1") == 0)
        {
            textEnemy1Button.text = "???";
        }
        if (slm.LoadIntFromPlayerPrefs("PlayerKilledEnemy2") == 0)
        {
            textEnemy2Button.text = "???";
        }
        if (slm.LoadIntFromPlayerPrefs("PlayerKilledEnemy3") == 0)
        {
            textEnemy3Button.text = "???";
        }
        if (slm.LoadIntFromPlayerPrefs("PlayerKilledBoss") == 0)
        {
            textBossButton.text = "???";
        }
    }

    //Shows a UI info panel at the passed index in the list
    private void ShowInfoGameObjectAtIndex(int index)
    {
        ShowInfoGameObject(infoGameObjects[index].name);
    }

    //Shows a UI info panel based on the passed name
    private void ShowInfoGameObject(string name)
    {
        //Show the GameObject with the given name and hide
        //  all others to prevent multiple from showing at once
        for (int i = 0; i < infoGameObjects.Count; i++)
        {
            if (infoGameObjects[i].name == name)
            {
                infoGameObjects[i].SetActive(true);
            }
            else
            {
                infoGameObjects[i].SetActive(false);
            }
        }
    }

    #region UI Buttons

    //Shows a UI info panel based on the passed name on button press
    public void ButtonShowInfoGameObject(string name)
    {
        ShowInfoGameObject(name);
    }

    //Used for buttons that should only show an info panel if a certain condition is met.
    public void ButtonShowInfoGameObjectConditional(string showInfo)
    {
        //The condition required for the panel to be unlocked
        //  is marked by the presence of '*', followed by the condition name
        //  which should match the name used to store the value in PlayerPrefs
        string infoGameObjectName = showInfo.Remove(showInfo.IndexOf("*"));
        string showConditionName = showInfo.Substring(showInfo.IndexOf("*") + 1);
        //If the condition was met (1 == true, 0 == false), show the UI info panel
        if(SaveLoadManager.instance.LoadIntFromPlayerPrefs(showConditionName) == 1)
        {
            ShowInfoGameObject(infoGameObjectName);
        }
    }

    //Loads the main menu scene on button press
    public void ButtonReturnToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    #endregion
}
