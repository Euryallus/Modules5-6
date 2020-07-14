using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Base class for game enemies, includes basic behaviour for all states (patrol, investigate, engage)
//

[System.Serializable]
public class Enemy : MonoBehaviour
{
    //altered per child class
    [Header("Class specific")]
        [SerializeField]
        protected float viewDistance;
        [SerializeField]
        protected float transitionTime;
        [SerializeField]
        public float enemySpeed;
        [SerializeField]
        public float enemyHealth;
        [SerializeField]
        protected float regenPerSecond = 0.5f;
        [SerializeField]
        private GameObject healthBar;
        [SerializeField]
        protected float enemyViewAngle = 120f;

    [SerializeField]
    private Material damageMaterial;
        
    //altered per child class
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
        [SerializeField]
        float damageReductionPurpleStarStone = 0.5f;

    protected starStoneManager generator;
    protected bool hasAttacked = false;

    public bool hasHurt = false;

    private int enemyType;

    //
    // ## BASE VARIABLES 
    //

    public new List<Transform> patrolPoints = new List<Transform>();
    protected List<Vector3> investigatePoints = new List<Vector3>();

    protected NavMeshAgent agent;

    protected GameObject player;
    private GameObject currentTarget;

    protected int investigationPointNo = 3;
    [SerializeField]
    protected int currentPatrolPoint = 0;
    int investigatePointer = 0;

    protected float healTimeElapsed = 0;
    protected float hitCount = 0;
    protected float viewConeAngle;
    private float enemyMaxHealth;
    private float secondsPassed = 0;

    //CheckForPlayer variables
    protected Vector3 playerVector;
    protected Vector3 lastKnownPos;

    protected bool canSeePlayer = false;

    protected float playerDist;
    protected float lookingCount = 0;

    [Header("Difficulty presents")]
    [SerializeField]
    public List<difficultySetting> difficulty = new List<difficultySetting>();

    protected enum State
    {
        Idle,
        Patrol,
        Investigate,
        Engage
    }
    protected State currentState;

    public Enemy(float viewDist, float viewAngle, float transitionWait, float speed, int type) 
    {
        //
        // ## CLASS CONTRUCTOR
        //

        viewDistance = viewDist;
        enemyViewAngle = viewAngle;
        transitionTime = transitionWait;
        enemySpeed = speed;

        enemyType = type;
    }

    private void Start()
    {        
        // Assigns variables based on input from inspector and components attached to game objects
        agent = gameObject.GetComponent<NavMeshAgent>(); 
        
        lastKnownPos = gameObject.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = State.Patrol;

        //difficulty alterations

        enemyHealth *= difficulty[PlayerPrefs.GetInt("Difficulty", 1)].healthPercentageChange;
        agent.speed = enemySpeed * difficulty[PlayerPrefs.GetInt("Difficulty", 1)].speedPercentageChange;
        meleeHitDamage *= difficulty[PlayerPrefs.GetInt("Difficulty", 1)].damagePercentageChange;

        enemyMaxHealth = enemyHealth;
        healthBar = transform.GetChild(0).gameObject;

        generator = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<starStoneManager>(); //saves scene's instance of GeneratorManager (allows access to star stone states)
    }

    private void Update()
    {
        if (player.GetComponent<playerHealth>().isDead())
        {
            currentState = State.Idle;
        }
        else
        {
            checkForPlayer();
        }



        // 
        // ## SWITCH STATEMENT
        // ## Runs different Update functions for each different "state" in the AI's Finite State Machine
        //

        switch (currentState)
        {
            case State.Idle:
                //
                // ## IDLE STATE
                // ## Nothing happens - Debug state
                //
                if(gameObject.GetComponent<NavMeshAgent>().enabled == true)
                {
                    gameObject.GetComponent<NavMeshAgent>().enabled = false;
                }
                //agent.SetDestination(transform.position);

                break;

            case State.Patrol:
                //
                // ## PATROL STATE
                // ## Enemy moves between pre-determined patrol points
                //
                Patrol();
                break;

            case State.Investigate:
                //
                // ## INVESTIGATE STATE
                // ## Enemy moves between randomly selected points around the players last known location
                // ## Is activated after ENGAGE ends and player is out of line of sight
                //
                Investigate();
                break;

            case State.Engage:
                //
                // ## ENGAGE STATE
                // ## Enemy type specific behaviour
                // ## Base function simply saves last seen location of player, and checks to see if INVESTIGATE state should be activated based on time elapsed since last sighting
                //
                Engage();
                break;
        }
    }
    public virtual void checkForPlayer()
    {
        //
        // ## RUNS EVERY UPDATE
        // ## Establishes enemies field of view, and checks if player is 'visible'
        // ## Alters state based on whether or not player is visible to enemy
        //

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
        //
        // ## BASE ENGAGE FUNCTION
        // ## Run AFTER all child-specific behaviour is executed
        // ## Transitions between Engage and Investigate behaviour if player has not been seen for longer than pre-determined transitionTime
        //

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
        //
        // ## BASE PATROL FUNCTION
        // ## Alternates "patrol" point the enemy aims for while out of combat
        // ## PATROL is the default state for enemies
        //

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 4f)
        {
            currentPatrolPoint += 1;
            if (currentPatrolPoint == patrolPoints.Count)
            {
                currentPatrolPoint = 0;
            }
        }

