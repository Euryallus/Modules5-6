using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class AchievementsScene : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private GameObject prefabAchievementDisplay;
    [SerializeField]
    private Transform transformAchievements;
    [SerializeField]
    private Color colourAchievementGot;

    // Start is called before the first frame update
    void Start()
    {
        ShowAchievementsUI();
    }

    private void ShowAchievementsUI()
    {
        AchievementsManager achievementsManager = AchievementsManager.instance;
        List<Achievement> achievements = achievementsManager.GetAchievements();

        foreach (Transform t in transformAchievements)
        {
            if(t.gameObject.name != "Title")
                Destroy(t.gameObject);
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            bool achievementGot = achievementsManager.GetAchievementCompleted(achievements[i].id);
            GameObject goAchievementDisplay = Instantiate(prefabAchievementDisplay, transformAchievements);

            TextMeshProUGUI textName = goAchievementDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textDescription = goAchievementDisplay.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            textName.text = achievements[i].uiName;
            textDescription.text = achievements[i].uiDescription;
            if (achievements[i].uiSprite != null)
            {
                goAchievementDisplay.transform.Find("Image").GetComponent<Image>().sprite = achievements[i].uiSprite;
            }

            if (achievementGot)
            {
                textName.color = colourAchievementGot;
                goAchievementDisplay.transform.Find("Cover").gameObject.SetActive(false);
            }
        }
    }

    public void ButtonReturnToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
