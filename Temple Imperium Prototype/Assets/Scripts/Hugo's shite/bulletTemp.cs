using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Enemy #2 bullet 
//
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
        //saves scene instance of generatorManager using find object
        generator = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<generatorStates>();
    }

    public void setParent(GameObject self)
    {
        //sets scene reference to "parent" (enemy bullet was fired by)
        //called from "parent" on bullet instantiation
        parent = self;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //saves damage as local variable, allows changes based on current star stone
        float damageToDo = bulletDamage;

        if (collision.transform.gameObject.CompareTag("Player")) //on physics collision with the player;
        {
            //
            // ## SWITCH STATEMENT
            // ## Allows each star stone effect to be implemented when player is hit
            //

            switch (generator.returnState())
            {
                case generatorStates.starStoneActive.Orange:
                    //
                    // ## ORANGE STAR STONE
                    // ## Burn enemy
                    //
                    collision.transform.gameObject.GetComponent<playerHealth>().setOnFire(fireLength, fireDamage, 0.5f);
                    break;

                case generatorStates.starStoneActive.Blue:
                    //
                    // ## BLUE STAR STONE
                    // ## slow player after hit
                    //
                    collision.transform.gameObject.GetComponent<playerMovement>().slowEffect(slowPercent, slowTime);
                    break;

                case generatorStates.starStoneActive.Purple:
                    //
                    // ## PURPLE STAR STONE
                    // ## Damage done is increased by pre-determined %
                    //
                    damageToDo *= purpleDamagePercent;
                    break;

                case generatorStates.starStoneActive.Pink:
                    //
                    // ## PINK STAR STONE
                    // ## Trigger enemy regen health
                    //
                    if (parent != null) //check for if parent has already been killed - if not, begin health regen
                    {
                        parent.GetComponent<Enemy>().externalRegenCall();

                    }
                    break;
            }

            collision.transform.gameObject.GetComponent<playerHealth>().takeDamage(damageToDo); //player takes damage (either base or altered due to star stone effects above)
        }

        Destroy(gameObject); //bullet is destroyed on physics collision regardless of surface (e.g. wall, player, other enemy)
    }
}
