using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//------------------------------------------------------\\
//  Stores a list of all available achievements and     \\
//  manages achievement functionality, e.g. setting     \\ 
//  achievements as completed, showing UI popups        \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class AchievementsManager : MonoBehaviour
{
    //A single static instance of this class will exist in every scene
    //  so it can easily be used by other scripts at any time
    public static AchievementsManager instance;

    //Set in inspector:
    [SerializeField]
    private List<Achievement> achievements;     //List of all available achievements
    [SerializeField]
    private GameObject prefabAchievementPopup;  //The UI popup that appears when the player gets an achievement
    [SerializeField]
    private string achievementPopupSoundName;   //Name of the sound that plays when the player gets an achievement
    [SerializeField]
    private Transform transformAchievementCanvas;   //The canvas for showing achievement popups, set to appear in front
                                                    //  of all other canvases so popups are always visible and scaled consistently

    private Dictionary<string, Achievement> achievementsDict;   //A dictionary of achievements which are indexed by their id's, allows achievements
                                                                // to easily be accessed by their id's rather than searching through a list

    public List<Achievement> GetAchievements() { return achievements; }

    private void Awake()
    {
        //Ensure that an instance of the class does not already exist
        if (instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        SetupAchievementsDict();
    }

    private void SetupAchievementsDict()
    {
        //Add all achievements that were defined in the inspector to
        //  the dictionary where they are indexed by their unique id's

        achievementsDict = new Dictionary<string, Achievement>();
        for (int i = 0; i < achievements.Count; i++)
        {
            achievementsDict.Add(achievements[i].id, achievements[i]);
        }
    }

    public void SetAchievementCompleted(string achievementId)
    {
        //If the achievements with the given id was not already completed,
        //  set is as completed in PlayerPrefs and show a UI popup to notify the player
        if (achievementsDict.ContainsKey(achievementId))
        {
            if(PlayerPrefs.GetInt("Achievements_" + achievementId, 0) == 0)
            {
                StartCoroutine(ShowAchievementPopupAfterDelay(achievementId));

                PlayerPrefs.SetInt("Achievements_" + achievementId, 1);
                PlayerPrefs.Save(); //Saving to PlayerPrefs so achievement info persists through play sessions
            }
        }
        else
        {
            //No achievement found with the given id, throw an error
            Debug.LogError("Trying to set achievement with unknown id: " + achievementId);
        }
    }

    public bool GetAchievementCompleted(string achievementId)
    {
        //Returns true or false based on if the achievement with
        //  the given id was already completed
        if (achievementsDict.ContainsKey(achievementId))
        {
            //In PlayerPrefs, achievements are stored as 0 (not completed), or 1 (completed),
            //  so checking if the value equals 1 converts this int value to a bool
            return (PlayerPrefs.GetInt("Achievements_" + achievementId, 0) == 1);
        }
        else
        {
            //No achievement found with the given id, throw an error
            Debug.LogError("Trying to get achievement with unknown id: " + achievementId);
            return false;
        }
    }

    private IEnumerator ShowAchievementPopupAfterDelay(string achievementId)
    {
        //Wait for a second in case this achievement was triggered just before a scene change,
        //  ensures the player's attention will be on the popup rather than the scene transition
        yield return new WaitForSeconds(1f);
        //Play the achievement get sound
        AudioManager.instance.PlaySoundEffect2D(achievementPopupSoundName);

        //Show a UI popup with the name of the achievement with the id that was passed
        GameObject goPopup = Instantiate(prefabAchievementPopup, transformAchievementCanvas);
        goPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = achievementsDict[achievementId].uiName;
    }
}