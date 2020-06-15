using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class customEditor : EditorWindow
{

    int enemyNumbers;
    float timeBetweenEnemies;
    int numberOfEnemyTypes;

    GameObject spawner;

    [SerializeField]
    GameObject enemy1;
    EnemyVariant1 enemy1Code;
    [SerializeField]
    GameObject enemy2;
    EnemyVariant2 enemy2Code;

    [MenuItem("Window/Custom")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(customEditor));
        
    }

    private void OnGUI()
    {
        GUILayout.Label("Wave settings", EditorStyles.boldLabel);
        enemyNumbers = EditorGUILayout.IntField("Number of enemies in wave", enemyNumbers);
        timeBetweenEnemies = EditorGUILayout.FloatField("Time between enemies spawning", timeBetweenEnemies);
        numberOfEnemyTypes = EditorGUILayout.IntField("Enemy types to spawn", numberOfEnemyTypes);
        GUILayout.Label("^ 1 = Only small enemies \n2 = Small and Medium enemies \n3 = All enemy types (not implemented yet)", EditorStyles.helpBox);

        if(GUILayout.Button("Spawn Wave"))
        {
            spawner = GameObject.FindGameObjectWithTag("Spawner");
            spawner.GetComponent<spawnerScript>().startWave(timeBetweenEnemies, enemyNumbers, numberOfEnemyTypes);
        }

        GUILayout.Space(10);

    }

}
