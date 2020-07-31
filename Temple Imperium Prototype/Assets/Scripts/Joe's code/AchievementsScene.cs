using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

//------------------------------------------------------\\
//  Provides functionality for the scene where a list   \\    
//  of achievements is displayed for the player         \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class AchievementsScene : MonoBehaviour
{
    //Set in inspector:
    [SerializeField]
    private GameObject prefabAchievementDisplay;    //The GameObject used to show an achievement in the UI
    [SerializeField]
    private Transform transformAchievements;        //The parent transform for all achievement UI elements
    [SerializeField]
    private Color colourAchievementGot;             //The colour used to represent achievements that have been completed in the UI

    // Start is called before the first frame update
    void Start()
    {
        ShowAchievementsUI();
    }

    //Shows a UI list of all achievements
    private void ShowAchievementsUI()
    {
        //Get a list of all achievements that were defined in the inspector
        AchievementsManager achievementsManager = AchievementsManager.instance;
        List<Achievement> achievements = achievementsManager.GetAchievements();

        //Destroy any existing placeholder GameObjects, ensuring the title stays
        //  as the title text should be shown before all other elements
        foreach (Transform t in transformAchievements)
        {
            if(t.gameObject.name != "Title")
                Destroy(t.gameObject);
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            //Looping through all achievements, check if the current one was already completed. This will affect how it is displayed in the UI
            bool achievementGot = achievementsManager.GetAchievementCompleted(achievements[i].id);
            //Add a new UI element as a child of transformAchievements
            GameObject goAchievementDisplay = Instantiate(prefabAchievementDisplay, transformAchievements);

            //Set the achievement name and description based on the text that was set in the inspector
            TextMeshProUGUI textName = goAchievementDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textDescription = goAchievementDisplay.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            textName.text = achievements[i].uiName;
            textDescription.text = achievements[i].uiDescription;

            //If a sprite was set, change the image on this UI element to display the sprite
            if (achievements[i].uiSprite != null)
            {
                goAchievementDisplay.transform.Find("Image").GetComponent<Image>().sprite = achievements[i].uiSprite;
            }

            //If the achievement represented by this UI element has been completed, switch the text colour
            //  from its default to the 'achievement got' colour, and remove the cover that darkens the GameObject by default
            if (achievementGot)
            {
                textName.color = colourAchievementGot;
                goAchievementDisplay.transform.Find("Cover").gameObject.SetActive(false);
            }
        }
    }

    //Returns to the main menu on button press
    public void ButtonReturnToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
