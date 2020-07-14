using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CodexScene : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private List<GameObject> infoGameObjects;
    [SerializeField]
    private TextMeshProUGUI textEnemy1Button;
    [SerializeField]
    private TextMeshProUGUI textEnemy2Button;
    [SerializeField]
    private TextMeshProUGUI textEnemy3Button;
    [SerializeField]
    private TextMeshProUGUI textBossButton;

    void Start()
    {
        ShowInfoGameObjectAtIndex(0);

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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowInfoGameObjectAtIndex(int index)
    {
        ShowInfoGameObject(infoGameObjects[index].name);
    }

    private void ShowInfoGameObject(string name)
    {
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

    public void ButtonShowInfoGameObject(string name)
    {
        ShowInfoGameObject(name);
    }
    public void ButtonShowInfoGameObjectConditional(string showInfo)
    {
        string infoGameObjectName = showInfo.Remove(showInfo.IndexOf("*"));
        string showConditionName = showInfo.Substring(showInfo.IndexOf("*") + 1);
        if(SaveLoadManager.instance.LoadIntFromPlayerPrefs(showConditionName) == 1)
        {
            ShowInfoGameObject(infoGameObjectName);
        }
    }
    public void ButtonReturnToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
