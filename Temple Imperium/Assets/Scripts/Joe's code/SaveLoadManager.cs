using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    private readonly Dictionary<string, string> playerPrefsDefaultStringDict = new Dictionary<string, string>
    {
    };
    private readonly Dictionary<string, int> playerPrefsDefaultIntDict = new Dictionary<string, int>
    {
    };
    private readonly Dictionary<string, float> playerPrefsDefaultFloatDict = new Dictionary<string, float>
    {
        { "Options_Volume_Music", 1f },
        { "Options_Volume_Sound", 1f }
    };

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
    }

    //==============PLAYER PREFS====================================

    public void SaveStringToPlayerPrefs(string key, string val)
    {
        PlayerPrefs.SetString(key, val);
        PlayerPrefs.Save();
    }
    public void SaveIntToPlayerPrefs(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
        PlayerPrefs.Save();
    }
    public void SaveFloatToPlayerPrefs(string key, float val)
    {
        PlayerPrefs.SetFloat(key, val);
        PlayerPrefs.Save();
    }

    public string LoadStringFromPlayerPrefs(string key)
    {
        if (playerPrefsDefaultStringDict.ContainsKey(key))
        {
            return PlayerPrefs.GetString(key, playerPrefsDefaultStringDict[key]);
        }
        else
        {
            Debug.LogError("No default value for PlayerPrefs string key: " + key);
            return PlayerPrefs.GetString(key);
        }
    }
    public int LoadIntFromPlayerPrefs(string key)
    {
        if (playerPrefsDefaultIntDict.ContainsKey(key))
        {
            //Debug.Log("Loading int " + key + ": " + PlayerPrefs.GetInt(key, playerPrefsDefaultIntDict[key]));
            return PlayerPrefs.GetInt(key, playerPrefsDefaultIntDict[key]);
        }
        else
        {
            Debug.LogError("No default value for PlayerPrefs int key: " + key);
            return PlayerPrefs.GetInt(key);
        }
    }
    public float LoadFloatFromPlayerPrefs(string key)
    {
        if (playerPrefsDefaultFloatDict.ContainsKey(key))
        {
            return PlayerPrefs.GetFloat(key, playerPrefsDefaultFloatDict[key]);
        }
        else
        {
            Debug.LogError("No default value for PlayerPrefs float key: " + key);
            return PlayerPrefs.GetFloat(key);
        }
    }
}
