using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class customEditor : EditorWindow
{
    float timeBetweenEnemies;
    int variant1;
    int variant2;
    int variant3;

    float waveLength;

    GameObject[] spawner;

    [MenuItem("Window/Custom")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(customEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Wave settings", EditorStyles.boldLabel);

        timeBetweenEnemies = EditorGUILayout.FloatField("Time between enemies spawning", timeBetweenEnemies);

        variant1 = EditorGUILayout.IntField("Number of variant 1 enemies", variant1);
        variant2 = EditorGUILayout.IntField("Number of variant 2 enemies", variant2);
        variant3 = EditorGUILayout.IntField("Number of variant 3 enemies", variant3);

        waveLength = EditorGUILayout.FloatField("Wave length in secoonds", waveLength);

        if(GUILayout.Button("Spawn Wave"))
        {
            spawner = GameObject.FindGameObjectsWithTag("Spawner");

            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i].GetComponent<spawnerScript>().startWave(timeBetweenEnemies, variant1, variant2, variant3);
            }

            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().initiateWave(waveLength);

            Debug.Log(GameObject.FindGameObjectWithTag("spawnerManager").name);
        }
    }
}
