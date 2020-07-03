using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Spawns enemies at set locations using data passed from waveData
//

public class spawnerScript : MonoBehaviour
{
    [Header("Enemy prefabs")]
        [SerializeField]
        private GameObject variant1;
        [SerializeField]
        private GameObject variant2;
        [SerializeField]
        private GameObject variant3;

    [Header("Spawn data")]
        [SerializeField]
        private int numberOfPointsPassedToEnemies = 4;
        [SerializeField]
        float spawnRadius;

    [SerializeField]
    public List<Transform> patrolPoints = new List<Transform>();

    private GameObject spawnedEnemy;
    public playStateControl stateControl;

    public bool spawning = false;

    private float timeBetween;

    private int enemyNumbers;
    private int enemyVairants;

    private int numberSpawned = 0 ;

    private int variant1Spawn;
    private int variant2Spawn;
    private int variant3Spawn;

    private int var1Count = 0;
    private int var2Count = 0;
    private int var3Count = 0;

    private int randomPatrolPoint;
    private int randomEnemy;

    private void Start()
    {
        stateControl = gameObject.GetComponent<playStateControl>();
    }

    public void startWave(float time,  int variant1, int variant2, int variant3)
    {
        //
        // ## Initialises all variables needed each wave and calls coroutine
        //

        var1Count = 0;
        var2Count = 0;
        var3Count = 0;

        variant1Spawn = variant1;
        variant2Spawn = variant2;
        variant3Spawn = variant3;

        spawning = true;

        timeBetween = time;

        enemyNumbers = variant1 + variant2 + variant3;

        StartCoroutine(waveSpawn());
    }

    public void startWave(waveData wave)
    {
        //
        // ## Initialises all variables needed each wave and calls coroutine (takes type waveData as a parameter)
        //

        numberSpawned = 0;
        var1Count = 0;
        var2Count = 0;
        var3Count = 0;

        variant1Spawn = wave.enemy1Numbers;
        variant2Spawn = wave.enemy2Numbers;
        variant3Spawn = wave.enemy3Numbers;

        spawning = true;

        timeBetween = wave.timeBetweenEnemySpawns;

        enemyNumbers = variant1Spawn + variant2Spawn + variant3Spawn;

        StartCoroutine(waveSpawn());
    }

    public void spawnEnemy(GameObject enemyToSpawn)
    {
        //
        // Instantiates enemy passed in parameters, assigns it 3+ patrol points from list defined in inspector, and assigns enemy a position within X meters of the spawner
        //

        spawnedEnemy = Instantiate(enemyToSpawn);
    
        List<Transform> pointsPassed = new List<Transform>();

        for (int j = 0; j < numberOfPointsPassedToEnemies; j++) //assigns [numberOfPointsPassedToEnemies] patrol points
        {
            randomPatrolPoint = Random.Range(0, patrolPoints.Count); //generates randsom point from list

            while (pointsPassed.Contains(patrolPoints[randomPatrolPoint]))
            {
                randomPatrolPoint = Random.Range(0, patrolPoints.Count); //if item already exists in points to pass, generate another
            }
           
            
           pointsPassed.Add(patrolPoints[randomPatrolPoint]);
        }
       
        spawnedEnemy.GetComponent<Enemy>().patrolPoints = pointsPassed; //pass points to newly spawned enemy
       
        spawnedEnemy.transform.position = (Random.insideUnitSphere * spawnRadius + gameObject.transform.position); //assign random position around spawner

    }

    public IEnumerator waveSpawn() //cycles each type of enemy every X seconds until desired number are spawned (e.g. all type 1's spawned, then type 2's, then type 3's)
    {
        while(numberSpawned < enemyNumbers)
        {
            if(var1Count < variant1Spawn)
            {
                spawnEnemy(variant1);
                var1Count += 1;
                numberSpawned+= 1;
            }

            else if(var2Count < variant2Spawn)
            {
                spawnEnemy(variant2);
                var2Count += 1;
                numberSpawned += 1;

            }

            else if (var3Count < variant3Spawn)
            {
                spawnEnemy(variant3);
                var3Count += 1;
                numberSpawned =+ 1;

            }

            yield return new WaitForSeconds(timeBetween);

        }

        spawning = false;
    }
}
