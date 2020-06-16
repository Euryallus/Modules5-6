using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class spawnerScript : MonoBehaviour
{
    [SerializeField]
    public List<Transform> patrolPoints = new List<Transform>();

    public playStateControl stateControl;

    public float spawnCount = 0;

    [SerializeField]
    private GameObject variant1;
    [SerializeField]
    private GameObject variant2;
    [SerializeField]
    private GameObject variant3;

    [SerializeField]
    int randomPatrolPoint;

    int randomEnemy;
    GameObject spawnedEnemy;

    [SerializeField]
    int numberOfPointsPassedToEnemies = 4;

    public bool spawning = false;

    float timeBetween;
    int enemyNumbers;
    int enemyVairants;

    int numberSpawned = 0 ;

    int variant1Spawn;
    int variant2Spawn;
    int variant3Spawn;

    int var1Count = 0;
    public int var2Count = 0;
    int var3Count = 0;

    [SerializeField]
    float spawnRadius;


    private void Start()
    {
        stateControl = gameObject.GetComponent<playStateControl>();
    }

    public void startWave(float time,  int variant1, int variant2, int variant3)
    {
        

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

    public void spawnEnemy(GameObject enemyToSpawn)
    {
        spawnedEnemy = Instantiate(enemyToSpawn);
    
        List<Transform> pointsPassed = new List<Transform>();

        for (int j = 0; j < numberOfPointsPassedToEnemies; j++)
        {
           randomPatrolPoint = Random.Range(0, patrolPoints.Count);
           pointsPassed.Add(patrolPoints[randomPatrolPoint]);
        }
       
        spawnedEnemy.GetComponent<Enemy>().patrolPoints = pointsPassed;
       
        spawnedEnemy.transform.position = (Random.insideUnitSphere * spawnRadius + gameObject.transform.position);

    }

    public IEnumerator waveSpawn()
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
