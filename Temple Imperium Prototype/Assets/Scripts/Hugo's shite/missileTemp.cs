using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileTemp : MonoBehaviour
{
    GameObject player;
    [SerializeField]
    GameObject explosion;

    [SerializeField]
    float missileSpeed = 3;

    [SerializeField]
    float missileDamage = 10f;

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

    generatorStates generator;

    GameObject parent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        generator = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<generatorStates>();
    }

    public void setParent(GameObject self)
    {
        parent = self;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, missileSpeed * Time.deltaTime);

        transform.LookAt(player.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float damageToDo = missileDamage;

        GameObject effect = Instantiate(explosion);
        effect.transform.position = transform.position;

        if (collision.gameObject.CompareTag("Player"))
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
                    if(parent != null)
                    {
                        parent.GetComponent<Enemy>().externalRegenCall();

                    }
                    break;
            }

            collision.gameObject.GetComponent<playerHealth>().takeDamage(damageToDo);
        }

        Destroy(gameObject);
    }

    
        
}