        // Below fix used https://forum.unity.com/threads/navmesh-calculatepath-not-working-correctly.464243/ for bug fixing
        // prevents AI trying to "patrol" points it cannot reach yet due to doors / blocks
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(patrolPoints[currentPatrolPoint].position, path);

        if (path.status == NavMeshPathStatus.PathPartial)
        {
            currentPatrolPoint += 1;
            if (currentPatrolPoint == patrolPoints.Count)
            {
                currentPatrolPoint = 0;
                agent.SetDestination(patrolPoints[currentPatrolPoint].position);
            }
        }
        else
        {
            agent.SetDestination(patrolPoints[currentPatrolPoint].position);
        }
    }

    public virtual void Investigate()
    {
        //
        // ## BASE INVESTIGATE FUNCTION
        // ## On first run, 3+ random ACCESSABLE points are chosen around the last seen player location
        // ## Enemy moves between these 3+ points until they reach the final one, then transition back to PATROL behaviour
        //

        if (investigatePoints.Count == 0) //runs on first call of INVESTIGATE behaviour
        {
            investigatePointer = 0;

            for (int i = 0; i < investigationPointNo; i++) //creates pre-determined number of points to investigate (default 3)
            {
                Vector3 investigatePoint = transform.position + Random.insideUnitSphere * 10; //randomly generates point around players last known location (radius of 10)

                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(investigatePoint, path);

                if(path.status == NavMeshPathStatus.PathPartial || investigatePoint.y > 3) //checks point is accessable to NavMesh agent before 'committing' to it
                {
                    investigatePoint = transform.position + Random.insideUnitSphere * 10;
                }

                investigatePoints.Add(investigatePoint); //adds point to list of points to investigate
            }
        }

        if(Vector3.Distance(transform.position, investigatePoints[investigatePointer]) <= 1f) 
        {
            // 
            // Enemy moves towards current point of investigation
            // When within 1 meter of said point, cycle to the next in the list
            // if list is exhausted: transition to PATROL behaviour

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
        // Begins coroutine that damages enemy every x seconds, mimicking burning effect
        secondsPassed = 0;
        StartCoroutine(putOutFire(fireEffectTime, fireDamageTaken, timeBetweenDamage));
        
        //Plays fire particle effect assigned to current enemy
        transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();
        
    }

    public void externalRegenCall()
    {
        //allows external scripts to begin enemy health regen 
        healTimeElapsed = 0; //resets variable used in coroutine
        StartCoroutine(regenHealth(healthRegenLength, timeBetweenHealthRegen, healthRegenAmountPerCall));

    }

    protected IEnumerator regenHealth(float timeActive, float timeBetweenHeal, float healthPerCall) 
    {
        //
        // ## REGENERATE COROUTINE
        // ## Called every 'timeBetweenHeal' seconds for timeActive seconds
        // ## On each call, heals enemy for healthPerCall amount
        //

        while (healTimeElapsed < timeActive && enemyHealth < enemyMaxHealth)
        {
            enemyHealth += healthPerCall;
            healTimeElapsed += timeBetweenHeal;
            //Updates health bar size according to new health value
            float healthBarX = enemyHealth / enemyMaxHealth;
            healthBar.transform.localScale = new Vector3(healthBarX, 1, 1);
            yield return new WaitForSeconds(timeBetweenHeal);
        }
    }

    protected IEnumerator putOutFire(int fireEffectTime, int fireDamageTaken, float timeBetweemDamage)
    {
        //
        // ## FIRE DAMAGE COROUTINE
        // ## Called every 'timeBetweenDamage' seconds for 'fireEffectTime' seconds
        // ## Enemy takes 'fireDamageTaken' damage every call
        //

        while(secondsPassed < fireEffectTime)
        {
            Damage(fireDamageTaken);
            secondsPassed += timeBetweemDamage;
            yield return new WaitForSeconds(timeBetweemDamage);

            if(secondsPassed >= fireEffectTime)
            {
                // When time is fully elapsed, enemy is "put out" naturally
                // Fire particle effect is stopped
                transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Stop();

            }
        }
    }

    protected void meleeHit()
    {
        //
        // ## BASE MELEE ATTACK FOR ENEMIES (Used by variants 1 & 2)
        //

        RaycastHit target;
        float damageToDo = meleeHitDamage;

        if (Physics.Raycast(transform.position, transform.forward, out target, meleeDistance)) 
        {
            //
            // ## If Player is within melee range, Player takes damage according to base enemy damage & star stone effect active 
            // ## Star Stone effects are also added on hit, e.g. player is set on fire when hit, as well as taking base melee damage
            //

            GameObject hitObj = target.transform.gameObject;

            if (hitObj.CompareTag("Player") && hitCount > hitInterval)
            {
                //
                // ## SWITCH STATEMENT
                // ## DEPENDS ON STAR STONE ACTIVE
                // 

                switch (generator.returnActive())
                {
                    case starStoneManager.starStones.Orange:
                        //
                        // ## ORANGE STAR STONE
                        // ## Burn enemy
                        //
                        hitObj.GetComponent<playerHealth>().setOnFire(fireLength, fireDamage, 0.5f);
                        break;

                    case starStoneManager.starStones.Blue:
                        //
                        // ## BLUE STAR STONE
                        // ## slow player after hit
                        //
                        hitObj.GetComponent<playerMovement>().slowEffect(slowPercent, slowTime);

                        break;

                    case starStoneManager.starStones.Purple:
                        //
                        // ## PURPLE STAR STONE
                        // ## Damage done is increased by pre-determined %
                        //
                        damageToDo *= purpleDamagePercent;
                        break;

                    case starStoneManager.starStones.Pink:
                        //
                        // ## PINK STAR STONE
                        // ## Trigger enemy regen health
                        //
                        healTimeElapsed = 0;
                        StartCoroutine(regenHealth(healthRegenLength, timeBetweenHealthRegen, healthRegenAmountPerCall));

                        break;
                }

                hitObj.GetComponent<playerHealth>().takeDamage(damageToDo); // Player takes appropriate damage

                hitCount = 0; //resets hit timer 
            }
        }
    }

    private IEnumerator hurtColour()
    {
        yield return new WaitForSeconds(0.2f);
        hasHurt = false;
    }



    //Added by Joe: //thank u Joe
    public void Damage(int hitPoints)
    {
        float damageDone = hitPoints;

        hasHurt = true;
        gameObject.GetComponent<MeshRenderer>().material = damageMaterial;
        StartCoroutine(hurtColour());
        if (generator.returnActive() == starStoneManager.starStones.Purple)
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
        AudioManager.instance.PlaySoundEffect3D("Believe", transform.position, 1f, 0.95f, 1.05f);
        int pointsToAdd = 5;

        //SaveLoadManager code added by Joe - sets PlayerKilledEnemy/boss to 1 (i.e. true)
        //  so the fact that the player has killed a certain enemy type is saved. Used to show relevant info in the codex scene.
        switch (enemyType)
        {
            case 1:
                pointsToAdd = 5;
                SaveLoadManager.instance.SaveIntToPlayerPrefs("PlayerKilledEnemy1", 1);
                break;
            case 2:
                pointsToAdd = 10;
                SaveLoadManager.instance.SaveIntToPlayerPrefs("PlayerKilledEnemy2", 1);
                break;
            case 3:
                pointsToAdd = 15;
                SaveLoadManager.instance.SaveIntToPlayerPrefs("PlayerKilledEnemy3", 1);
                break;
            case 4:
                pointsToAdd = 30;
                SaveLoadManager.instance.SaveIntToPlayerPrefs("PlayerKilledBoss", 1);
                break;
        }

        player.GetComponent<playerHealth>().addScore(pointsToAdd);
        Destroy(gameObject);
    }

    
}


