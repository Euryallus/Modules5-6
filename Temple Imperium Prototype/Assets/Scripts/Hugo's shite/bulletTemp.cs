using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletTemp : MonoBehaviour
{
    [SerializeField]
    float bulletDamage = 5;

    generatorStates generator;

    [Header("Star stone effects")]
    [SerializeField]
    float fireLength = 4;
    [SerializeField]
    int fireDamage = 2;

    [SerializeField]
    float slowPercent = 0.6f;
    float slowTime = 2f;

    [SerializeField]
    float purpleDamagePercent = 1.6f;

    GameObject parent;

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<generatorStates>();
    }

    public void setParent(GameObject self)
    {
        parent = self;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float damageToDo = bulletDamage;

        if (collision.transform.gameObject.CompareTag("Player"))
        {
            //star stone effects
            switch (generator.returnState())
            {
                case generatorStates.starStoneActive.Orange:
                    //Burn enemy
                    collision.transform.gameObject.GetComponent<playerHealth>().setOnFire(fireLength, fireDamage, 0.5f);
                    break;

                case generatorStates.starStoneActive.Blue:
                    //slow player after hit
                    collision.transform.gameObject.GetComponent<playerMovement>().slowEffect(slowPercent, slowTime);
                    break;
                case generatorStates.starStoneActive.Purple:
                    damageToDo *= purpleDamagePercent;
                    break;

                case generatorStates.starStoneActive.Pink:
                    if (parent != null)
                    {
                        parent.GetComponent<Enemy>().externalRegenCall();

                    }
                    break;
            }

            collision.transform.gameObject.GetComponent<playerHealth>().takeDamage(damageToDo);
        }

        Destroy(gameObject);
    }
}
