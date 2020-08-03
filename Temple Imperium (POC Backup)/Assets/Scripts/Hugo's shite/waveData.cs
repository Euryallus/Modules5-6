using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Custom data created for each wave of combat, created by designer and altered in inspector
//

[CreateAssetMenu(fileName ="Wave", menuName = "Waves/New Wave")] //allows waveData items to be created in project assets
public class waveData : ScriptableObject
{
    [SerializeField]
    public int waveNumber { get; set; }
    public float timeBetweenEnemySpawns;
    public int enemy1Numbers;
    public int enemy2Numbers;
    public int enemy3Numbers;
    public float waveLength;

    public float downtime;

    public waveData(int number, float timeBetween, int type1, int type2, int type3, float length, float down) //data available to change in inspector, and accessable by referencing item in script (used in playStateControl)
    {
        waveNumber = number;
        timeBetweenEnemySpawns = timeBetween;
        enemy1Numbers = type1;
        enemy2Numbers = type2;
        enemy3Numbers = type3;
        waveLength = length;
        downtime = down;
    } 

}
