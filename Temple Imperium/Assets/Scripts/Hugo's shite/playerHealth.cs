using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerHealth : MonoBehaviour
{

    [Header("Shield ability")]
        [SerializeField]
        private float shieldUpTime = 5f;
        [SerializeField]
        private float damageReductionPercent = 0.75f;
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

    public Image healthBar;

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
        shieldActive = false;
        shield.SetActive(false);
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
}
