using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Wave spawning GUI window for Designers use (testing enemy waves) 
//

[System.Serializable]
public class customEditor : EditorWindow
{
    float timeBetweenEnemies;
    int variant1;
    int variant2;
    int variant3;

    float waveLength;

    GameObject[] spawner;

    [MenuItem("Window/Custom")] //location of window in Window tab
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(customEditor)); 
    }

    private void OnGUI()
    {
        GUILayout.Label("Wave settings", EditorStyles.boldLabel);

        //
        // ## Allows input of key wave information (e.g. enemy numbers, wave timer, etc.)
        // #################################################################################################### //
        timeBetweenEnemies = EditorGUILayout.FloatField("Time between enemies spawning", timeBetweenEnemies); 

        variant1 = EditorGUILayout.IntField("Number of variant 1 enemies", variant1);
        variant2 = EditorGUILayout.IntField("Number of variant 2 enemies", variant2);
        variant3 = EditorGUILayout.IntField("Number of variant 3 enemies", variant3);

        waveLength = EditorGUILayout.FloatField("Wave length in secoonds", waveLength);

        // #################################################################################################### //

        if (GUILayout.Button("Spawn Wave"))
        {
            //
            // ## ON BUTTON PRESS   
            // ## locates all 'spawners' in the scene and starts a 'wave' according to input values above
            // 

            spawner = GameObject.FindGameObjectsWithTag("Spawner");
            for (int i = 0; i < spawner.Length; i++)
            {
                spawner[i].GetComponent<spawnerScript>().startWave(timeBetweenEnemies, variant1, variant2, variant3);
            }

            waveData newWave = new waveData(0, timeBetweenEnemies, variant1, variant2, variant3, waveLength, 30); //creates object newWave of type waveData based on input values

            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().initiateWave(newWave); //calls initiateWave within spawnerManager to allow timer, win and lose conditions to function
        }
    }
}
