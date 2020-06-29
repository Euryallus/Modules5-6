using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    [Header("Star Stone effects")]
    [SerializeField]
    protected int fireDamage = 2;
    [SerializeField]
    protected float fireLength = 4;

    [SerializeField]
    protected float slowPercent = 0.6f;
    [SerializeField]
    protected float slowTime = 2;

    [SerializeField]
    protected float purpleDamagePercent = 1.5f;

    protected float healTimeElapsed = 0;

    [SerializeField]
    protected float healthRegenLength = 3f;
    [SerializeField]
    protected float healthRegenAmountPerCall = 2f;
    [SerializeField]
    protected float timeBetweenHealthRegen = 0.5f;

    [SerializeField]
    protected float meleeDistance = 4f;
    [SerializeField]
    protected float meleeHitDamage = 5;

    [SerializeField]
    protected float hitInterval = 1f;
    protected float hitCount = 0;


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

    //STAR STONE CHANGES VARIABLES
    protected generatorStates generator;
    protected bool hasAttacked = false;

    public float damageReductionPurpleStarStone = 0.5f;

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

        generator = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<generatorStates>();
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

        hitCount += Time.deltaTime;
    }

    public virtual void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 4f)
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

    public void externalRegenCall()
    {
        healTimeElapsed = 0;
        StartCoroutine(regenHealth(healthRegenLength, timeBetweenHealthRegen, healthRegenAmountPerCall));

    }

    protected IEnumerator regenHealth(float timeActive, float timeBetweenHeal, float healthPerCall)
    {
        while (healTimeElapsed < timeActive && enemyHealth < enemyMaxHealth)
        {
            enemyHealth += healthPerCall;
            healTimeElapsed += timeBetweenHeal;
            float healthBarX = enemyHealth / enemyMaxHealth;
            healthBar.transform.localScale = new Vector3(healthBarX, 1, 1);
            yield return new WaitForSeconds(timeBetweenHeal);
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

    protected void meleeHit()
    {
        RaycastHit target;
        float damageToDo = meleeHitDamage;

        if (Physics.Raycast(transform.position, transform.forward, out target, meleeDistance))
        {
            GameObject hitObj = target.transform.gameObject;

            if (hitObj.CompareTag("Player") && hitCount > hitInterval)
            {
                //star stone effects
                switch (generator.returnState())
                {
                    case generatorStates.starStoneActive.Orange:
                        //Burn enemy
                        hitObj.GetComponent<playerHealth>().setOnFire(fireLength, fireDamage, 0.5f);
                        break;

                    case generatorStates.starStoneActive.Blue:

                        hitObj.GetComponent<playerMovement>().slowEffect(slowPercent, slowTime);

                        break;

                    case generatorStates.starStoneActive.Purple:
                        damageToDo *= purpleDamagePercent;
                        break;

                    case generatorStates.starStoneActive.Pink:
                        healTimeElapsed = 0;
                        StartCoroutine(regenHealth(healthRegenLength, timeBetweenHealthRegen, healthRegenAmountPerCall));

                        break;
                }

                hitObj.GetComponent<playerHealth>().takeDamage(damageToDo);

                hitCount = 0;
            }
        }
    }



    //Added by Joe: //thank u Joe
    public void Damage(int hitPoints)
    {
        float damageDone = hitPoints;

        if(generator.returnState() == generatorStates.starStoneActive.Purple)
        {
            damageDone /= 1 + damageReductionPurpleStarStone;
        }
        enemyHealth -= damageDone;

        if (enemyHealth <= 0)
        {
            Die();
            return;
        }

        float healthBarX = enemyHealth / enemyMaxHealth;
        healthBar.transform.localScale = new Vector3(healthBarX, 1, 1);
    }

    float originalEnemySpeed;
    bool slowingEnemy;
    Coroutine slowEnemyCoroutine;
    public void SlowEnemyForTime(float speedMultiplier, float time)
    {
        if (slowingEnemy)
        {
            enemySpeed = originalEnemySpeed;
            agent.speed = enemySpeed;

            slowingEnemy = false;
            StopCoroutine(slowEnemyCoroutine);
        }

        slowEnemyCoroutine = StartCoroutine(SlowEnemyForTimeCoroutine(speedMultiplier, time));
    }

    private IEnumerator SlowEnemyForTimeCoroutine(float speedMultiplier, float time)
    {
        originalEnemySpeed = enemySpeed;
        slowingEnemy = true;
        enemySpeed = originalEnemySpeed * speedMultiplier;
        agent.speed = enemySpeed;

        yield return new WaitForSeconds(time);

        enemySpeed = originalEnemySpeed;
        agent.speed = enemySpeed;
    }

    private void Die()
    {
        SoundEffectPlayer.instance.PlaySoundEffect3D("Believe", transform.position, 1f, 0.95f, 1.05f);
        Destroy(gameObject);
    }

    
}


