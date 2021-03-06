﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerHealth : MonoBehaviour
{

    [Header("Shield ability")]
        [SerializeField]
        private float shieldUpTime = 5f;
        [SerializeField]
        private float damageReductionPercent = 1f;
        private GameObject shield;

    private bool shieldActive = false;
    private IEnumerator coroutine;

    [Header("Player health")]
        [SerializeField]
        private float health = 30;
        private float maxHealth;
        [SerializeField]
        private Color fullHealthColour;
        [SerializeField]
        private Color ZeroHealthColour;
        [SerializeField]
        private float shieldRegenTime = 5f;

        [SerializeField]
        private float shieldSlowPercentage = 0.5f;

    public Image healthBar;


    //START STONE EFFECTS
    float secondsPassed = 0;
    public ParticleSystem fireEffect;
    bool onFire = false;
    [SerializeField]
    public Text stateDisplay;




    private void Start()
    {
        maxHealth = health;
        shield = gameObject.transform.GetChild(1).gameObject;
        shield.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && shieldActive == false)
        {
            shieldActive = true;
            gameObject.GetComponent<playerMovement>().slowEffect(shieldSlowPercentage, shieldUpTime);
            coroutine = sheildDown();
            StartCoroutine(coroutine);
            shield.SetActive(true);
        }

        float healthBarX = health / maxHealth  ;
        if (healthBarX < 0)
        {
            healthBarX = 0;
        }

        if(healthBar != null)
        {
            healthBar.transform.localScale = new Vector3(healthBarX, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        }
    }

    public IEnumerator sheildDown()
    {
        yield return new WaitForSeconds(shieldUpTime);
        
        shield.SetActive(false);

        yield return new WaitForSeconds(shieldRegenTime);

        shieldActive = false;
    }

    public bool isShieldActive()
    {
        return shieldActive;
    }

    public void takeDamage(float damageTaken)
    {
        float damage = damageTaken;
        if (shieldActive)
        {
            damage = damage * (1-damageReductionPercent);
        }

        healthBar.color = Color.Lerp(healthBar.color, ZeroHealthColour, damage / health);

        health -= damage;

        if(health == 0)
        {
            Debug.LogWarning("Player dead");
        }
    }

    public void setOnFire(float fireEffectTime, int fireDamageTaken, float timeBetweenDamage)
    {
        if(onFire == false)
        {
            stateDisplay.text = "On fire!";
            onFire = true;
            secondsPassed = 0;
            StartCoroutine(putOutFire(fireEffectTime, fireDamageTaken, timeBetweenDamage));

            fireEffect.Play();
        }
        
    }
    protected IEnumerator putOutFire(float fireEffectTime, int fireDamageTaken, float timeBetweemDamage)
    {
        while (secondsPassed < fireEffectTime)
        {
            takeDamage(fireDamageTaken);
            secondsPassed += timeBetweemDamage;
            yield return new WaitForSeconds(timeBetweemDamage);

            if (secondsPassed >= fireEffectTime)
            {
                fireEffect.Stop();
                onFire = false;
                stateDisplay.text = "";

            }
        }
    }

    //Added by Joe:
    public void RestoreHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, maxHealth);
    }

    

}
