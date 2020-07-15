using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Manages player's health & associated effects
//

public class playerHealth : MonoBehaviour
{
    [Header("Shield ability")]
        [SerializeField]
        private float shieldUpTime = 5f;
        [SerializeField]
        private float damageReductionPercent = 1f;
        private GameObject shield;

    [Header("Player health")]
        [SerializeField]
        private float health = 30;

        [SerializeField]
        private Color fullHealthColour;
        [SerializeField]
        private Color ZeroHealthColour;
        [SerializeField]
        private float shieldRegenTime = 5f;
        [SerializeField]
        private float shieldSlowPercentage = 0.5f;
        [SerializeField]
        private Image healthBar;

    [Header("Star stone effects")]
        [SerializeField]
        private  ParticleSystem fireEffect;
        [SerializeField]
        public Text stateDisplay;
    [SerializeField]
    public Image shieldUI;

    private float maxHealth;
    private float secondsPassed = 0;
    private bool onFire = false;
    private bool shieldActive = false;
    private IEnumerator coroutine;

    private GameObject healthFlash;
    private GameObject deathMenu;

    private bool isPlayerDead = false;
    private float shieldCooldown = 0;

    private bool isEndlessMode;
    private int endlessModeScore = 0;
    [SerializeField]
    private Text scoreUI;
    [SerializeField]
    private Text highScoreUI;
    [SerializeField]
    private Text scoreHUD;

    private void Start()
    {
        //assigns values & gameObjects accordingly, disables shield visual effect
        maxHealth = health;
        shield = gameObject.transform.GetChild(1).gameObject;
        shield.SetActive(false);

        healthFlash = GameObject.FindGameObjectWithTag("healthFlash");
        healthFlash.GetComponent<Image>().enabled = false;
        deathMenu = GameObject.FindGameObjectWithTag("deathMenu");
        deathMenu.SetActive(false);

        endlessModeScore = 0;

        if (PlayerPrefs.GetInt("EndlessMode", 0) == 1)
        {
            isEndlessMode = true;
        }
        else
        {
            isEndlessMode = false;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && shieldActive == false) //if Input = Q, activate shield, slow player & activate cooldown
        {
            shieldActive = true;
            gameObject.GetComponent<playerMovement>().slowEffect(shieldSlowPercentage, shieldUpTime);
            shieldCooldown = shieldUpTime;
            coroutine = shieldDown();
            StartCoroutine(coroutine);
            shield.SetActive(true);
        }

        float healthBarX = health / maxHealth; 

        if (healthBarX < 0) //sets X scale component based on current health & default size (max. health)
        {
            healthBarX = 0;
        }

        if(healthBar != null)
        {
            //update's health bar size based on current health
            healthBar.transform.localScale = new Vector3(healthBarX, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }

        if (healthFlash.GetComponent<Image>().IsActive())
        {
            healthFlash.GetComponent<Image>().color = new Color(healthFlash.GetComponent<Image>().color.r, healthFlash.GetComponent<Image>().color.g, healthFlash.GetComponent<Image>().color.b, healthFlash.GetComponent<Image>().color.a - Time.deltaTime);
            if(healthFlash.GetComponent<Image>().color.a <= 0)
            {
                healthFlash.GetComponent<Image>().enabled = false;
            }
        }

        if (isEndlessMode)
        {
            scoreHUD.text = "Score: " + endlessModeScore.ToString();
        }
    }

    public IEnumerator shieldDown() //cooldown for shield to disable again
    {
        shieldUI.color = new Color(shieldUI.color.r, shieldUI.color.g, shieldUI.color.b, 0);
        yield return new WaitForSeconds(shieldUpTime); //waits for X seconds to end shield effect, then another X seconds for it to reset
        
        shield.SetActive(false);
        

        yield return new WaitForSeconds(shieldRegenTime);
        shieldUI.color = new Color(shieldUI.color.r, shieldUI.color.g, shieldUI.color.b, 1);
        shieldActive = false;
    }

    public bool isDead()
    {
        return isPlayerDead;
    }

    public void stopMovement()
    {
        isPlayerDead = true;

        gameObject.GetComponent<playerMovement>().enabled = false;
        gameObject.GetComponent<WeaponHolder>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
    }

    public bool isShieldActive() //returns true / false value for if shield is active
    {
        return shieldActive;
    }

    public void takeDamage(float damageTaken) //manages player damage taken
    {
        float damage = damageTaken; //stores base damage as local variable to be fiddled with

        if (shieldActive)
        {
            damage = damage * (1-damageReductionPercent); //alters damage taken by pre-determined % when shield is active
        }

        healthBar.color = Color.Lerp(healthBar.color, ZeroHealthColour, damage / health); //alters colour of health bar depending on current health

        health -= damage;

        healthFlash.GetComponent<Image>().enabled = true;
        healthFlash.GetComponent<Image>().color = new Color(healthFlash.GetComponent<Image>().color.r, healthFlash.GetComponent<Image>().color.g, healthFlash.GetComponent<Image>().color.b, 0.3f);


        if (health <= 0)
        {
            // TEMPORARY DEATH STATE

            // ############################
            deathMenu.SetActive(true);
            if (isEndlessMode)
            {
                scoreUI.text = "Score: " + endlessModeScore.ToString();

                if(PlayerPrefs.GetInt("Highscore", 0) < endlessModeScore)
                {
                    PlayerPrefs.SetInt("Highscore", endlessModeScore);
                }

                highScoreUI.text = "Highscore: " + PlayerPrefs.GetInt("Highscore", endlessModeScore).ToString();
            }
            else
            {
                scoreUI.text = "";
                highScoreUI.text = "";
            }

            deathMenu.GetComponent<Animator>().Play("deadFade", 0);

            GameObject.FindGameObjectWithTag("spawnerManager").GetComponent<playStateControl>().playerDied();

            stopMovement();

            // ############################
        }
    }

    public void setOnFire(float fireEffectTime, int fireDamageTaken, float timeBetweenDamage) //Similar to enemies onFire effect, takes damage every few seconds for X seconds to mimic burning
    {
        if(onFire == false)
        {
            stateDisplay.text = "On fire!";
            onFire = true;
            secondsPassed = 0;
            StartCoroutine(putOutFire(fireEffectTime, fireDamageTaken, timeBetweenDamage)); //begins coroutine that damages player every X seconds until they get "put out"

            fireEffect.Play(); //enables fire particle effect
        }
        
    }
    protected IEnumerator putOutFire(float fireEffectTime, int fireDamageTaken, float timeBetweemDamage)
    {
        while (secondsPassed < fireEffectTime) //repeats every x seconds while player is still "on fire"
        {
            takeDamage(fireDamageTaken); //inflicts damage
            secondsPassed += timeBetweemDamage; //increases counter
            yield return new WaitForSeconds(timeBetweemDamage); 

            if (secondsPassed >= fireEffectTime) //once fire time has elapsed, stop particle effect, and "put out" player
            {
                fireEffect.Stop();
                onFire = false;
                stateDisplay.text = "";

            }
        }
    }

    public void addScore(int score)
    {
        endlessModeScore += score;
    }

    //Added by Joe:
    public void RestoreHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, maxHealth);

        //Hugo again
        healthBar.color = Color.Lerp(healthBar.color, fullHealthColour, amount / health); //alters colour of health bar depending on health added
    }

    

}
