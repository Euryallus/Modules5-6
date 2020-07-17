using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager instance;

    //Set in inspector:
    [SerializeField]
    private List<Achievement> achievements;
    [SerializeField]
    private GameObject prefabAchievementPopup;
    [SerializeField]
    private string achievementPopupSoundName;
    [SerializeField]
    private Transform transformAchievementCanvas;

    private Dictionary<string, Achievement> achievementsDict;

    public List<Achievement> GetAchievements()
    {
        return achievements;
    }

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
        achievementsDict = new Dictionary<string, Achievement>();
        for (int i = 0; i < achievements.Count; i++)
        {
            achievementsDict.Add(achievements[i].id, achievements[i]);
        }
    }

    public void SetAchievementCompleted(string achievementId)
    {
        if (achievementsDict.ContainsKey(achievementId))
        {
            if(PlayerPrefs.GetInt(achievementId, 0) == 0)
            {
                StartCoroutine(ShowAchievementPopupAfterDelay(achievementId));

                PlayerPrefs.SetInt("Achievements_" + achievementId, 1);
            }
        }
        else
        {
            Debug.LogError("Trying to set achievement with unknown id: " + achievementId);
        }
        PlayerPrefs.Save();
    }

    public bool GetAchievementCompleted(string achievementId)
    {
        if (achievementsDict.ContainsKey(achievementId))
        {
            return (PlayerPrefs.GetInt("Achievements_" + achievementId, 0) == 1);
        }
        else
        {
            Debug.LogError("Trying to get achievement with unknown id: " + achievementId);
            return false;
        }
    }

    private IEnumerator ShowAchievementPopupAfterDelay(string achievementId)
    {
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySoundEffect2D(achievementPopupSoundName);

        GameObject goPopup = Instantiate(prefabAchievementPopup, transformAchievementCanvas);
        goPopup.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = achievementsDict[achievementId].uiName;
    }
}

[Serializable]
public struct Achievement
{
    //Set in inspector:
    [SerializeField]
    private string m_id;
    [SerializeField]
    private string m_uiName;
    [SerializeField]
    private string m_uiDescription;
    [SerializeField]
    private Sprite m_uiSprite;

    //Properties:
    public string id { get { return m_id; } set { m_id = value; } }
    public string uiName { get { return m_uiName; } set { m_uiName = value; } }
    public string uiDescription { get { return m_uiDescription; } set { m_uiDescription = value; } }
    public Sprite uiSprite { get { return m_uiSprite; } set { m_uiSprite = value; } }
}
