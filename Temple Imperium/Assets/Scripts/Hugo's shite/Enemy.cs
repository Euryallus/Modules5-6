﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class Enemy : MonoBehaviour
{
    //altered per object
    [Header("Class specific")]

    [SerializeField]
    protected float viewDistance;


    protected float viewConeAngle;

    [SerializeField]
    protected float transitionTime;

    [SerializeField]
    public float enemySpeed;

    [SerializeField]
    public float enemyHealth;
    [SerializeField]
    protected float regenPerSecond = 0.5f;
    private float enemyMaxHealth;
    [SerializeField]
    private GameObject healthBar;

    private float secondsPassed = 0;


    //enemy base variables
    protected NavMeshAgent agent;
    protected GameObject player;

    int investigationPointNo = 3;

    [SerializeField]
    GameObject currentTarget;

    protected enum State
    {
        Idle,
        Patrol,
        Investigate,
        Engage
    }

    [SerializeField]
    protected State currentState;

    //PATROL behaviour
    public new List<Transform> patrolPoints = new List<Transform>();

    protected int currentPatrolPoint = 0;

    //INVESTIGATE behaviour
    List<Vector3> investigatePoints = new List<Vector3>();
    int investigatePointer = 0;

    //CheckForPlayer variables

    protected Vector3 playerVector;
    protected Vector3 lastKnownPos;

    [SerializeField]
    protected bool canSeePlayer = false;

    [SerializeField]
    protected float enemyViewAngle = 120f;

    protected float playerDist;
    [SerializeField]
    protected float lookingCount = 0;

    public Enemy (float viewDist, float viewAngle, float transitionWait, float speed)
    {
        viewDistance = viewDist;
        enemyViewAngle = viewAngle;
        transitionTime = transitionWait;
        enemySpeed = speed;
    }

    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
        lastKnownPos = gameObject.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Patrol;

        enemyMaxHealth = enemyHealth;
        healthBar = transform.GetChild(0).gameObject;

        StartCoroutine(regenHealth());
    }

    private void Update()
    {
        checkForPlayer();

        switch (currentState)
        {
            case State.Idle:

                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Investigate:
                Investigate();
                break;

            case State.Engage:
                Engage();
                break;
        }

        //gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,0,0);

    }
    public virtual void checkForPlayer()
    {
        playerDist = Vector3.Distance(player.transform.position, gameObject.transform.position);
        Debug.DrawRay(transform.position, (transform.forward * viewDistance), Color.green);

        playerVector = player.transform.position - gameObject.transform.position;

        viewConeAngle = Vector3.Angle(playerVector.normalized * playerDist, transform.forward);

        RaycastHit hit;
       
        if (playerDist < viewDistance && viewConeAngle < enemyViewAngle / 2 && Physics.Raycast(transform.position, playerVector.normalized, out hit, playerDist) && hit.transform.gameObject.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, playerVector.normalized * playerDist, Color.red, 0.25f, false);

            lookingCount = 0;
            lastKnownPos = player.transform.position;
            canSeePlayer = true;

            currentState = State.Engage;
            currentTarget = hit.transform.gameObject;
        }
        else
        {
            canSeePlayer = false;
        }
    }

    public virtual void Engage()
    {
        if (canSeePlayer == false)
        {
            lookingCount += Time.deltaTime;

            if(lookingCount > transitionTime)
            {
                agent.SetDestination(lastKnownPos);
                if(Vector3.Distance(transform.position, lastKnownPos) < 1f)
                {
                    currentState = State.Investigate;
                    investigatePoints = new List<Vector3>();
                    lookingCount = 0;
                }
            }
        }
    }

    public virtual void Patrol()
    {
        if (agent.remainingDistance < 0.5f)
        {
            currentPatrolPoint += 1;
            if (currentPatrolPoint == patrolPoints.Count)
            {
                currentPatrolPoint = 0;
            }
        }
        agent.SetDestination(patrolPoints[currentPatrolPoint].position);
    }

    public virtual void Investigate()
    {
        if(investigatePoints.Count == 0)
        {
            investigatePointer = 0;

            for (int i = 0; i < investigationPointNo; i++)
            {
                Vector3 investigatePoint = transform.position + Random.insideUnitSphere * 10; 

                while(agent.CalculatePath(investigatePoint, agent.path) != true || investigatePoint.y > 3)
                {
                    investigatePoint = transform.position + Random.insideUnitSphere * 10;
                }

                investigatePoints.Add(investigatePoint);
                Debug.Log(investigatePoint);
            }
        }

        if(Vector3.Distance(transform.position, investigatePoints[investigatePointer]) <= 1f)
        {
            investigatePointer += 1;

            if(investigatePointer == investigatePoints.Count)
            {
                currentState = State.Patrol;
                return;
            }
        }
    }

    public void setOnFire(int fireEffectTime, int fireDamageTaken, float timeBetweenDamage)
    {
           secondsPassed = 0;
           StartCoroutine(putOutFire(fireEffectTime, fireDamageTaken, timeBetweenDamage));
       
           transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
    }

    protected IEnumerator regenHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            enemyHealth += regenPerSecond;
        }
    }

    protected IEnumerator putOutFire(int fireEffectTime, int fireDamageTaken, float timeBetweemDamage)
    {
        while(secondsPassed < fireEffectTime)
        {
            Damage(fireDamageTaken);
            secondsPassed += timeBetweemDamage;
            yield return new WaitForSeconds(timeBetweemDamage);

            if(secondsPassed >= fireEffectTime)
            {
                transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    //Added by Joe: //thank u Joe
    public void Damage(int hitPoints)
    {
        enemyHealth -= hitPoints;

        float healthBarX = enemyHealth / enemyMaxHealth;
        healthBar.transform.localScale = new Vector3(healthBarX, 1, 1);

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SoundEffectPlayer.instance.PlaySoundEffect("Believe", true, transform.position, 1f, 0.95f, 1.05f);
        Destroy(gameObject);
    }
}


