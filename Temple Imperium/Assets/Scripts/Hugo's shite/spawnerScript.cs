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


    private void Start()
    {
        stateControl = gameObject.GetComponent<playStateControl>();
    }

    public void FixedUpdate()
    {
        spawnCount += Time.deltaTime;

        if(spawning && spawnCount > timeBetween)
        {
            spawnEnemy();
            spawnCount = 0;
        }
    }

    public void startWave(float time, int numberOfEnemies, int enemyTypes)
    {
        //enemyTypes = 1 - only small enemies
        //enemyTypes = 2 - small + medium
        //enemyTypes = 3 - all 3 variants

        spawning = true;

        timeBetween = time;
        enemyNumbers = numberOfEnemies;
        enemyVairants = enemyTypes;

        numberSpawned = 0;
    }

    public void spawnEnemy()
    {
        List<Transform> pointsPassed = new List<Transform>();

        randomEnemy = Random.Range(0, enemyVairants);
       
        switch (randomEnemy) {
            case 0:
                spawnedEnemy = Instantiate(variant1);
                break;
            case 1:
                spawnedEnemy = Instantiate(variant2);
                break;
            case 2:
                spawnedEnemy = Instantiate(variant3);
                break;
       
        }
       
        for (int j = 0; j < numberOfPointsPassedToEnemies; j++)
        {
           randomPatrolPoint = Random.Range(0, patrolPoints.Count);
           pointsPassed.Add(patrolPoints[randomPatrolPoint]);
        }
       
        spawnedEnemy.GetComponent<Enemy>().patrolPoints = pointsPassed;
       
        spawnedEnemy.transform.position = gameObject.transform.position;
        if (numberSpawned == 0)
        {
            stateControl.initiateWave(1.5f);
        }

        numberSpawned += 1;

        if(numberSpawned == enemyNumbers)
        {
            spawning = false;
        }

    }
}
