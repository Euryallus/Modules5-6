using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Proof of Concept phase
// ## Purpose: Enemy #3 missile
//
public class missileTemp : MonoBehaviour
{
    [Header("Base alterations")]
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
        [SerializeField]
        float slowTime = 2f;
        [SerializeField]
        float purpleDamagePercent = 1.6f;

    private starStoneManager generator;
    private GameObject parent;
    private GameObject player;

    void Start()
    {
        // Stores scene instances of Player and generator manager using 'find object;
        player = GameObject.FindGameObjectWithTag("Player");
        generator = GameObject.FindGameObjectWithTag("GeneratorManager").GetComponent<starStoneManager>();
    }

    public void setParent(GameObject self) //allows 'parent' object (object that spawned missile) to be stored and affected
    {
        parent = self;
        missileDamage *= parent.GetComponent<Enemy>().difficulty[PlayerPrefs.GetInt("Difficulty", 1)].damagePercentageChange;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //moves the missile towards the player in a 'homing missile' style way, and rotates missile to face player
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, missileSpeed * Time.deltaTime);
        transform.LookAt(player.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //saves damage as local variable, allows changes based on current star stone
        float damageToDo = missileDamage;

        GameObject effect = Instantiate(explosion);
        effect.transform.position = transform.position;

        if (collision.gameObject.CompareTag("Player")) //on physics collision with the player;
        {
            //
            // ## SWITCH STATEMENT
            // ## Allows each star stone effect to be implemented when player is hit
            //

            switch (generator.returnActive())
            {
                case starStoneManager.starStones.Orange:
                    //
                    // ## ORANGE STAR STONE
                    // ## Burn enemy
                    //
                    collision.transform.gameObject.GetComponent<playerHealth>().setOnFire(fireLength, fireDamage, 0.5f);
                    break;

                case starStoneManager.starStones.Blue:
                    //
                    // ## BLUE STAR STONE
                    // ## slow player after hit
                    //
                    collision.transform.gameObject.GetComponent<playerMovement>().slowEffect(slowPercent, slowTime);
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
                    if (parent != null)
                    {
                        parent.GetComponent<Enemy>().externalRegenCall();

                    }
                    break;
            }

            collision.gameObject.GetComponent<playerHealth>().takeDamage(damageToDo); //Damage is inflicted on player and styar stone effects are added (e.g. player catches on fire)
        }

        AudioManager.instance.PlaySoundEffect3D("Explosion", transform.position);
        Destroy(gameObject); // Destroys missile on any plhysics collision (e.g. walls, floor, player etc.)
    }

    
        
}
