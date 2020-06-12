using System.Collections;
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

    [SerializeField]
    protected float viewConeAngle;

    [SerializeField]
    protected float transitionTime;

    [SerializeField]
    protected float enemySpeed;

    [SerializeField]
    protected float enemyHealth;


    //enemy base variables
    protected NavMeshAgent agent;
    protected GameObject player;

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


    //CheckForPlayer variables

    protected Vector3 playerVector;
    protected Vector3 lastKnownPos;

    [SerializeField]
    protected bool canSeePlayer = false;

    
    protected float enemyViewAngle = 120f;
    protected float playerDist;
    [SerializeField]
    protected float lookingCount = 0;

    public Enemy (float viewDist, float viewAngle, float transitionWait, float speed)
    {
        viewDistance = viewDist;
        viewConeAngle = viewAngle;
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
                currentState = State.Investigate;
                lookingCount = 0;
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
        
    }

    //Added by Joe: //thank u Joe
    public void Damage(int hitPoints)
    {
        enemyHealth -= hitPoints;

        Debug.Log("ENEMY " + gameObject.name + " HAS " + enemyHealth.ToString());

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}


